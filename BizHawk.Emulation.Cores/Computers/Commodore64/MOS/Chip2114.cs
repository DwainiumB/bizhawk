﻿using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.MOS
{
	// used as Color RAM in C64

	sealed public class Chip2114
	{
		byte[] ram;

		public Chip2114()
		{
			HardReset();
		}

		public void HardReset()
		{
			ram = new byte[0x400];
		}

		public byte Peek(int addr)
		{
			return ram[addr & 0x3FF];
		}

		public void Poke(int addr, byte val)
		{
			ram[addr & 0x3FF] = (byte)(val & 0xF);
		}

		public byte Read(int addr)
		{
			return ram[addr & 0x3FF];
		}

		public int ReadInt(int addr)
		{
			return ram[addr & 0x3FF];
		}

		public void SyncState(Serializer ser)
		{
			SaveState.SyncObject(ser, this);
		}

		public void Write(int addr, byte val)
		{
			ram[addr & 0x3FF] = (byte)(val & 0xF);
		}
	}
}
