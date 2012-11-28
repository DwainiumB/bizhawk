﻿using BizHawk.Emulation.CPUs.M6502;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Computers.Commodore64.MOS
{
	// an extension of the 6502 processor

	public class MOS6510 : IStandardIO
	{
		// ------------------------------------

		private C64Chips chips;
		private MOS6502X cpu;
		private bool freezeCpu;
		private bool pinAEC;
		private bool pinCassetteButton;
		private bool pinCassetteMotor;
		private bool pinCassetteOutput;
		private bool pinCharen;
		private bool pinIRQ;
		private bool pinLoram;
		private bool pinHiram;
		private bool pinNMI;
		private bool pinRDY;
		private byte portDir;
		private bool unusedPin0;
		private bool unusedPin1;
		private uint unusedPinTTL0;
		private uint unusedPinTTL1;
		private uint unusedPinTTLCycles;

		// ------------------------------------

		public MOS6510(C64Chips newChips)
		{
			chips = newChips;
			cpu = new MOS6502X();

			// configure cpu r/w
			cpu.DummyReadMemory = Read;
			cpu.ReadMemory = Read;
			cpu.WriteMemory = Write;

			// configure data port defaults
			portDir = 0x2F;
			SetPortData(0x37);

			// todo: verify this value (I only know that unconnected bits fade after a number of cycles)
			unusedPinTTLCycles = 40;
			unusedPinTTL0 = 0;
			unusedPinTTL1 = 0;
		}

		public void HardReset()
		{
			cpu.Reset();
			cpu.FlagI = true;
			cpu.BCD_Enabled = true;
			cpu.PC = (ushort)(chips.pla.Read(0xFFFC) | (chips.pla.Read(0xFFFD) << 8));
		}

		// ------------------------------------

		public void ExecutePhase1()
		{
			UpdatePins();
		}

		public void ExecutePhase2()
		{
			UpdatePins();

			if (pinAEC && !freezeCpu)
			{
				// the 6502 core expects active high
				// so we reverse the polarity here
				cpu.NMI = !pinNMI;
				cpu.IRQ = !pinIRQ;
				cpu.ExecuteOne();
				chips.ram.Write(0x0FFE, (byte)(cpu.PC & 0xFF));
				chips.ram.Write(0x0FFF, (byte)((cpu.PC >> 8) & 0xFF));
			}

			// process unused pin TTL
			if (unusedPinTTL0 == 0)
				unusedPin0 = false;
			else
				unusedPinTTL0--;

			if (unusedPinTTL1 == 0)
				unusedPin1 = false;
			else
				unusedPinTTL1--;
		}

		private void UpdatePins()
		{
			pinAEC = chips.vic.AEC;
			pinIRQ = chips.vic.IRQ && chips.cia0.IRQ;
			pinNMI = chips.cia1.IRQ;
			pinRDY = chips.vic.BA;

			if (pinRDY)
				freezeCpu = false;
		}

		// ------------------------------------

		public byte Peek(int addr)
		{
			if (addr == 0x0000)
				return PortDirection;
			else if (addr == 0x0001)
				return PortData;
			else
				return chips.pla.Peek(addr);
		}

		public void Poke(int addr, byte val)
		{
			if (addr == 0x0000)
				portDir = val;
			else if (addr == 0x0001)
				SetPortData(val);
			chips.pla.Poke(addr, val);
		}

		public byte Read(ushort addr)
		{
			// cpu freezes after first read when RDY is low
			if (!pinRDY)
				freezeCpu = true;

			if (addr == 0x0000)
				return PortDirection;
			else if (addr == 0x0001)
				return PortData;
			else
				return chips.pla.Read(addr);
		}

		public void Write(ushort addr, byte val)
		{
			if (addr == 0x0000)
				PortDirection = val;
			else if (addr == 0x0001)
				PortData = val;
			chips.pla.Write(addr, val);
		}

		// ------------------------------------

		public bool AEC
		{
			get { return pinAEC; }
		}

		public bool IRQ
		{
			get { return pinIRQ; }
		}

		public bool NMI
		{
			get { return pinNMI; }
		}

		public bool RDY
		{
			get { return pinRDY; }
		}

		public byte PortData
		{
			get
			{
				byte result = 0x00;

				result |= pinLoram ? (byte)0x01 : (byte)0x00;
				result |= pinHiram ? (byte)0x02 : (byte)0x00;
				result |= pinCharen ? (byte)0x04 : (byte)0x00;
				result |= pinCassetteOutput ? (byte)0x08 : (byte)0x00;
				result |= pinCassetteButton ? (byte)0x10 : (byte)0x00;
				result |= pinCassetteMotor ? (byte)0x20 : (byte)0x00;
				result |= unusedPin0 ? (byte)0x40 : (byte)0x00;
				result |= unusedPin1 ? (byte)0x80 : (byte)0x00;
				return result;
			}
			set
			{
				byte val = Port.CPUWrite(PortData, value, portDir);
				SetPortData(val);
			}
		}

		public byte PortDirection
		{
			get { return portDir; }
			set { portDir = value; }
		}

		private void SetPortData(byte val)
		{
			pinCassetteOutput = ((val & 0x08) != 0);
			pinCassetteButton = ((val & 0x10) != 0);
			pinCassetteMotor = ((val & 0x20) != 0);

			if (!chips.pla.UltimaxMode)
			{
				pinLoram = ((val & 0x01) != 0);
				pinHiram = ((val & 0x02) != 0);
				pinCharen = ((val & 0x04) != 0);
				chips.pla.LoRam = pinLoram;
				chips.pla.HiRam = pinHiram;
				chips.pla.Charen = pinCharen;
			}

			unusedPin0 = ((val & 0x40) != 0);
			unusedPin1 = ((val & 0x80) != 0);
			unusedPinTTL0 = unusedPinTTLCycles;
			unusedPinTTL1 = unusedPinTTLCycles;
		}

		// ------------------------------------
	}
}
