﻿using System;

namespace BizHawk.Emulation.Computers.Commodore64.MOS
{
	// emulates the PLA
	// which handles all bank switching

	public class MOSPLA
	{
		// ------------------------------------

		public Func<int, byte> PeekBasicRom;
		public Func<int, byte> PeekCartridgeLo;
		public Func<int, byte> PeekCartridgeHi;
		public Func<int, byte> PeekCharRom;
		public Func<int, byte> PeekCia0;
		public Func<int, byte> PeekCia1;
		public Func<int, byte> PeekColorRam;
		public Func<int, byte> PeekExpansionLo;
		public Func<int, byte> PeekExpansionHi;
		public Func<int, byte> PeekKernalRom;
		public Func<int, byte> PeekMemory;
		public Func<int, byte> PeekSid;
		public Func<int, byte> PeekVic;
		public Action<int, byte> PokeCartridgeLo;
		public Action<int, byte> PokeCartridgeHi;
		public Action<int, byte> PokeCia0;
		public Action<int, byte> PokeCia1;
		public Action<int, byte> PokeColorRam;
		public Action<int, byte> PokeExpansionLo;
		public Action<int, byte> PokeExpansionHi;
		public Action<int, byte> PokeMemory;
		public Action<int, byte> PokeSid;
		public Action<int, byte> PokeVic;
        public Func<bool> ReadAEC;
        public Func<bool> ReadBA;
		public Func<int, byte> ReadBasicRom;
		public Func<int, byte> ReadCartridgeLo;
		public Func<int, byte> ReadCartridgeHi;
		public Func<bool> ReadCharen;
		public Func<int, byte> ReadCharRom;
		public Func<int, byte> ReadCia0;
		public Func<int, byte> ReadCia1;
		public Func<int, byte> ReadColorRam;
		public Func<int, byte> ReadExpansionLo;
		public Func<int, byte> ReadExpansionHi;
		public Func<bool> ReadExRom;
		public Func<bool> ReadGame;
		public Func<bool> ReadHiRam;
		public Func<int, byte> ReadKernalRom;
		public Func<bool> ReadLoRam;
		public Func<int, byte> ReadMemory;
		public Func<int, byte> ReadSid;
		public Func<int, byte> ReadVic;
		public Action<int, byte> WriteCartridgeLo;
		public Action<int, byte> WriteCartridgeHi;
		public Action<int, byte> WriteCia0;
		public Action<int, byte> WriteCia1;
		public Action<int, byte> WriteColorRam;
		public Action<int, byte> WriteExpansionLo;
		public Action<int, byte> WriteExpansionHi;
		public Action<int, byte> WriteMemory;
		public Action<int, byte> WriteSid;
		public Action<int, byte> WriteVic;
	
		// ------------------------------------

        protected enum PLABank
		{
			None,
			RAM,
			BasicROM,
			KernalROM,
			CharROM,
			IO,
			CartridgeLo,
			CartridgeHi,
			Vic,
			Sid,
			ColorRam,
			Cia0,
			Cia1,
			Expansion0,
			Expansion1
		}

		// ------------------------------------

        protected bool p0;
        protected bool p1;
        protected bool p2;
        protected bool p3;
        protected bool p4;
        protected bool p5;
        protected bool p9;
        protected bool p11;
        protected bool p13;
        protected bool p15;
        protected bool p17;
        protected bool p19;
        protected bool p20;
        protected bool p21;
        protected bool p22;
        protected bool p24;
        protected bool p25;
        protected bool p26;
        protected bool p27;
        protected bool p28;
        protected bool loram;
        protected bool hiram;
        protected bool game;
        protected bool exrom;
        protected bool charen;
        protected bool a15;
        protected bool a14;
        protected bool a13;
        protected bool a12;

		private PLABank Bank(int addr, bool read)
		{
            loram = ReadLoRam();
            hiram = ReadHiRam();
            game = ReadGame();

            a15 = (addr & 0x08000) != 0;
            a14 = (addr & 0x04000) != 0;
            a13 = (addr & 0x02000) != 0;
            a12 = (addr & 0x01000) != 0;
            #region OLDPLA
            //// io/character access
            //if (a15 && a14 && !a13 && a12)
            //{
            //    // character rom, banked in at D000-DFFF
            //    charen = ReadCharen();
            //    if (read && !charen)
            //    {
            //        p3 = hiram && game;
            //        p4 = loram && game;
            //        p5 = hiram && !exrom && !game;
            //        if (p3 || p4 || p5)
            //            return PLABank.CharROM;
            //    }

            //    // io block, banked in at D000-DFFF
            //    p9 = hiram && charen && game;
            //    p11 = loram && charen && game;
            //    p13 = hiram && charen && !exrom && !game;
            //    p15 = loram && charen && !exrom && !game;
            //    p17 = exrom && !game;
            //    if (p9 || p11 || p13 || p15 || p17)
            //    {
            //        if (addr < 0xD400)
            //            return PLABank.Vic;
            //        if (addr < 0xD800)
            //            return PLABank.Sid;
            //        if (addr < 0xDC00)
            //            return PLABank.ColorRam;
            //        if (addr < 0xDD00)
            //            return PLABank.Cia0;
            //        if (addr < 0xDE00)
            //            return PLABank.Cia1;
            //        if (addr < 0xDF00)
            //            return PLABank.Expansion0;
            //        return PLABank.Expansion1;
            //    }
            //}

            //// basic rom, banked at A000-BFFF
            //p0 = loram && hiram && a15 && !a14 && a13 && read && game;
            //if (p0)
            //    return PLABank.BasicROM;

            //// kernal rom, banked at E000-FFFF
            //exrom = ReadExRom();
            //if (hiram && a15 && a14 && a13 && read)
            //{
            //    p1 = game;
            //    p2 = !exrom && !game;
            //    if (p1 || p2)
            //        return PLABank.KernalROM;
            //}

            //// cartridge low, banked at 8000-9FFF
            //if (a15 && !a14 && !a13)
            //{
            //    p19 = loram && hiram && read && !exrom;
            //    p20 = exrom && !game;
            //    if (p19 || p20)
            //        return PLABank.CartridgeLo;
            //}

            //// cartridge high, banked either at A000-BFFF or E000-FFFF depending
            //if (a15 && a13 && !game)
            //{
            //    p21 = hiram && !a14 && read && !exrom;
            //    p22 = a14 && exrom;
            //    if (p21 || p22)
            //        return PLABank.CartridgeHi;
            //}

            //// ultimax mode ram exclusion
            //if (exrom && !game)
            //{
            //    p24 = !a15 && !a14 && a12;
            //    p25 = !a15 && !a14 && a13;
            //    p26 = !a15 && a14;
            //    p27 = a15 && !a14 && a13;
            //    p28 = a15 && a14 && !a13 && !a12;
            //    if (!(p24 || p25 || p26 || p27 || p28))
            //        return PLABank.RAM;
            //}
            //else
            //{
            //    return PLABank.RAM;
            //}

            //return PLABank.None;
            #endregion

            // upper memory regions 8000-FFFF
            exrom = ReadExRom();
            if (a15)
            {
                // io/character access
                if (a14 && !a13 && a12)
                {
                    // character rom, banked in at D000-DFFF
                    charen = ReadCharen();
                    if (read && !charen && (((hiram || loram) && game) || (hiram && !exrom && !game)))
                        return PLABank.CharROM;

                    // io block, banked in at D000-DFFF
                    if ((charen && (hiram || loram)) || (exrom && !game))
                    {
                        if (addr < 0xD400)
                            return PLABank.Vic;
                        if (addr < 0xD800)
                            return PLABank.Sid;
                        if (addr < 0xDC00)
                            return PLABank.ColorRam;
                        if (addr < 0xDD00)
                            return PLABank.Cia0;
                        if (addr < 0xDE00)
                            return PLABank.Cia1;
                        if (addr < 0xDF00)
                            return PLABank.Expansion0;
                        return PLABank.Expansion1;
                    }
                }

                // cartridge high, banked either at A000-BFFF or E000-FFFF depending
                if (a13 && !game && ((hiram && !a14 && read && !exrom) || (a14 && exrom)))
                    return PLABank.CartridgeHi;

                // cartridge low, banked at 8000-9FFF
                if (!a14 && !a13 && ((loram && hiram && read && !exrom) || (exrom && !game)))
                    return PLABank.CartridgeLo;

                // kernal rom, banked at E000-FFFF
                if (hiram && a14 && a13 && read && (game || (!exrom && !game)))
                    return PLABank.KernalROM;

                // basic rom, banked at A000-BFFF
                if (loram && hiram && !a14 && a13 && read && game)
                    return PLABank.BasicROM;
            }

            // ultimax mode ram exclusion
            if (exrom && !game)
            {
                p24 = !a15 && !a14 && a12;        //00x1 1000-1FFF, 3000-3FFF
                p25 = !a15 && !a14 && a13;        //001x 2000-3FFF
                p26 = !a15 && a14;                //01xx 4000-7FFF
                p27 = a15 && !a14 && a13;         //101x A000-BFFF
                p28 = a15 && a14 && !a13 && !a12; //1100 C000-CFFF
                if (p24 || p25 || p26 || p27 || p28)
                    return PLABank.None;
            }

            return PLABank.RAM;
        }

		public byte Peek(int addr)
		{
			switch (Bank(addr, true))
			{
				case PLABank.BasicROM:
					return PeekBasicRom(addr);
				case PLABank.CartridgeHi:
					return PeekCartridgeHi(addr);
				case PLABank.CartridgeLo:
					return PeekCartridgeLo(addr);
				case PLABank.CharROM:
					return PeekCharRom(addr);
				case PLABank.Cia0:
					return PeekCia0(addr);
				case PLABank.Cia1:
					return PeekCia1(addr);
				case PLABank.ColorRam:
					return PeekColorRam(addr);
				case PLABank.Expansion0:
					return PeekExpansionLo(addr);
				case PLABank.Expansion1:
					return PeekExpansionHi(addr);
				case PLABank.KernalROM:
					return PeekKernalRom(addr);
				case PLABank.RAM:
					return PeekMemory(addr);
				case PLABank.Sid:
					return PeekSid(addr);
				case PLABank.Vic:
					return PeekVic(addr);
			}
			return 0xFF;
		}

		public void Poke(int addr, byte val)
		{
			switch (Bank(addr, false))
			{
				case PLABank.CartridgeHi:
					PokeCartridgeHi(addr, val);
					break;
				case PLABank.CartridgeLo:
					PokeCartridgeLo(addr, val);
					break;
				case PLABank.Cia0:
					PokeCia0(addr, val);
					break;
				case PLABank.Cia1:
					PokeCia1(addr, val);
					break;
				case PLABank.ColorRam:
					PokeColorRam(addr, val);
					break;
				case PLABank.Expansion0:
					PokeExpansionLo(addr, val);
					break;
				case PLABank.Expansion1:
					PokeExpansionHi(addr, val);
					break;
				case PLABank.RAM:
					PokeMemory(addr, val);
					break;
				case PLABank.Sid:
					PokeSid(addr, val);
					break;
				case PLABank.Vic:
					PokeVic(addr, val);
					break;
			}
		}

		public byte Read(int addr)
		{
			switch (Bank(addr, true))
			{
				case PLABank.BasicROM:
					return ReadBasicRom(addr);
				case PLABank.CartridgeHi:
					return ReadCartridgeHi(addr);
				case PLABank.CartridgeLo:
					return ReadCartridgeLo(addr);
				case PLABank.CharROM:
					return ReadCharRom(addr);
				case PLABank.Cia0:
					return ReadCia0(addr);
				case PLABank.Cia1:
					return ReadCia1(addr);
				case PLABank.ColorRam:
					return ReadColorRam(addr);
				case PLABank.Expansion0:
					return ReadExpansionLo(addr);
				case PLABank.Expansion1:
					return ReadExpansionHi(addr);
				case PLABank.KernalROM:
					return ReadKernalRom(addr);
				case PLABank.RAM:
					return ReadMemory(addr);
				case PLABank.Sid:
					return ReadSid(addr);
				case PLABank.Vic:
					return ReadVic(addr);
			}
			return 0xFF;
		}

        public void SyncState(Serializer ser)
        {
            Sync.SyncObject(ser, this);
        }

        public byte VicRead(int addr)
        {
            game = ReadGame();
            exrom = ReadExRom();
            a14 = (addr & 0x04000) == 0;
            a13 = (addr & 0x02000) != 0;
            a12 = (addr & 0x01000) != 0;

            // read char rom at 1000-1FFF and 9000-9FFF
            if (a14 && !a13 && a12 && (game || !exrom))
                return ReadCharRom(addr);

            // read cartridge rom in ultimax mode
            if (a13 && a12 && exrom && !game)
                return ReadCartridgeHi(addr);

            return ReadMemory(addr);
        }

        public void Write(int addr, byte val)
		{
			switch (Bank(addr, false))
			{
				case PLABank.CartridgeHi:
					WriteCartridgeHi(addr, val);
			        WriteMemory(addr, val);
					break;
				case PLABank.CartridgeLo:
					WriteCartridgeLo(addr, val);
			        WriteMemory(addr, val);
					break;
				case PLABank.Cia0:
					WriteCia0(addr, val);
					break;
				case PLABank.Cia1:
					WriteCia1(addr, val);
					break;
				case PLABank.ColorRam:
					WriteColorRam(addr, val);
					break;
				case PLABank.Expansion0:
					WriteExpansionLo(addr, val);
					return;
				case PLABank.Expansion1:
					WriteExpansionHi(addr, val);
                    return;
				case PLABank.RAM:
			        WriteMemory(addr, val);
					break;
				case PLABank.Sid:
					WriteSid(addr, val);
					break;
				case PLABank.Vic:
					WriteVic(addr, val);
					break;
			}
		}
	}
}
