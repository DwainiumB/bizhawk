﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BizHawk.Common;
using BizHawk.Emulation.Cores.Components.M6502;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.MOS
{
	// an extension of the 6502 processor

	sealed public class MOS6510
	{
		// ------------------------------------

		MOS6502X cpu;
        int lagCycles;
		bool pinNMILast;
		LatchedPort port;
		bool thisNMI;

		public Func<int, byte> PeekMemory;
		public Action<int, byte> PokeMemory;
		public Func<bool> ReadAEC;
		public Func<bool> ReadIRQ;
		public Func<bool> ReadNMI;
		public Func<bool> ReadRDY;
		public Func<int, byte> ReadMemory;
		public Func<byte> ReadPort;
		public Action<int, byte> WriteMemory;
		public Action<int, byte> WriteMemoryPort;

		// ------------------------------------

		public MOS6510()
		{
			cpu = new MOS6502X();

			// configure cpu r/w
			cpu.DummyReadMemory = Read;
			cpu.ReadMemory = Read;
			cpu.WriteMemory = Write;

			// perform hard reset
			HardReset();
		}

		public void HardReset()
		{
			// configure CPU defaults
			cpu.Reset();
			cpu.FlagI = true;
			cpu.BCD_Enabled = true;
			if (ReadMemory != null)
				cpu.PC = (ushort)(ReadMemory(0x0FFFC) | (ReadMemory(0x0FFFD) << 8));

			// configure data port defaults
			port = new LatchedPort();
			port.Direction = 0x00;
			port.Latch = 0xFF;

			// NMI is high on startup (todo: verify)
			pinNMILast = true;
		}

		// ------------------------------------

		public void ExecutePhase1()
		{
			cpu.IRQ = !ReadIRQ();
		}

		public void ExecutePhase2()
		{
			cpu.RDY = ReadRDY();

			// the 6502 core expects active high
			// so we reverse the polarity here
			thisNMI = ReadNMI();
			if (!thisNMI && pinNMILast)
				cpu.NMI = true;

            if (ReadAEC())
            {
                cpu.ExecuteOne();
                pinNMILast = thisNMI;
            }
            else
            {
                lagCycles++;
            }
		}

        public int LagCycles
        {
            get
            {
                return lagCycles;
            }
            set
            {
                lagCycles = value;
            }
        }

		// ------------------------------------

		public ushort PC
		{
			get
			{
				return cpu.PC;
			}
			set
			{
				cpu.PC = value;
			}
		}

		public byte A
		{
			get { return cpu.A; } set { cpu.A = value; }
		}

		public byte X
		{
			get { return cpu.X; } set { cpu.X = value; }
		}

		public byte Y
		{
			get { return cpu.Y; } set { cpu.Y = value; }
		}

		public byte S
		{
			get { return cpu.S; } set { cpu.S = value; }
		}

		public bool FlagC { get { return cpu.FlagC; } }
		public bool FlagZ { get { return cpu.FlagZ; } }
		public bool FlagI { get { return cpu.FlagI; } }
		public bool FlagD { get { return cpu.FlagD; } }
		public bool FlagB { get { return cpu.FlagB; } }
		public bool FlagV { get { return cpu.FlagV; } }
		public bool FlagN { get { return cpu.FlagN; } }
		public bool FlagT { get { return cpu.FlagT; } }

		public byte Peek(long addr)
		{
			if (addr == 0x0000)
				return port.Direction;
			else if (addr == 0x0001)
				return PortData;
			else
				return PeekMemory((int)addr);
		}

		public void Poke(long addr, byte val)
		{
			if (addr == 0x0000)
				port.Direction = val;
			else if (addr == 0x0001)
				port.Latch = val;
			else
				PokeMemory((int)addr, val);
		}

		public byte PortData
		{
			get
			{
				return port.ReadInput(ReadPort());
			}
			set
			{
				port.Latch = value;
			}
		}

		public byte Read(ushort addr)
		{
			if (addr == 0x0000)
				return port.Direction;
			else if (addr == 0x0001)
				return PortData;
			else
				return ReadMemory(addr);
		}

		public void SyncState(Serializer ser)
		{
			cpu.SyncState(ser);
			SaveState.SyncObject(ser, this);
		}

		public void Write(ushort addr, byte val)
		{
			if (addr == 0x0000)
			{
				port.Direction = val;
				WriteMemoryPort(addr, val);
			}
			else if (addr == 0x0001)
			{
				port.Latch = val;
				WriteMemoryPort(addr, val);
			}
			else
			{
				WriteMemory(addr, val);
			}
		}
	}
}
