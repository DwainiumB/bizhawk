﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Computers.Commodore64
{
	public partial class Sid : ISoundProvider
	{
		private short[] sampleBuffer;
		private int sampleBufferCapacity;
		private int sampleBufferCount;
		private int sampleBufferIndex;
		private int sampleBufferReadIndex;
		private int sampleCounter;

		public void DiscardSamples()
		{
			sampleBuffer = new short[sampleBufferCapacity];
			ResetBuffer();
		}

		public short[] GetAllSamples()
		{
			if (sampleBufferCount > 0)
			{
				short[] samples = new short[sampleBufferCount];
				GetSamples(samples);
				return samples;
			}
			else
			{
				return new short[] { };
			}
		}

		public void GetSamples(short[] samples)
		{
			int count = samples.Length;
			int copied = 0;

			while (copied < count)
			{
				samples[copied] = sampleBuffer[sampleBufferReadIndex];
				if (sampleBufferIndex != sampleBufferReadIndex)
					sampleBufferReadIndex++;
				copied++;
				if (sampleBufferReadIndex == sampleBufferCapacity)
					sampleBufferReadIndex = 0;
			}

			// catch buffer up
			sampleBufferReadIndex = sampleBufferIndex;
			sampleBufferCount = 0;
		}

		private void InitSound(int initSampleRate)
		{
			sampleBufferCapacity = initSampleRate;
			DiscardSamples();
		}

		public int MaxVolume
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		private void ResetBuffer()
		{
			sampleBufferReadIndex = 0;
			sampleBufferIndex = 0;
		}

		private void SubmitSample()
		{
			if (sampleCounter == 0)
			{
				int mixer;

				mixer = voices[0].Output();
				mixer += voices[1].Output();
				if (!regs.D3 || !regs.FILT[2])
					mixer += voices[2].Output();

				// apply volume
				mixer *= regs.VOL;
				mixer >>= 4;

				// apply filter
				// (todo)

				// the mixer is very loud at this point, let's make it quieter
				mixer /= 4;

				if (mixer > 32767)
					mixer = 326767;
				else if (mixer < -32768)
					mixer = -32768;

				short output = (short)mixer;

				// run twice since the buffer expects stereo sound (I THINK)
				for (int i = 0; i < 2; i++)
				{
					sampleBufferIndex++;
					sampleBufferCount++;
					if (sampleBufferIndex == sampleBufferCapacity)
						sampleBufferIndex = 0;
					sampleBuffer[sampleBufferIndex] = output;
				}
				sampleCounter = cyclesPerSample;
			}
			sampleCounter--;
		}
	}

	public class SidSyncSoundProvider : ISyncSoundProvider
	{
		private Sid sid;

		public SidSyncSoundProvider(Sid source)
		{
			sid = source;
		}

		public void DiscardSamples()
		{
			sid.DiscardSamples();
		}

		public void GetSamples(out short[] samples, out int nsamp)
		{
			samples = sid.GetAllSamples();
			nsamp = samples.Length / 2;
		}
	}
}
