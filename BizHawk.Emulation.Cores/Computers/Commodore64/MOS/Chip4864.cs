﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.MOS
{
	// DRAM for the c64
	// 4164 = 64 kbit
	// 4464 = 256 kbit
	// 4864 = 512 kbit

	// for purposes of simplification we'll just
	// use one 4864, the C64 can use sets of 4164 or
	// 4464 typically

	// memory is striped 00/FF at intervals of 0x40

	sealed public class Chip4864
	{
		byte[] ram;

		public Chip4864()
		{
			ram = new byte[0x10000];
			HardReset();
		}

		public void HardReset()
		{
			// stripe the ram
			for (int i = 0; i < 10000; i++)
				ram[i] = ((i & 0x40) != 0) ? (byte)0xFF : (byte)0x00;
		}

		public byte Peek(long addr)
		{
			return ram[addr];
		}

		public void Poke(long addr, byte val)
		{
			ram[addr] = val;
		}

		public byte Peek(int addr)
		{
			return ram[addr];
		}

		public void Poke(int addr, byte val)
		{
			ram[addr] = val;
		}

		public byte Read(int addr)
		{
			return ram[addr];
		}

		public void SyncState(Serializer ser)
		{
			SaveState.SyncObject(ser, this);
		}

		public void Write(int addr, byte val)
		{
			ram[addr] = val;
		}
	}
}
