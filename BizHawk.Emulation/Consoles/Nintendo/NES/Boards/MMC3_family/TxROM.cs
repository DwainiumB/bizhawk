﻿using System;
using System.IO;
using System.Diagnostics;

//http://wiki.nesdev.com/w/index.php/TxROM
//read some background info on namco 108 and DxROM boards here

namespace BizHawk.Emulation.Consoles.Nintendo
{
	public class TxROM : MMC3Board_Base
	{
		public override void WritePRG(int addr, byte value)
		{
			base.WritePRG(addr, value);
			SetMirrorType(mmc3.mirror);  //often redundant, but gets the job done
		}
	
		public override byte[] SaveRam
		{
			get
			{
				if (!Cart.wram_battery) return null;
				return WRAM;
				//some boards have a wram that is backed-up or not backed-up. need to handle that somehow
				//(nestopia splits it into NVWRAM and WRAM but i didnt like that at first.. but it may player better with this architecture)
			}
		}

		public override byte ReadWRAM(int addr)
		{
			if (Cart.wram_size != 0)
				return WRAM[addr & wram_mask];
			else return 0xFF;
		}

		public override void WriteWRAM(int addr, byte value)
		{
			if (Cart.wram_size != 0)
				WRAM[addr & wram_mask] = value;
		}


		public override void SyncState(Serializer ser)
		{
			base.SyncState(ser);
		}

		public override bool Configure(NES.EDetectionOrigin origin)
		{
			//analyze board type
			switch (Cart.board_type)
			{
				case "TXROM-HOMEBREW":
					break;
				case "NES-TBROM": //tecmo world cup soccer (DE) [untested]
					AssertPrg(64); AssertChr(64); AssertVram(0); AssertWram(0);
					AssertBattery(false);
					break;
				case "NES-TEROM": //Adv of lolo 2
					AssertPrg(32); AssertChr(32); AssertVram(0); AssertWram(0);
					AssertBattery(false);
					break;
				case "NES-TFROM": //legacy of the wizard
					AssertPrg(128); AssertChr(32, 64); AssertVram(0); AssertWram(0);
					AssertBattery(false);
					break;
				case "NES-TGROM": //mega man 4 & 6
					AssertPrg(128, 256, 512); AssertChr(0); AssertVram(8); AssertWram(0);
					AssertBattery(false);
					break;
				case "NES-TKROM": //kirby's adventure
					AssertPrg(128, 256, 512); AssertChr(128, 256); AssertVram(0); AssertWram(8);
					break;
				case "NES-TLROM": //mega man 3
					AssertPrg(128, 256, 512); AssertChr(128, 256); AssertVram(0); AssertWram(0);
					AssertBattery(false);
					break;
				case "NES-TL1ROM": //Double dragon 2
					AssertPrg(128); AssertChr(128); AssertVram(0); AssertWram(0);
					AssertBattery(false);
					break;
				case "NES-TL2ROM": //batman (U) ?
					AssertPrg(128); AssertChr(128); AssertVram(0); AssertWram(0);
					AssertBattery(false);
					break;
				case "NES-TSROM": //super mario bros. 3 (U)
					AssertPrg(128, 256, 512); AssertChr(128, 256); AssertVram(0); AssertWram(8);
					AssertBattery(false);
					break;
				case "NES-B4": //batman (U)
					AssertPrg(128); AssertChr(128); AssertVram(0); AssertWram(0);
					AssertBattery(false);
					break;
				default:
					return false;
			}

			BaseSetup();

			return true;
		}

	}


}
