using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using BizHawk.Emulation.CPUs.M6502;

namespace BizHawk.Emulation.Consoles.Nintendo
{
	[CoreVersion("0.9.9.9",FriendlyName="NESHawk")]
	public partial class NES : IEmulator
	{
		//hardware/state
		public MOS6502X cpu;
		int cpu_accumulate; //cpu timekeeper
		public PPU ppu;
		public APU apu;
		byte[] ram;
		NESWatch[] sysbus_watch = new NESWatch[65536];
		public byte[] CIRAM; //AKA nametables
		string game_name; //friendly name exposed to user and used as filename base
		CartInfo cart; //the current cart prototype. should be moved into the board, perhaps
		INESBoard board; //the board hardware that is currently driving things
		public bool SoundOn = true;
		int sprdma_countdown; //used to 
		bool _irq_apu; //various irq signals that get merged to the cpu irq pin

		//irq state management
		public bool irq_apu { get { return _irq_apu; } set { _irq_apu = value; } }

		//user configuration 
		int[,] palette = new int[64,3];
		int[] palette_compiled = new int[64*8];
		IPortDevice[] ports;
		
		//Sound config
		public void SetSquare1(bool enabled) { apu.EnableSquare1 = enabled; }
		public void SetSquare2(bool enabled) { apu.EnableSquare2 = enabled; }
		public void SetTriangle(bool enabled) { apu.EnableTriangle = enabled; }
		public void SetNoise(bool enabled) { apu.EnableNoise = enabled; }
		public void SetDMC(bool enabled) { apu.EnableDMC = enabled; }

		public void HardReset()
		{
			cpu = new MOS6502X();
			cpu.DummyReadMemory = ReadMemory;
			cpu.ReadMemory = ReadMemory;
			cpu.WriteMemory = WriteMemory;
			cpu.BCD_Enabled = false;
			ppu = new PPU(this);
			apu = new APU(this);
			ram = new byte[0x800];
			CIRAM = new byte[0x800];
			ports = new IPortDevice[2];
			ports[0] = new JoypadPortDevice(this,0);
			ports[1] = new JoypadPortDevice(this,1);

			//fceux uses this technique, which presumably tricks some games into thinking the memory is randomized
			for (int i = 0; i < 0x800; i++)
			{
				if ((i & 4) != 0) ram[i] = 0xFF; else ram[i] = 0x00;
			}

			//in this emulator, reset takes place instantaneously
			cpu.PC = (ushort)(ReadMemory(0xFFFC) | (ReadMemory(0xFFFD) << 8));
			cpu.P = 0x34;
			cpu.S = 0xFD;
		}

		bool resetSignal;
		public void FrameAdvance(bool render)
		{
			videoProvider.FillFrameBuffer();

			lagged = true;
			if (resetSignal)
			{
				board.NESSoftReset();
				cpu.NESSoftReset();
				apu.NESSoftReset();
				//need to study what happens to ppu and apu and stuff..
			}

			Controller.UpdateControls(Frame++);
			//if (resetSignal)
				//Controller.UnpressButton("Reset");   TODO fix this
			resetSignal = Controller["Reset"];
			ppu.FrameAdvance();
			if (lagged)
			{
				_lagcount++;
				islag = true;
			}
			else
				islag = false;
		}

		//PAL:
		//0 15 30 45 60 -> 12 27 42 57 -> 9 24 39 54 -> 6 21 36 51 -> 3 18 33 48 -> 0
		//sequence of ppu clocks per cpu clock: 4,3,3,3,3
		//NTSC:
		//sequence of ppu clocks per cpu clock: 3
		static ByteBuffer cpu_sequence_NTSC = new ByteBuffer(new byte[]{3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3});
		static ByteBuffer cpu_sequence_PAL = new ByteBuffer(new byte[]{4,3,3,3,3,4,3,3,3,3,4,3,3,3,3,4,3,3,3,3,4,3,3,3,3,4,3,3,3,3,4,3,3,3,3,4,3,3,3,3});
		public int cpu_step, cpu_stepcounter, cpu_deadcounter;
		protected void RunCpuOne()
		{
			cpu_stepcounter++;
			if (cpu_stepcounter == cpu_sequence_NTSC[cpu_step])
			{
				cpu_step++;
				cpu_step &= 31;
				cpu_stepcounter = 0;

				if (sprdma_countdown > 0)
				{
					sprdma_countdown--;
					if (sprdma_countdown == 0)
					{
						//its weird that this is 514.. normally itd be 512 (and people would say its wrong) or 513 (and people would say its right)
						//but 514 passes test 4-irq_and_dma
						cpu_deadcounter = 514;
					}
				}

				if (cpu_deadcounter > 0)
					cpu_deadcounter--;
				else
				{
					cpu.IRQ = _irq_apu || board.IRQSignal;
					cpu.ExecuteOne();
				}

				apu.RunOne();
				ppu.PostCpuInstructionOne();
			}
		}

		public byte ReadReg(int addr)
		{
			switch (addr)
			{
				case 0x4000: case 0x4001: case 0x4002: case 0x4003:
				case 0x4004: case 0x4005: case 0x4006: case 0x4007:
				case 0x4008: case 0x4009: case 0x400A: case 0x400B:
				case 0x400C: case 0x400D: case 0x400E: case 0x400F:
				case 0x4010: case 0x4011: case 0x4012: case 0x4013:
					return apu.ReadReg(addr);
				case 0x4014: /*OAM DMA*/ break;
				case 0x4015: return apu.ReadReg(addr); 
				case 0x4016:
				case 0x4017:
					return read_joyport(addr);
				default:
					//Console.WriteLine("read register: {0:x4}", addr);
					break;

			}
			return 0xFF;
		}

		void WriteReg(int addr, byte val)
		{
			switch (addr)
			{
				case 0x4000: case 0x4001: case 0x4002: case 0x4003:
				case 0x4004: case 0x4005: case 0x4006: case 0x4007:
				case 0x4008: case 0x4009: case 0x400A: case 0x400B:
				case 0x400C: case 0x400D: case 0x400E: case 0x400F:
				case 0x4010: case 0x4011: case 0x4012: case 0x4013:
					apu.WriteReg(addr, val);
					break;
				case 0x4014: Exec_OAMDma(val); break;
				case 0x4015: apu.WriteReg(addr, val); break;
				case 0x4016:
					ports[0].Write(val & 1);
					ports[1].Write(val & 1);
					break;
				case 0x4017: apu.WriteReg(addr, val); break;
				default:
					//Console.WriteLine("wrote register: {0:x4} = {1:x2}", addr, val);
					break;
			}
		}

		byte read_joyport(int addr)
		{
			//read joystick port
			//many todos here
			lagged = false;
			byte ret;
			if(addr == 0x4016)
				ret = ports[0].Read();
			else ret = ports[1].Read();
			return ret;
		}

		void Exec_OAMDma(byte val)
		{
			ushort addr = (ushort)(val << 8);
			for (int i = 0; i < 256; i++)
			{
				byte db = ReadMemory((ushort)addr);
				WriteMemory(0x2004, db);
				addr++;
			}
			//schedule a sprite dma event for beginning 1 cycle in the future.
			//this receives 2 because thats just the way it works out.
			sprdma_countdown = 2;
		}

		/// <summary>
		/// sets the provided palette as current
		/// </summary>
		public void SetPalette(int[,] pal)
		{
			Array.Copy(pal,palette,64*3);
			for(int i=0;i<64*8;i++)
			{
				int d = i >> 6;
				int c = i & 63;
				int r = palette[c, 0];
				int g = palette[c, 1];
				int b = palette[c, 2];
				Palettes.ApplyDeemphasis(ref r, ref g, ref b, d);
				palette_compiled[i] = (int)unchecked((int)0xFF000000 | (r << 16) | (g << 8) | b);
			}
		}

		/// <summary>
		/// looks up an internal NES pixel value to an rgb int (applying the core's current palette and assuming no deemph)
		/// </summary>
		public int LookupColor(int pixel)
		{
			return palette_compiled[pixel];
		}

		public byte DummyReadMemory(ushort addr) { return 0; }

		public byte ReadMemory(ushort addr)
		{
			byte ret;
			if (addr < 0x0800) ret = ram[addr];
			else if(addr >= 0x8000) ret = board.ReadPRG(addr - 0x8000); //easy optimization, since rom reads are so common, move this up (reordering the rest of these elseifs is not easy)
			else if (addr < 0x1000) ret = ram[addr - 0x0800];
			else if (addr < 0x1800) ret = ram[addr - 0x1000];
			else if (addr < 0x2000) ret = ram[addr - 0x1800];
			else if (addr < 0x4000) ret = ppu.ReadReg(addr & 7);
			else if (addr < 0x4020) ret = ReadReg(addr); //we're not rebasing the register just to keep register names canonical
			else if (addr < 0x6000) ret = board.ReadEXP(addr - 0x4000);
			else ret = board.ReadWRAM(addr - 0x6000);
			
			//handle breakpoints and stuff.
			//the idea is that each core can implement its own watch class on an address which will track all the different kinds of monitors and breakpoints and etc.
			//but since freeze is a common case, it was implemented through its own mechanisms
			//if (sysbus_watch[addr] != null)
			//{
			//    sysbus_watch[addr].Sync();
			//    ret = sysbus_watch[addr].ApplyGameGenie(ret);
			//}

			return ret;
		}

		public void WriteMemory(ushort addr, byte value)
		{
			if (addr < 0x0800) ram[addr] = value;
			else if (addr < 0x1000) ram[addr - 0x0800] = value;
			else if (addr < 0x1800) ram[addr - 0x1000] = value;
			else if (addr < 0x2000) ram[addr - 0x1800] = value;
			else if (addr < 0x4000) ppu.WriteReg(addr & 7, value);
			else if (addr < 0x4020) WriteReg(addr, value);  //we're not rebasing the register just to keep register names canonical
			else if (addr < 0x6000) board.WriteEXP(addr - 0x4000, value); 
			else if (addr < 0x8000) board.WriteWRAM(addr - 0x6000, value);
			else board.WritePRG(addr - 0x8000, value);
		}

	}
}