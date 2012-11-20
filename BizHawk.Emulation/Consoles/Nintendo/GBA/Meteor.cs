﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace BizHawk.Emulation.Consoles.Nintendo.GBA
{
	public class GBA : IEmulator, IVideoProvider, ISyncSoundProvider
	{
		public static readonly ControllerDefinition GBAController =
		new ControllerDefinition
		{
			Name = "GBA Controller",
			BoolButtons =
			{					
				"Up", "Down", "Left", "Right", "Select", "Start", "B", "A", "L", "R"//, "Reset", "Power",		
			}
		};
		public ControllerDefinition ControllerDefinition { get { return GBAController; } }
		public IController Controller { get; set; }

		public void Load(byte[] rom, byte[] bios)
		{
			if (bios.Length != 16384)
				throw new Exception("GBA bios must be exactly 16384 bytes!");
			Init();
			LibMeteor.libmeteor_reset();
			LibMeteor.libmeteor_loadbios(bios, (uint)bios.Length);
			LibMeteor.libmeteor_loadrom(rom, (uint)rom.Length);

			SetUpMemoryDomains();
		}

		public void FrameAdvance(bool render, bool rendersound = true)
		{
			Controller.UpdateControls(Frame++);
			IsLagFrame = true;
			LibMeteor.libmeteor_frameadvance();
			if (IsLagFrame)
				LagCount++;
		}

		public int Frame { get; private set; }
		public int LagCount { get; set; }
		public bool IsLagFrame { get; private set; }
		public string SystemId { get { return "GBA"; } }
		public bool DeterministicEmulation { get { return true; } }

		public void ResetFrameCounter()
		{
			Frame = 0;
			LagCount = 0;
		}

		#region saveram

		public byte[] ReadSaveRam()
		{
			return new byte[0];
		}

		public void StoreSaveRam(byte[] data)
		{
		}

		public void ClearSaveRam()
		{
		}

		public bool SaveRamModified { get { return false; } set { } }

		#endregion

		#region savestates

		public void SaveStateText(System.IO.TextWriter writer)
		{
		}

		public void LoadStateText(System.IO.TextReader reader)
		{
		}

		public void SaveStateBinary(System.IO.BinaryWriter writer)
		{
		}

		public void LoadStateBinary(System.IO.BinaryReader reader)
		{
		}

		public byte[] SaveStateBinary()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			SaveStateBinary(bw);
			bw.Flush();
			return ms.ToArray();
		}

		#endregion

		public CoreInputComm CoreInputComm { get; set; }

		CoreOutputComm _CoreOutputComm = new CoreOutputComm
		{
			VsyncNum = 262144,
			VsyncDen = 4389
		};

		public CoreOutputComm CoreOutputComm { get { return _CoreOutputComm; } }

		#region memorydomains

		List<MemoryDomain> _MemoryDomains = new List<MemoryDomain>();
		public IList<MemoryDomain> MemoryDomains { get { return _MemoryDomains; } }
		public MemoryDomain MainMemory
		{
			// some core tools assume MainMemory == MemoryDomains[0], so do that anyway
			get { return MemoryDomains[0]; }
		}

		void AddMemoryDomain(LibMeteor.MemoryArea which, int size, string name)
		{
			IntPtr data = LibMeteor.libmeteor_getmemoryarea(which);
			if (data == IntPtr.Zero)
				throw new Exception("libmeteor_getmemoryarea() returned NULL??");

			MemoryDomain md = new MemoryDomain(name, size, Endian.Little,
				delegate(int addr)
				{
					unsafe
					{
						byte* d = (byte*)data;
						if (addr < 0 || addr >= size)
							throw new IndexOutOfRangeException();
						return d[addr];
					}
				},
				delegate(int addr, byte val)
				{
					unsafe
					{
						byte* d = (byte*)data;
						if (addr < 0 || addr >= size)
							throw new IndexOutOfRangeException();
						d[addr] = val;
					}
				});
			_MemoryDomains.Add(md);
		}

		void SetUpMemoryDomains()
		{
			_MemoryDomains.Clear();
			// this must be first to coincide with "main memory"
			// note that ewram could also be considered main memory depending on which hairs you split
			AddMemoryDomain(LibMeteor.MemoryArea.iwram, 32 * 1024, "IWRAM");
			AddMemoryDomain(LibMeteor.MemoryArea.ewram, 256 * 1024, "EWRAM");
			AddMemoryDomain(LibMeteor.MemoryArea.bios, 16 * 1024, "BIOS");
			AddMemoryDomain(LibMeteor.MemoryArea.palram, 1024, "PALRAM");
			AddMemoryDomain(LibMeteor.MemoryArea.vram, 96 * 1024, "VRAM");
			AddMemoryDomain(LibMeteor.MemoryArea.oam, 1024, "OAM");
			// even if the rom is less than 32MB, the whole is still valid in meteor
			AddMemoryDomain(LibMeteor.MemoryArea.rom, 32 * 1024 * 1024, "ROM");
		}

		#endregion

		/// <summary>like libsnes, the library is single-instance</summary>
		static GBA attachedcore;
		/// <summary>hold pointer to message callback so it won't get GCed</summary>
		LibMeteor.MessageCallback messagecallback;
		/// <summary>hold pointer to input callback so it won't get GCed</summary>
		LibMeteor.InputCallback inputcallback;

		LibMeteor.Buttons GetInput()
		{
			// libmeteor bitflips everything itself, so 0 == off, 1 == on
			IsLagFrame = false;
			LibMeteor.Buttons ret = 0;
			if (Controller["Up"]) ret |= LibMeteor.Buttons.BTN_UP;
			if (Controller["Down"]) ret |= LibMeteor.Buttons.BTN_DOWN;
			if (Controller["Left"]) ret |= LibMeteor.Buttons.BTN_LEFT;
			if (Controller["Right"]) ret |= LibMeteor.Buttons.BTN_RIGHT;
			if (Controller["Select"]) ret |= LibMeteor.Buttons.BTN_SELECT;
			if (Controller["Start"]) ret |= LibMeteor.Buttons.BTN_START;
			if (Controller["B"]) ret |= LibMeteor.Buttons.BTN_B;
			if (Controller["A"]) ret |= LibMeteor.Buttons.BTN_A;
			if (Controller["L"]) ret |= LibMeteor.Buttons.BTN_L;
			if (Controller["R"]) ret |= LibMeteor.Buttons.BTN_R;
			return ret;
		}

		void PrintMessage(string msg, bool abort)
		{
			if (!abort)
				Console.Write(msg.Replace("\n", "\r\n"));
			else
				throw new Exception("libmeteor abort:\n " + msg);
		}

		void Init()
		{
			if (attachedcore != null)
				attachedcore.Dispose();

			messagecallback = PrintMessage;
			inputcallback = GetInput;
			LibMeteor.libmeteor_setmessagecallback(messagecallback);
			LibMeteor.libmeteor_setkeycallback(inputcallback);

			LibMeteor.libmeteor_init();
			videobuffer = new int[240 * 160];
			videohandle = GCHandle.Alloc(videobuffer, GCHandleType.Pinned);
			soundbuffer = new short[2048]; // nominal length of one frame is something like 1480 shorts?
			soundhandle = GCHandle.Alloc(soundbuffer, GCHandleType.Pinned);

			if (!LibMeteor.libmeteor_setbuffers
				(videohandle.AddrOfPinnedObject(), (uint)(sizeof(int) * videobuffer.Length),
				soundhandle.AddrOfPinnedObject(), (uint)(sizeof(short) * soundbuffer.Length)))
				throw new Exception("libmeteor_setbuffers() returned false??");

			attachedcore = this;
		}

		bool disposed = false;
		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				videohandle.Free();
				soundhandle.Free();
				// guarantee crash if it gets accessed
				LibMeteor.libmeteor_setbuffers(IntPtr.Zero, 240 * 160 * 4, IntPtr.Zero, 4);
				messagecallback = null;
				inputcallback = null;
				LibMeteor.libmeteor_setmessagecallback(messagecallback);
				LibMeteor.libmeteor_setkeycallback(inputcallback);
				_MemoryDomains.Clear();
			}
		}

		#region IVideoProvider

		public IVideoProvider VideoProvider { get { return this; } }

		int[] videobuffer;
		GCHandle videohandle;

		public int[] GetVideoBuffer() { return videobuffer; }
		public int VirtualWidth { get { return 240; } }
		public int BufferWidth { get { return 240; } }
		public int BufferHeight { get { return 160; } }
		public int BackgroundColor { get { return unchecked((int)0xff000000); } }

		#endregion

		#region ISoundProvider

		short[] soundbuffer;
		GCHandle soundhandle;

		public ISoundProvider SoundProvider { get { return null; } }
		public ISyncSoundProvider SyncSoundProvider { get { return this; } }
		public bool StartAsyncSound() { return false; }
		public void EndAsyncSound() { }

		public void GetSamples(out short[] samples, out int nsamp)
		{
			uint nbytes = LibMeteor.libmeteor_emptysound();
			samples = soundbuffer;
			nsamp = (int)(nbytes / 4);
		}

		public void DiscardSamples()
		{
			LibMeteor.libmeteor_emptysound();
		}

		#endregion
	}
}
