﻿using BizHawk.Emulation.Cores.Computers.Commodore64.MOS;

namespace BizHawk.Emulation.Cores.Computers.Commodore64
{
	public static class PRG
	{
		static public void Load(MOSPLA pla, byte[] prgFile)
		{
			int length = prgFile.Length;
			if (length > 2)
			{
				int addr = (prgFile[0] | (prgFile[1] << 8));
				int offset = 2;
				unchecked
				{
					while (offset < length)
					{
						pla.Write(addr, prgFile[offset]);
						offset++;
						addr++;
					}
				}
			}
		}
	}
}
