using System;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

using BizHawk.Common;
using BizHawk.Common.NumberExtensions;

using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Components.Z80;

//http://www.ticalc.org/pub/text/calcinfo/

namespace BizHawk.Emulation.Cores.Calculators
{
	[CoreAttributes(
		"TI83Hawk",
		"zeromus",
		isPorted: false,
		isReleased: true
		)]
	public class TI83 : IEmulator
	{
		//hardware
		private readonly Z80A cpu = new Z80A();
		private readonly byte[] rom;
		private byte[] ram;
		private int romPageLow3Bits;
		private int romPageHighBit;
		private byte maskOn;
		private bool onPressed;
		private int keyboardMask;

		private int disp_mode;
		private int disp_move;
		private uint disp_x, disp_y;
		private int m_LinkOutput, m_LinkInput;

		private int m_LinkState
		{
			get
			{
				return (m_LinkOutput | m_LinkInput) ^ 3;
			}
		}

		private bool LinkActive;
		private bool m_CursorMoved;

		//-------

		public byte ReadMemory(ushort addr)
		{
			byte ret;
			int romPage = romPageLow3Bits | (romPageHighBit << 3);
			//Console.WriteLine("read memory: {0:X4}", addr);
			if (addr < 0x4000)
				ret = rom[addr]; //ROM zero-page
			else if (addr < 0x8000)
				ret = rom[romPage * 0x4000 + addr - 0x4000]; //other rom page
			else ret = ram[addr - 0x8000];

			CoreComm.MemoryCallbackSystem.CallRead(addr);

			return ret;
		}

		public void WriteMemory(ushort addr, byte value)
		{
			if (addr < 0x4000)
				return; //ROM zero-page
			else if (addr < 0x8000)
				return; //other rom page
			else ram[addr - 0x8000] = value;

			CoreComm.MemoryCallbackSystem.CallWrite(addr);
		}

		public void WriteHardware(ushort addr, byte value)
		{
			switch (addr)
			{
				case 0: //PORT_LINK
					romPageHighBit = (value >> 4) & 1;
					m_LinkOutput = value & 3;

					if (LinkActive)
					{
						//Prevent rom calls from disturbing link port activity
						if (LinkActive && cpu.RegisterPC < 0x4000)
							return;

						LinkPort.Update();
					}
					break;
				case 1: //PORT_KEYBOARD:
					lagged = false;
					keyboardMask = value;
					//Console.WriteLine("write PORT_KEYBOARD {0:X2}",value);
					break;
				case 2: //PORT_ROMPAGE
					romPageLow3Bits = value & 0x7;
					break;
				case 3: //PORT_STATUS
					maskOn = (byte)(value & 1);
					break;
				case 16: //PORT_DISPCTRL
					//Console.WriteLine("write PORT_DISPCTRL {0}",value);
					WriteDispCtrl(value);
					break;
				case 17: //PORT_DISPDATA
					//Console.WriteLine("write PORT_DISPDATA {0}",value);
					WriteDispData(value);
					break;
			}
		}

		public byte ReadHardware(ushort addr)
		{
			switch (addr)
			{
				case 0: //PORT_LINK
					LinkPort.Update();
					return (byte)((romPageHighBit << 4) | (m_LinkState << 2) | m_LinkOutput);
				case 1: //PORT_KEYBOARD:
					//Console.WriteLine("read PORT_KEYBOARD");
					return ReadKeyboard();
				case 2: //PORT_ROMPAGE
					return (byte)romPageLow3Bits;
				case 3: //PORT_STATUS
					{
						//Console.WriteLine("read PORT_STATUS");
						// Bits:
						// 0   - Set if ON key is down and ON key is trapped
						// 1   - Update things (keyboard etc)
						// 2   - Unknown, but used
						// 3   - Set if ON key is up
						// 4-7 - Unknown
						//if (onPressed && maskOn) ret |= 1;
						//if (!onPressed) ret |= 0x8;
						return (byte)((Controller.IsPressed("ON") ? maskOn : 8) | (LinkActive ? 0 : 2));
					}

				case 4: //PORT_INTCTRL
					//Console.WriteLine("read PORT_INTCTRL");
					return 0xFF;

				case 16: //PORT_DISPCTRL
					//Console.WriteLine("read DISPCTRL");
					break;

				case 17: //PORT_DISPDATA
					return ReadDispData();
			}
			return 0xFF;
		}

		byte ReadKeyboard()
		{
			CoreComm.InputCallback.Call();
			//ref TI-9X

			int ret = 0xFF;
			//Console.WriteLine("keyboardMask: {0:X2}",keyboardMask);
			if ((keyboardMask & 1) == 0)
			{
				if (Controller.IsPressed("DOWN")) ret ^= 1;
				if (Controller.IsPressed("LEFT")) ret ^= 2;
				if (Controller.IsPressed("RIGHT")) ret ^= 4;
				if (Controller.IsPressed("UP")) ret ^= 8;
			}
			if ((keyboardMask & 2) == 0)
			{
				if (Controller.IsPressed("ENTER")) ret ^= 1;
				if (Controller.IsPressed("PLUS")) ret ^= 2;
				if (Controller.IsPressed("MINUS")) ret ^= 4;
				if (Controller.IsPressed("MULTIPLY")) ret ^= 8;
				if (Controller.IsPressed("DIVIDE")) ret ^= 16;
				if (Controller.IsPressed("EXP")) ret ^= 32;
				if (Controller.IsPressed("CLEAR")) ret ^= 64;
			}
			if ((keyboardMask & 4) == 0)
			{
				if (Controller.IsPressed("DASH")) ret ^= 1;
				if (Controller.IsPressed("3")) ret ^= 2;
				if (Controller.IsPressed("6")) ret ^= 4;
				if (Controller.IsPressed("9")) ret ^= 8;
				if (Controller.IsPressed("PARACLOSE")) ret ^= 16;
				if (Controller.IsPressed("TAN")) ret ^= 32;
				if (Controller.IsPressed("VARS")) ret ^= 64;
			}
			if ((keyboardMask & 8) == 0)
			{
				if (Controller.IsPressed("DOT")) ret ^= 1;
				if (Controller.IsPressed("2")) ret ^= 2;
				if (Controller.IsPressed("5")) ret ^= 4;
				if (Controller.IsPressed("8")) ret ^= 8;
				if (Controller.IsPressed("PARAOPEN")) ret ^= 16;
				if (Controller.IsPressed("COS")) ret ^= 32;
				if (Controller.IsPressed("PRGM")) ret ^= 64;
				if (Controller.IsPressed("STAT")) ret ^= 128;
			}
			if ((keyboardMask & 16) == 0)
			{
				if (Controller.IsPressed("0")) ret ^= 1;
				if (Controller.IsPressed("1")) ret ^= 2;
				if (Controller.IsPressed("4")) ret ^= 4;
				if (Controller.IsPressed("7")) ret ^= 8;
				if (Controller.IsPressed("COMMA")) ret ^= 16;
				if (Controller.IsPressed("SIN")) ret ^= 32;
				if (Controller.IsPressed("MATRIX")) ret ^= 64;
				if (Controller.IsPressed("X")) ret ^= 128;
			}

			if ((keyboardMask & 32) == 0)
			{
				if (Controller.IsPressed("STO")) ret ^= 2;
				if (Controller.IsPressed("LN")) ret ^= 4;
				if (Controller.IsPressed("LOG")) ret ^= 8;
				if (Controller.IsPressed("SQUARED")) ret ^= 16;
				if (Controller.IsPressed("NEG1")) ret ^= 32;
				if (Controller.IsPressed("MATH"))
					ret ^= 64;
				if (Controller.IsPressed("ALPHA")) ret ^= 128;
			}

			if ((keyboardMask & 64) == 0)
			{
				if (Controller.IsPressed("GRAPH")) ret ^= 1;
				if (Controller.IsPressed("TRACE")) ret ^= 2;
				if (Controller.IsPressed("ZOOM")) ret ^= 4;
				if (Controller.IsPressed("WINDOW")) ret ^= 8;
				if (Controller.IsPressed("Y")) ret ^= 16;
				if (Controller.IsPressed("2ND")) ret ^= 32;
				if (Controller.IsPressed("MODE")) ret ^= 64;
				if (Controller.IsPressed("DEL")) ret ^= 128;
			}

			return (byte)ret;

		}

		byte ReadDispData()
		{
			if (m_CursorMoved)
			{
				m_CursorMoved = false;
				return 0x00; //not accurate this should be stale data or something
			}

			byte ret;
			if (disp_mode == 1)
			{
				ret = vram[disp_y * 12 + disp_x];
			}
			else
			{
				int column = 6 * (int)disp_x;
				int offset = (int)disp_y * 12 + (column >> 3);
				int shift = 10 - (column & 7);
				ret = (byte)(((vram[offset] << 8) | vram[offset + 1]) >> shift);
			}

			doDispMove();
			return ret;
		}

		void WriteDispData(byte value)
		{
			int offset;
			if (disp_mode == 1)
			{
				offset = (int)disp_y * 12 + (int)disp_x;
				vram[offset] = value;
			}
			else
			{
				int column = 6 * (int)disp_x;
				offset = (int)disp_y * 12 + (column >> 3);
				if (offset < 0x300)
				{
					int shift = column & 7;
					int mask = ~(252 >> shift);
					int Data = value << 2;
					vram[offset] = (byte)(vram[offset] & mask | (Data >> shift));
					if (shift > 2 && offset < 0x2ff)
					{
						offset++;

						shift = 8 - shift;

						mask = ~(252 << shift);
						vram[offset] = (byte)(vram[offset] & mask | (Data << shift));
					}
				}
			}

			doDispMove();
		}

		void doDispMove()
		{
			switch (disp_move)
			{
				case 0: disp_y--; break;
				case 1: disp_y++; break;
				case 2: disp_x--; break;
				case 3: disp_x++; break;
			}

			disp_x &= 0xF; //0xF or 0x1F? dunno
			disp_y &= 0x3F;
		}

		void WriteDispCtrl(byte value)
		{
			if (value <= 1)
				disp_mode = value;
			else if (value >= 4 && value <= 7)
				disp_move = value - 4;
			else if ((value & 0xC0) == 0x40)
			{
				//hardware scroll
			}
			else if ((value & 0xE0) == 0x20)
			{
				disp_x = (uint)(value & 0x1F);
				m_CursorMoved = true;
			}
			else if ((value & 0xC0) == 0x80)
			{
				disp_y = (uint)(value & 0x3F);
				m_CursorMoved = true;
			}
			else if ((value & 0xC0) == 0xC0)
			{
				//contrast
			}
			else if (value == 2)
			{
			}
			else if (value == 3)
			{
			}
			else
			{
			}
		}

		public TI83(CoreComm comm, GameInfo game, byte[] rom, object Settings)
		{
			PutSettings(Settings ?? new TI83Settings());

			CoreComm = comm;
			cpu.ReadMemory = ReadMemory;
			cpu.WriteMemory = WriteMemory;
			cpu.ReadHardware = ReadHardware;
			cpu.WriteHardware = WriteHardware;
			cpu.IRQCallback = IRQCallback;
			cpu.NMICallback = NMICallback;

			this.rom = rom;
			LinkPort = new Link(this);

			//different calculators (different revisions?) have different initPC. we track this in the game database by rom hash
			//if( *(unsigned long *)(m_pRom + 0x6ce) == 0x04D3163E ) m_Regs.PC.W = 0x6ce; //KNOWN
			//else if( *(unsigned long *)(m_pRom + 0x6f6) == 0x04D3163E ) m_Regs.PC.W = 0x6f6; //UNKNOWN

			if (game["initPC"])
				startPC = ushort.Parse(game.OptionValue("initPC"), NumberStyles.HexNumber);

			HardReset();
			SetupMemoryDomains();
		}

		void IRQCallback()
		{
			//Console.WriteLine("IRQ with vec {0} and cpu.InterruptMode {1}", cpu.RegisterI, cpu.InterruptMode);
			cpu.Interrupt = false;
		}

		void NMICallback()
		{
			Console.WriteLine("NMI");
			cpu.NonMaskableInterrupt = false;
		}


		public CoreComm CoreComm { get; private set; }

		protected byte[] vram = new byte[0x300];
		class MyVideoProvider : IVideoProvider
		{
			private readonly TI83 emu;
			public MyVideoProvider(TI83 emu)
			{
				this.emu = emu;
			}

			public int[] GetVideoBuffer()
			{
				//unflatten bit buffer
				int[] pixels = new int[96 * 64];
				int i = 0;
				for (int y = 0; y < 64; y++)
					for (int x = 0; x < 96; x++)
					{
						int offset = y * 96 + x;
						int bufbyte = offset >> 3;
						int bufbit = offset & 7;
						int bit = ((emu.vram[bufbyte] >> (7 - bufbit)) & 1);
						if (bit == 0)
						{
							unchecked { pixels[i++] = (int)emu.Settings.BGColor; }
						}
						else
						{
							pixels[i++] = (int)emu.Settings.ForeColor;
						}

					}
				return pixels;
			}

			public int VirtualWidth { get { return 96; } }
			public int VirtualHeight { get { return 64; } }
			public int BufferWidth { get { return 96; } }
			public int BufferHeight { get { return 64; } }
			public int BackgroundColor { get { return 0; } }
		}
		public IVideoProvider VideoProvider
		{
			get { return new MyVideoProvider(this); }
		}

		public ISoundProvider SoundProvider { get { return NullSound.SilenceProvider; } }
		public ISyncSoundProvider SyncSoundProvider { get { return new FakeSyncSound(NullSound.SilenceProvider, 735); } }
		public bool StartAsyncSound() { return true; }
		public void EndAsyncSound() { }

		public static readonly ControllerDefinition TI83Controller =
			new ControllerDefinition
			{
				Name = "TI83 Controller",
				BoolButtons = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9","DOT",
					"ON","ENTER",
					"DOWN","LEFT","UP","RIGHT",
					"PLUS","MINUS","MULTIPLY","DIVIDE",
					"CLEAR", "EXP", "DASH", "PARACLOSE", "TAN", "VARS", "PARAOPEN",
					"COS", "PRGM", "STAT", "COMMA", "SIN", "MATRIX", "X",
					"STO", "LN", "LOG", "SQUARED", "NEG1", "MATH", "ALPHA",
					"GRAPH", "TRACE", "ZOOM", "WINDOW", "Y", "2ND", "MODE", "DEL"
				}
			};

		public ControllerDefinition ControllerDefinition { get { return TI83Controller; } }

		public IController Controller { get; set; }

		//configuration
		ushort startPC;

		public void FrameAdvance(bool render, bool rendersound)
		{
			lagged = true;
			//I eyeballed this speed
			for (int i = 0; i < 5; i++)
			{
				onPressed = Controller.IsPressed("ON");
				//and this was derived from other emus
				cpu.ExecuteCycles(10000);
				cpu.Interrupt = true;
			}

			Frame++;
			if (lagged)
			{
				lagCount++;
				isLag = true;
			}
			else
			{
				isLag = false;
			}
		}

		public void HardReset()
		{
			cpu.Reset();
			ram = new byte[0x8000];
			for (int i = 0; i < 0x8000; i++)
				ram[i] = 0xFF;
			cpu.RegisterPC = startPC;

			cpu.IFF1 = false;
			cpu.IFF2 = false;
			cpu.InterruptMode = 2;

			maskOn = 1;
			romPageHighBit = 0;
			romPageLow3Bits = 0;
			keyboardMask = 0;

			disp_mode = 0;
			disp_move = 0;
			disp_x = disp_y = 0;
		}

		private int lagCount = 0;
		private bool lagged = true;
		private bool isLag = false;
		private int frame;
		public int Frame { get { return frame; } set { frame = value; } }
		public int LagCount { get { return lagCount; } set { lagCount = value; } }
		public bool IsLagFrame { get { return isLag; } }

		public void ResetCounters()
		{
			Frame = 0;
			lagCount = 0;
			isLag = false;
		}

		public bool DeterministicEmulation { get { return true; } }

		public byte[] ReadSaveRam() { return null; }
		public void StoreSaveRam(byte[] data) { }
		public void ClearSaveRam() { }
		public bool SaveRamModified
		{
			get { return false; }
			set { }
		}

		public bool BinarySaveStatesPreferred { get { return false; } }
		public void SaveStateBinary(BinaryWriter bw) { SyncState(Serializer.CreateBinaryWriter(bw)); }
		public void LoadStateBinary(BinaryReader br) { SyncState(Serializer.CreateBinaryReader(br)); }
		public void SaveStateText(TextWriter tw) { SyncState(Serializer.CreateTextWriter(tw)); }
		public void LoadStateText(TextReader tr) { SyncState(Serializer.CreateTextReader(tr)); }

		void SyncState(Serializer ser)
		{
			ser.BeginSection("TI83");
			cpu.SyncState(ser);
			ser.Sync("RAM", ref ram, false);
			ser.Sync("romPageLow3Bits", ref romPageLow3Bits);
			ser.Sync("romPageHighBit", ref romPageHighBit);
			ser.Sync("disp_mode", ref disp_mode);
			ser.Sync("disp_move", ref disp_move);
			ser.Sync("disp_x", ref disp_x);
			ser.Sync("disp_y", ref disp_y);
			ser.Sync("m_CursorMoved", ref m_CursorMoved);
			ser.Sync("maskOn", ref maskOn);
			ser.Sync("onPressed", ref onPressed);
			ser.Sync("keyboardMask", ref keyboardMask);
			ser.Sync("m_LinkOutput", ref m_LinkOutput);
			ser.Sync("VRAM", ref vram, false);
			ser.Sync("Frame", ref frame);
			ser.Sync("LagCount", ref lagCount);
			ser.Sync("IsLag", ref isLag);
			ser.EndSection();
		}

		byte[] stateBuffer;
		public byte[] SaveStateBinary()
		{
			if (stateBuffer == null)
			{
				var stream = new MemoryStream();
				var writer = new BinaryWriter(stream);
				SaveStateBinary(writer);
				stateBuffer = stream.ToArray();
				writer.Close();
				return stateBuffer;
			}
			else
			{
				var stream = new MemoryStream(stateBuffer);
				var writer = new BinaryWriter(stream);
				SaveStateBinary(writer);
				writer.Close();
				return stateBuffer;
			}
		}

		public string SystemId { get { return "TI83"; } }
		public string BoardName { get { return null; } }

		private MemoryDomainList _memoryDomains;
		private const ushort RamSizeMask = 0x7FFF;

		private void SetupMemoryDomains()
		{
			var domains = new List<MemoryDomain>
			{
				new MemoryDomain(
					"Main RAM",
					ram.Length,
					MemoryDomain.Endian.Little,
					addr => ram[addr],
					(addr, value) => ram[addr] = value
				)
			};

			_memoryDomains = new MemoryDomainList(domains);
		}

		public MemoryDomainList MemoryDomains { get { return _memoryDomains; } }

		public void Dispose() { }

		public Link LinkPort;

		public class Link
		{
			// Emulates TI linking software.
			// See http://www.ticalc.org/archives/files/fileinfo/294/29418.html for documentation

			// Note: Each hardware read/write to the link port calls tthe update method.
			readonly TI83 Parent;

			private FileStream CurrentFile;
			//private int FileBytesLeft;
			private byte[] VariableData;

			private Action NextStep;
			private Queue<byte> CurrentData = new Queue<byte>();
			private ushort BytesToSend;
			private byte BitsLeft;
			private byte CurrentByte;
			private byte StepsLeft;

			private Status CurrentStatus = Status.Inactive;

			private enum Status
			{
				Inactive,
				PrepareReceive,
				PrepareSend,
				Receive,
				Send
			}

			public Link(TI83 Parent)
			{
				this.Parent = Parent;
			}

			public void Update()
			{
				if (CurrentStatus == Status.PrepareReceive)
				{
					//Get the first byte, and start sending it.
					CurrentByte = CurrentData.Dequeue();
					CurrentStatus = Status.Receive;
					BitsLeft = 8;
					StepsLeft = 5;
				}

				if (CurrentStatus == Status.PrepareSend && Parent.m_LinkState != 3)
				{
					CurrentStatus = Status.Send;
					BitsLeft = 8;
					StepsLeft = 5;
					CurrentByte = 0;
				}

				if (CurrentStatus == Status.Receive)
				{
					switch (StepsLeft)
					{
						case 5:
							//Receive step 1: Lower the other device's line.
							Parent.m_LinkInput = ((CurrentByte & 1) == 1) ? 2 : 1;
							CurrentByte >>= 1;
							StepsLeft--;
							break;

						case 4:
							//Receive step 2: Wait for the calc to lower the other line.
							if ((Parent.m_LinkState & 3) == 0)
								StepsLeft--;
							break;

						case 3:
							//Receive step 3: Raise the other device's line back up.
							Parent.m_LinkInput = 0;
							StepsLeft--;
							break;

						case 2:
							//Receive step 4: Wait for the calc to raise its line back up.
							if ((Parent.m_LinkState & 3) == 3)
								StepsLeft--;
							break;

						case 1:
							//Receive step 5: Finish.   
							BitsLeft--;

							if (BitsLeft == 0)
							{
								if (CurrentData.Count > 0)
									CurrentStatus = Status.PrepareReceive;
								else
								{
									CurrentStatus = Status.Inactive;
									if (NextStep != null)
										NextStep();
								}
							}
							else
								//next bit in the current byte.
								StepsLeft = 5;
							break;
					}
				}
				else if (CurrentStatus == Status.Send)
				{
					switch (StepsLeft)
					{
						case 5:
							//Send step 1: Calc lowers a line.
							if (Parent.m_LinkState != 3)
							{
								int Bit = Parent.m_LinkState & 1;
								int Shift = 8 - BitsLeft;
								CurrentByte |= (byte)(Bit << Shift);
								StepsLeft--;
							}
							break;

						case 4:
							//send step 2: Lower our line.
							Parent.m_LinkInput = Parent.m_LinkOutput ^ 3;
							StepsLeft--;
							break;

						case 3:
							//Send step 3: wait for the calc to raise its line.
							if ((Parent.m_LinkOutput & 3) == 0)
								StepsLeft--;
							break;

						case 2:
							//Send step 4: raise the other devices lines.
							Parent.m_LinkInput = 0;
							StepsLeft--;
							break;

						case 1:
							//Send step 5: Finish
							BitsLeft--;

							if (BitsLeft == 0)
							{
								BytesToSend--;
								CurrentData.Enqueue(CurrentByte);

								if (BytesToSend > 0)
									CurrentStatus = Status.PrepareSend;
								else
								{
									CurrentStatus = Status.Inactive;
									if (NextStep != null)
										NextStep();
								}
							}
							else
							{
								//next bit in the current byte.
								StepsLeft = 5;
							}
							break;
					}
				}
			}

			public void SendFileToCalc(FileStream FS, bool Verify)
			{
				if (Verify)
					VerifyFile(FS);

				FS.Seek(55, SeekOrigin.Begin);
				CurrentFile = FS;
				SendNextFile();
			}

			private void VerifyFile(FileStream FS)
			{
				//Verify the file format.
				byte[] Expected = new byte[] { 0x2a, 0x2a, 0x54, 0x49, 0x38, 0x33, 0x2a, 0x2a, 0x1a, 0x0a, 0x00 };
				byte[] Actual = new byte[11];

				FS.Seek(0, SeekOrigin.Begin);
				FS.Read(Actual, 0, 11);

				//Check the header.
				for (int n = 0; n < 11; n++)
					if (Expected[n] != Actual[n])
					{
						FS.Close();
						throw new IOException("Invalid Header.");
					}

				//Seek to the end of the comment.
				FS.Seek(53, SeekOrigin.Begin);

				int Size = FS.ReadByte() + FS.ReadByte() * 256;

				if (FS.Length != Size + 57)
				{
					FS.Close();
					throw new IOException("Invalid file length.");
				}

				//Verify the checksum.
				ushort Checksum = 0;
				for (int n = 0; n < Size; n++)
					Checksum += (ushort)FS.ReadByte();

				ushort ActualChecksum = (ushort)(FS.ReadByte() + FS.ReadByte() * 256);

				if (Checksum != ActualChecksum)
				{
					FS.Close();
					throw new IOException("Invalid Checksum.");
				}
			}

			private void SendNextFile()
			{
				byte[] Header = new byte[13];
				if (!CurrentFile.CanRead || CurrentFile.Read(Header, 0, 13) != 13)
				{
					//End of file.
					CurrentFile.Close();
					return;
				}

				int Size = Header[2] + Header[3] * 256;
				VariableData = new byte[Size + 2];
				CurrentFile.Read(VariableData, 0, Size + 2);

				//Request to send the file.
				CurrentData.Clear();

				CurrentData.Enqueue(0x03);
				CurrentData.Enqueue(0xC9);
				foreach (byte B in Header)
					CurrentData.Enqueue(B);

				//Calculate the checksum for the command.
				ushort Checksum = 0;
				for (int n = 2; n < Header.Length; n++)
					Checksum += Header[n];

				CurrentData.Enqueue((byte)(Checksum % 256));
				CurrentData.Enqueue((byte)(Checksum / 256));

				//Finalize the command.
				CurrentStatus = Status.PrepareReceive;
				NextStep = ReceiveReqAck;
				Parent.LinkActive = true;
			}

			private void ReceiveReqAck()
			{
				Parent.LinkActive = false;
				CurrentData.Clear();

				// Prepare to receive the Aknowledgement response from the calculator.
				BytesToSend = 8;
				CurrentStatus = Status.PrepareSend;
				NextStep = SendVariableData;
			}

			private void SendVariableData()
			{
				// Check to see if out of memory first.
				CurrentData.Dequeue();
				CurrentData.Dequeue();
				CurrentData.Dequeue();
				CurrentData.Dequeue();
				CurrentData.Dequeue();

				if (CurrentData.Dequeue() == 0x36)
					OutOfMemory();
				else
				{
					CurrentData.Clear();

					CurrentData.Enqueue(0x03);
					CurrentData.Enqueue(0x56);
					CurrentData.Enqueue(0x00);
					CurrentData.Enqueue(0x00);

					CurrentData.Enqueue(0x03);
					CurrentData.Enqueue(0x15);

					//Add variable data.
					foreach (byte B in VariableData)
						CurrentData.Enqueue(B);

					//Calculate the checksum.
					ushort Checksum = 0;
					for (int n = 2; n < VariableData.Length; n++)
						Checksum += VariableData[n];

					CurrentData.Enqueue((byte)(Checksum % 256));
					CurrentData.Enqueue((byte)(Checksum / 256));

					CurrentStatus = Status.PrepareReceive;
					NextStep = ReceiveDataAck;
					Parent.LinkActive = true;
				}
			}

			private void ReceiveDataAck()
			{
				Parent.LinkActive = false;
				CurrentData.Clear();

				// Prepare to receive the Aknowledgement response from the calculator.
				BytesToSend = 4;
				CurrentStatus = Status.PrepareSend;
				NextStep = EndTransmission;
			}

			private void EndTransmission()
			{
				CurrentData.Clear();

				// Send the end transmission command.
				CurrentData.Enqueue(0x03);
				CurrentData.Enqueue(0x92);
				CurrentData.Enqueue(0x00);
				CurrentData.Enqueue(0x00);

				CurrentStatus = Status.PrepareReceive;
				NextStep = FinalizeFile;
				Parent.LinkActive = true;
			}

			private void OutOfMemory()
			{
				CurrentFile.Close();
				Parent.LinkActive = false;
				CurrentData.Clear();

				// Prepare to receive the Aknowledgement response from the calculator.
				BytesToSend = 3;
				CurrentStatus = Status.PrepareSend;
				NextStep = EndOutOfMemory;
			}

			private void EndOutOfMemory()
			{
				CurrentData.Clear();

				// Send the end transmission command.
				CurrentData.Enqueue(0x03);
				CurrentData.Enqueue(0x56);
				CurrentData.Enqueue(0x01);
				CurrentData.Enqueue(0x00);

				CurrentStatus = Status.PrepareReceive;
				NextStep = FinalizeFile;
				Parent.LinkActive = true;
			}

			private void FinalizeFile()
			{
				// Resets the link software, and checks to see if there is an additional file to send.
				CurrentData.Clear();
				Parent.LinkActive = false;
				NextStep = null;
				SendNextFile();
			}
		}

		public class TI83Settings
		{
			public uint BGColor = 0x889778;
			public uint ForeColor = 0x36412D;

			public TI83Settings()
			{
			}

			public TI83Settings Clone()
			{
				return (TI83Settings)MemberwiseClone();
			}
		}

		TI83Settings Settings;

		public object GetSettings() { return Settings.Clone(); }

		public bool PutSettings(object o)
		{
			Settings = (TI83Settings)o;
			return false;
		}

		public object GetSyncSettings() { return null; }
		public bool PutSyncSettings(object o) { return false; }

		public Dictionary<string, int> GetCpuFlagsAndRegisters()
		{
			return new Dictionary<string, int>
			{
				{ "A", cpu.RegisterA },
				{ "AF", cpu.RegisterAF },
				{ "B", cpu.RegisterB },
				{ "BC", cpu.RegisterBC },
				{ "C", cpu.RegisterC },
				{ "D", cpu.RegisterD },
				{ "DE", cpu.RegisterDE },
				{ "E", cpu.RegisterE },
				{ "F", cpu.RegisterF },
				{ "H", cpu.RegisterH },
				{ "HL", cpu.RegisterHL },
				{ "I", cpu.RegisterI },
				{ "IX", cpu.RegisterIX },
				{ "IY", cpu.RegisterIY },
				{ "L", cpu.RegisterL },
				{ "PC", cpu.RegisterPC },
				{ "R", cpu.RegisterR },
				{ "Shadow AF", cpu.RegisterShadowAF },
				{ "Shadow BC", cpu.RegisterShadowBC },
				{ "Shadow DE", cpu.RegisterShadowDE },
				{ "Shadow HL", cpu.RegisterShadowHL },
				{ "SP", cpu.RegisterSP },
				{ "Flag C", cpu.RegisterF.Bit(0) ? 1 : 0 },
				{ "Flag N", cpu.RegisterF.Bit(1) ? 1 : 0 },
				{ "Flag P/V", cpu.RegisterF.Bit(2) ? 1 : 0 },
				{ "Flag 3rd", cpu.RegisterF.Bit(3) ? 1 : 0 },
				{ "Flag H", cpu.RegisterF.Bit(4) ? 1 : 0 },
				{ "Flag 5th", cpu.RegisterF.Bit(5) ? 1 : 0 },
				{ "Flag Z", cpu.RegisterF.Bit(6) ? 1 : 0 },
				{ "Flag S", cpu.RegisterF.Bit(7) ? 1 : 0 }
			};
		}

		public void SetCpuRegister(string register, int value)
		{
			switch (register)
			{
				default:
					throw new InvalidOperationException();
				case "A":
					cpu.RegisterA = (byte)value;
					break;
				case "AF":
					cpu.RegisterAF = (byte)value;
					break;
				case "B":
					cpu.RegisterB = (byte)value;
					break;
				case "BC":
					cpu.RegisterBC = (byte)value;
					break;
				case "C":
					cpu.RegisterC = (byte)value;
					break;
				case "D":
					cpu.RegisterD = (byte)value;
					break;
				case "DE":
					cpu.RegisterDE = (byte)value;
					break;
				case "E":
					cpu.RegisterE = (byte)value;
					break;
				case "F":
					cpu.RegisterF = (byte)value;
					break;
				case "H":
					cpu.RegisterH = (byte)value;
					break;
				case "HL":
					cpu.RegisterHL = (byte)value;
					break;
				case "I":
					cpu.RegisterI = (byte)value;
					break;
				case "IX":
					cpu.RegisterIX = (byte)value;
					break;
				case "IY":
					cpu.RegisterIY = (byte)value;
					break;
				case "L":
					cpu.RegisterL = (byte)value;
					break;
				case "PC":
					cpu.RegisterPC = (ushort)value;
					break;
				case "R":
					cpu.RegisterR = (byte)value;
					break;
				case "Shadow AF":
					cpu.RegisterShadowAF = (byte)value;
					break;
				case "Shadow BC":
					cpu.RegisterShadowBC = (byte)value;
					break;
				case "Shadow DE":
					cpu.RegisterShadowDE = (byte)value;
					break;
				case "Shadow HL":
					cpu.RegisterShadowHL = (byte)value;
					break;
				case "SP":
					cpu.RegisterSP = (byte)value;
					break;
			}
		}
	}
}