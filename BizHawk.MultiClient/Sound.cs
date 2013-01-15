﻿using System;
using BizHawk.Emulation.Sound;
#if WINDOWS
using SlimDX.DirectSound;
using SlimDX.Multimedia;
#else
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
#endif

using BizHawk.Emulation.Consoles.Nintendo;

namespace BizHawk.MultiClient
{
#if WINDOWS
	public class Sound : IDisposable
	{
		private bool Muted = false;
		private bool disposed = false;

		private SecondarySoundBuffer DSoundBuffer;
		private byte[] SoundBuffer;
		private const int BufferSize = 4410 * 2 * 2; // 1/10th of a second, 2 bytes per sample, 2 channels;
		//private int SoundBufferPosition; //TODO: use this
		public bool needDiscard;

		private BufferedAsync semisync = new BufferedAsync();

		private ISoundProvider asyncsoundProvider;
		private ISyncSoundProvider syncsoundProvider;

		public Sound(IntPtr handle, DirectSound device)
		{
			if (device != null)
			{
				device.SetCooperativeLevel(handle, CooperativeLevel.Priority);

				var format = new WaveFormat();
				format.SamplesPerSecond = 44100;
				format.BitsPerSample = 16;
				format.Channels = 2;
				format.FormatTag = WaveFormatTag.Pcm;
				format.BlockAlignment = 4;
				format.AverageBytesPerSecond = format.SamplesPerSecond * format.Channels * (format.BitsPerSample / 8);

				var desc = new SoundBufferDescription();
				desc.Format = format;
				desc.Flags = BufferFlags.GlobalFocus | BufferFlags.Software | BufferFlags.GetCurrentPosition2 | BufferFlags.ControlVolume;
				desc.SizeInBytes = BufferSize;
				DSoundBuffer = new SecondarySoundBuffer(device, desc);
				ChangeVolume(Global.Config.SoundVolume);
			}
			SoundBuffer = new byte[BufferSize];

			disposed = false;
		}

		public void StartSound()
		{
			if (disposed) throw new ObjectDisposedException("Sound");
			if (Global.Config.SoundEnabled == false) return;
			if (DSoundBuffer == null) return;
			if (IsPlaying)
				return;

			needDiscard = true;

			DSoundBuffer.Write(SoundBuffer, 0, LockFlags.EntireBuffer);
			DSoundBuffer.CurrentPlayPosition = 0;
			DSoundBuffer.Play(0, PlayFlags.Looping);
		}

		bool IsPlaying
		{
			get
			{
				if (DSoundBuffer == null) return false;
				if ((DSoundBuffer.Status & BufferStatus.Playing) != 0) return true;
				return false;
			}
		}

		public void StopSound()
		{
			if (!IsPlaying)
				return;
			for (int i = 0; i < SoundBuffer.Length; i++)
				SoundBuffer[i] = 0;
			DSoundBuffer.Write(SoundBuffer, 0, LockFlags.EntireBuffer);
			DSoundBuffer.Stop();
		}

		public void Dispose()
		{
			if (disposed) return;
			if (DSoundBuffer != null && DSoundBuffer.Disposed == false)
			{
				DSoundBuffer.Dispose();
				DSoundBuffer = null;
			}
		}

		public void SetSyncInputPin(ISyncSoundProvider source)
		{
			syncsoundProvider = source;
			asyncsoundProvider = null;
			semisync.DiscardSamples();
		}

		public void SetAsyncInputPin(ISoundProvider source)
		{
			syncsoundProvider = null;
			asyncsoundProvider = source;
			semisync.BaseSoundProvider = source;
			semisync.RecalculateMagic(Global.CoreComm.VsyncRate);
		}

		static int circularDist(int from, int to, int size)
		{
			if (size == 0)
				return 0;
			int diff = (to - from);
			while (diff < 0)
				diff += size;
			return diff;
		}

		int soundoffset;
		int SNDDXGetAudioSpace()
		{
			if (DSoundBuffer == null) return 0;

			int playcursor = DSoundBuffer.CurrentPlayPosition;
			int writecursor = DSoundBuffer.CurrentWritePosition;

			int curToWrite = circularDist(soundoffset, writecursor, BufferSize);
			int curToPlay = circularDist(soundoffset, playcursor, BufferSize);

			if (curToWrite < curToPlay)
				return 0; // in-between the two cursors. we shouldn't write anything during this time.

			return curToPlay / 4;
		}

		public void UpdateSilence()
		{
			Muted = true;
			UpdateSound();
			Muted = false;
		}

		public void UpdateSound()
		{
			if (Global.Config.SoundEnabled == false || disposed)
			{
				if (asyncsoundProvider != null) asyncsoundProvider.DiscardSamples();
				if (syncsoundProvider != null) syncsoundProvider.DiscardSamples();
				return;
			}

			int samplesNeeded = SNDDXGetAudioSpace() * 2;
			short[] samples;

			int samplesProvided = 0;


			if (Muted)
			{
				if (samplesNeeded == 0)
					return;
				samples = new short[samplesNeeded];
				samplesProvided = samplesNeeded;

				if (asyncsoundProvider != null) asyncsoundProvider.DiscardSamples();
				if (syncsoundProvider != null) syncsoundProvider.DiscardSamples();
			}
			else if (syncsoundProvider != null)
			{
				if (DSoundBuffer == null) return; // can cause SNDDXGetAudioSpace() = 0
				int nsampgot;

				syncsoundProvider.GetSamples(out samples, out nsampgot);

				samplesProvided = 2 * nsampgot;

				if (!Global.ForceNoThrottle)
					while (samplesNeeded < samplesProvided)
					{
						System.Threading.Thread.Sleep((samplesProvided - samplesNeeded) / 88); // let audio clock control sleep time
						samplesNeeded = SNDDXGetAudioSpace() * 2;
					}
			}
			else if (asyncsoundProvider != null)
			{
				if (samplesNeeded == 0)
					return;
				samples = new short[samplesNeeded];
				//if (asyncsoundProvider != null && Muted == false)
				//{
				semisync.BaseSoundProvider = asyncsoundProvider;
				semisync.GetSamples(samples);
				//}
				//else asyncsoundProvider.DiscardSamples();
				samplesProvided = samplesNeeded;
			}
			else
				return;
			
			int cursor = soundoffset;
			for (int i = 0; i < samplesProvided; i++)
			{
				short s = samples[i];
				SoundBuffer[cursor++] = (byte)(s & 0xFF);
				SoundBuffer[cursor++] = (byte)(s >> 8);

				if (cursor >= SoundBuffer.Length)
					cursor = 0;
			}

			DSoundBuffer.Write(SoundBuffer, 0, LockFlags.EntireBuffer);

			soundoffset += samplesProvided * 2;
			soundoffset %= BufferSize;
		}

		/// <summary>
		/// Range: 0-100
		/// </summary>
		/// <param name="vol"></param>
		public void ChangeVolume(int vol)
		{
			if (vol > 100)
				vol = 100;
			if (vol < 0)
				vol = 0;
			Global.Config.SoundVolume = vol;
			UpdateSoundSettings();
		}

		/// <summary>
		/// Uses Global.Config.SoundEnabled, this just notifies the object to read it
		/// </summary>
		public void UpdateSoundSettings()
		{
			int vol = Global.Config.SoundVolume;
			if (!Global.Config.SoundEnabled || Global.Config.SoundVolume == 0)
				DSoundBuffer.Volume = -5000;
			else
				DSoundBuffer.Volume = 0 - ((100 - Global.Config.SoundVolume) * 15);

			/* //adelikat: I've been told this isn't TAS safe, so I'm disabling this speed hack
			if (Global.Emulator is NES)
			{
				NES n = Global.Emulator as NES;
				if (Global.Config.SoundEnabled == false)
					n.SoundOn = false;
				else
					n.SoundOn = true;
			}
			*/
		}
	}
#else
	//OpenAL implementation for other platforms
	public class Sound
	{
		public bool Muted = false;
		public bool needDiscard;
		private AudioContext _audContext;
		private int _audSource;
		private const int BUFFER_SIZE = 735 * 2 * 2; // 1/60th of a second, 2 bytes per sample, 2 channels;
		private ISoundProvider asyncsoundProvider;
		private ISyncSoundProvider syncsoundProvider;
		private BufferedAsync semisync = new BufferedAsync();

		public Sound()
		{
			try
			{
				_audContext = new AudioContext();
				_audSource = AL.GenSource();
				//Buffer 6 frames worth of sound
				//How large of a buffer we need seems to depend on the console
				//For GameGear, 3 or 4 is usually fine. For NES I need 6 frames or it can get choppy.
				for(int i=0; i<6; i++)
				{
					int buffer = AL.GenBuffer();
					short[] samples = new short[BUFFER_SIZE>>1]; //Initialize with empty sound
					AL.BufferData(buffer, ALFormat.Stereo16, samples, BUFFER_SIZE, 44100);
					AL.SourceQueueBuffer(_audSource, buffer);
				}
			} 
			catch(AudioException e)
			{ 
				System.Windows.Forms.MessageBox.Show("Unable to initalize sound. That's too bad.");
			}
		}

		public void StartSound()
		{
			AL.SourcePlay(_audSource);
		}

		public bool IsPlaying = false;

		public void StopSound()
		{
			AL.SourceStop(_audSource);
		}
		
		public void Dispose()
		{
			//Todo: Should I delete the buffers?
			AL.DeleteSource(_audSource);
		}

		int SNDDXGetAudioSpace()
		{
			return BUFFER_SIZE>>2;
		}

		public void UpdateSilence()
		{
			Muted = true;
			UpdateSound();
			Muted = false;
		}

		public void UpdateSound()
		{
			if (Global.Config.SoundEnabled == false)
			{
				if (asyncsoundProvider != null) asyncsoundProvider.DiscardSamples();
				if (syncsoundProvider != null) syncsoundProvider.DiscardSamples();
				return;
			}
			int amtToFill = 0;
			AL.GetSource(_audSource, ALGetSourcei.BuffersProcessed, out amtToFill);
			for(int i=0; i<amtToFill; i++)
			{
				int samplesNeeded = SNDDXGetAudioSpace() * 2;
				int samplesProvided = 0;
				short[] samples;

				if (Muted)
				{
					if (samplesNeeded == 0)
						return;
					samples = new short[samplesNeeded];
					samplesProvided = samplesNeeded;
				}
				else if (syncsoundProvider != null)
				{
					int nsampgot;
					syncsoundProvider.GetSamples(out samples, out nsampgot);
					samplesProvided = 2 * nsampgot;
					
					if (!Global.ForceNoThrottle)
						while (samplesNeeded < samplesProvided)
					{
						System.Threading.Thread.Sleep((samplesProvided - samplesNeeded) / 88); // let audio clock control sleep time
						samplesNeeded = SNDDXGetAudioSpace() * 2;
					}
				}
				else if (asyncsoundProvider != null)
				{
					if (samplesNeeded == 0)
						return;
					samples = new short[samplesNeeded];
					semisync.BaseSoundProvider = asyncsoundProvider;
					semisync.GetSamples(samples);
					samplesProvided = samplesNeeded;
				}
				else
					return;

				AL.GetSource(_audSource, ALGetSourcei.BuffersProcessed, out amtToFill);
				int buffer = AL.SourceUnqueueBuffer(_audSource);
				AL.BufferData(buffer, ALFormat.Stereo16, samples, samplesProvided*2, 44100);	
				AL.SourceQueueBuffer(_audSource, buffer);
			}
			if(AL.GetSourceState(_audSource) != ALSourceState.Playing)
			{
				AL.SourcePlay(_audSource);
			}
		}

		/// <summary>
		/// Range: 0-100
		/// </summary>
		/// <param name="vol"></param>
		public void ChangeVolume(int vol)
		{
			Global.Config.SoundVolume = vol;
			UpdateSoundSettings();
		}

		/// <summary>
		/// Uses Global.Config.SoundEnabled, this just notifies the object to read it
		/// </summary>
		public void UpdateSoundSettings()
		{
			if (Global.Emulator is NES)
			{
				NES n = Global.Emulator as NES;
				if (Global.Config.SoundEnabled == false)
					n.SoundOn = false;
				else
					n.SoundOn = true;
			}
		}

		public void SetSyncInputPin(ISyncSoundProvider source)
		{
			syncsoundProvider = source;
			asyncsoundProvider = null;
			semisync.DiscardSamples();
		}
		
		public void SetAsyncInputPin(ISoundProvider source)
		{
			syncsoundProvider = null;
			asyncsoundProvider = source;
			semisync.BaseSoundProvider = source;
		}
	}
#endif
}
