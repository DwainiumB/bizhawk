﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Computers.Commodore64
{

	// constants for the EnvelopeGenerator and calculation
	// methods are based from the libsidplayfp residfp library.

	public partial class EnvelopeGenerator
	{
		public enum EnvelopeState
		{
			Attack, Decay, Release
		}

		// value table for the envelope shift register
		static int[] adsrTable = new int[]
		{
			0x7F00, 0x0006, 0x003C, 0x0330,
			0x20C0, 0x6755, 0x3800, 0x500E,
			0x1212, 0x0222, 0x1848, 0x59B8,
			0x3840, 0x77E2, 0x7625, 0x0A93
		};

		int attack;
		int decay;
		bool gate;
		int release;
		int sustain;

		public byte envelopeCounter;
		public bool envelopeProcessEnabled;
		public int exponentialCounter;
		public int exponentialCounterPeriod;
		public bool freeze;
		public int lfsr;
		public int rate;
		public EnvelopeState state;

		public EnvelopeGenerator()
		{
			Reset();
		}

		public int Attack
		{
			get
			{
				return attack;
			}
			set
			{
				attack = value;
				if (state == EnvelopeState.Attack)
					rate = adsrTable[attack];
			}
		}

		public void Clock()
		{
			if (envelopeProcessEnabled)
			{
				envelopeProcessEnabled = false;
				envelopeCounter--;
				UpdateExponentialCounter();
			}

			if (lfsr != rate)
			{
				int feedback = ((lfsr >> 14) ^ (lfsr >> 13)) & 0x01;
				lfsr = ((lfsr << 1) & 0x7FFF) | feedback;
				return;
			}

			lfsr = 0x7FFF;

			if ((state == EnvelopeState.Attack) || (++exponentialCounter == exponentialCounterPeriod))
			{
				exponentialCounter = 0;
				if (!freeze)
				{
					switch (state)
					{
						case EnvelopeState.Attack:
							++envelopeCounter;
							if (envelopeCounter == 0xFF)
							{
								state = EnvelopeState.Decay;
								rate = adsrTable[decay];
							}
							break;
						case EnvelopeState.Decay:
							if (envelopeCounter == ((sustain << 4) | sustain))
							{
								return;
							}
							if (exponentialCounterPeriod != 1)
							{
								envelopeProcessEnabled = true;
								return;
							}
							envelopeCounter--;
							break;
						case EnvelopeState.Release:
							if (exponentialCounterPeriod != 1)
							{
								envelopeProcessEnabled = true;
								return;
							}
							envelopeCounter--;
							break;
					}

					UpdateExponentialCounter();
				}
			}
		}

		public int Decay
		{
			get
			{
				return decay;
			}
			set
			{
				decay = value;
				if (state == EnvelopeState.Decay)
					rate = adsrTable[decay];
			}
		}

		public bool Gate
		{
			get
			{
				return gate;
			}
			set
			{
				bool gateThis = value;

				if (!gate && gateThis)
				{
					state = EnvelopeState.Attack;
					rate = adsrTable[attack];
					freeze = false;
					envelopeProcessEnabled = false;
				}
				else if (gate && !gateThis)
				{
					state = EnvelopeState.Release;
					rate = adsrTable[release];
				}

				gate = gateThis;
			}
		}

		public short Output
		{
			get
			{
				return envelopeCounter;
			}
		}

		public int Release
		{
			get
			{
				return release;
			}
			set
			{
				release = value;
				if (state == EnvelopeState.Release)
					rate = adsrTable[release];
			}
		}

		public void Reset()
		{
			attack = 0;
			decay = 0;
			sustain = 0;
			release = 0;
			gate = false;

			envelopeCounter = 0;
			envelopeProcessEnabled = false;
			exponentialCounter = 0;
			exponentialCounterPeriod = 1;
			
			lfsr = 0x7FFF;
			state = EnvelopeState.Release;
			rate = adsrTable[release];
			freeze = true;
		}

		public void SetState(int stateAtk, int stateDcy, int stateSus, int stateRls, bool stateGate, EnvelopeState stateState)
		{
			attack = stateAtk;
			decay = stateDcy;
			sustain = stateSus;
			release = stateRls;
			gate = stateGate;
			state = stateState;
		}

		public int Sustain
		{
			get
			{
				return sustain;
			}
			set
			{
				sustain = value;
			}
		}

		private void UpdateExponentialCounter()
		{
			switch (envelopeCounter)
			{
				case 0x00:
					exponentialCounterPeriod = 1;
					freeze = true;
					break;
				case 0x06:
					exponentialCounterPeriod = 30;
					break;
				case 0x0E:
					exponentialCounterPeriod = 16;
					break;
				case 0x1A:
					exponentialCounterPeriod = 8;
					break;
				case 0x36:
					exponentialCounterPeriod = 4;
					break;
				case 0x5D:
					exponentialCounterPeriod = 2;
					break;
				case 0xFF:
					exponentialCounterPeriod = 1;
					break;
			}
		}
	}
}
