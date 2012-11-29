﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Computers.Commodore64.Cartridges
{
	public class Mapper0005 : Cartridge
	{
		private byte[][] banksA = new byte[0][]; //8000
		private byte[][] banksB = new byte[0][]; //A000
		private uint bankMask;
		private uint bankNumber;
		private byte[] currentBankA;
		private byte[] currentBankB;
		private byte[] dummyBank;

		public Mapper0005(List<uint> newAddresses, List<uint> newBanks, List<byte[]> newData)
		{
			uint count = (uint)newAddresses.Count;

			// build dummy bank
			dummyBank = new byte[0x2000];
			for (uint i = 0; i < 0x2000; i++)
				dummyBank[i] = 0xFF; // todo: determine if this is correct

			if (count == 64) //512k
			{
				pinGame = true;
				pinExRom = false;
				bankMask = 0x3F;
				banksA = new byte[64][];
			}
			else if (count == 32) //256k
			{
				pinGame = false;
				pinExRom = false;
				bankMask = 0x0F;
				banksA = new byte[16][];
				banksB = new byte[16][];
			}
			else if (count == 16) //128k
			{
				pinGame = true;
				pinExRom = false;
				bankMask = 0x0F;
				banksA = new byte[16][];
			}
			else if (count == 4) //32k
			{
				pinGame = true;
				pinExRom = false;
				bankMask = 0x03;
				banksA = new byte[4][];
			}
			else
			{
				// we don't know what format this is...
				throw new Exception("This looks like an Ocean cartridge but cannot be loaded...");
			}

			// for safety, initialize all banks to dummy
			for (uint i = 0; i < banksA.Length; i++)
				banksA[i] = dummyBank;
			for (uint i = 0; i < banksB.Length; i++)
				banksB[i] = dummyBank;

			// now load in the banks
			for (int i = 0; i < count; i++)
			{
				if (newAddresses[i] == 0x8000)
				{
					banksA[newBanks[i]] = newData[i];
				}
				else if (newAddresses[i] == 0xA000)
				{
					banksB[newBanks[i]] = newData[i];
				}
			}

			BankSet(0);
		}

		private void BankSet(uint index)
		{
			bankNumber = index & bankMask;
			if (!pinExRom)
				currentBankA = banksA[bankNumber];
			else
				currentBankA = dummyBank;

			if (!pinGame)
				currentBankB = banksB[bankNumber];
			else
				currentBankB = dummyBank;
		}

		public override byte Peek8000(int addr)
		{
			return currentBankA[addr];
		}

		public override byte PeekA000(int addr)
		{
			return currentBankB[addr];
		}

		public override void PokeDE00(int addr, byte val)
		{
			if (addr == 0x00)
				BankSet(val);
		}

		public override byte Read8000(ushort addr)
		{
			return currentBankA[addr];
		}

		public override byte ReadA000(ushort addr)
		{
			return currentBankB[addr];
		}

		public override void WriteDE00(ushort addr, byte val)
		{
			if (addr == 0x00)
				BankSet(val);
		}
	}
}
