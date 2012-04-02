﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Consoles.Atari._2600
{
	/*
	UA (UA Ltd)
	-----

	This one was found out later on, lurking on a proto of Pleaides.  It works with 8K of ROM
	and banks it in 4K at a time.

	Accessing 0220 will select the first bank, and accessing 0240 will select the second.
	*/

	class mUA : MapperBase 
	{
		int toggle = 0;

		public override byte ReadMemory(ushort addr)
		{
			Address(addr);
			if (addr < 0x1000) return base.ReadMemory(addr);
			return core.rom[toggle * 4 * 1024 + (addr & 0xFFF)];
		}
		public override void WriteMemory(ushort addr, byte value)
		{
			Address(addr);
			if (addr < 0x1000) base.WriteMemory(addr, value);
		}

		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
			ser.Sync("toggle", ref toggle);
		}

		void Address(ushort addr)
		{
			if (addr == 0x0220) toggle = 0;
			else if (addr == 0x0240) toggle = 1;
		}
	}
}
