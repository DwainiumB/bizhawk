﻿//http://wiki.superfamicom.org/snes/show/Backgrounds

//TODO 
//libsnes needs to be modified to support multiple instances - THIS IS NECESSARY - or else loading one game and then another breaks things
//rename snes.dll so nobody thinks it's a stock snes.dll (we'll be editing it substantially at some point)
//wrap dll code around some kind of library-accessing interface so that it doesnt malfunction if the dll is unavailable

using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BizHawk.Emulation.Consoles.Nintendo.SNES
{
	public unsafe static class LibsnesDll
	{
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern string snes_library_id();
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int snes_library_revision_major();
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int snes_library_revision_minor();

		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_init();
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_power();
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_run();
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_term();

		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_load_cartridge_normal(
			[MarshalAs(UnmanagedType.LPStr)]
			string rom_xml, 
			[MarshalAs(UnmanagedType.LPArray)]
			byte[] rom_data, 
			int rom_size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void snes_video_refresh_t(int *data, int width, int height);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void snes_input_poll_t();
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate ushort snes_input_state_t(int port, int device, int index, int id);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void snes_audio_sample_t(ushort left, ushort right);

		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_set_video_refresh(snes_video_refresh_t video_refresh);
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_set_input_poll(snes_input_poll_t input_poll);
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_set_input_state(snes_input_state_t input_state);
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_set_audio_sample(snes_audio_sample_t audio_sample);

		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool snes_check_cartridge(
			[MarshalAs(UnmanagedType.LPArray)] byte[] rom_data,
			int rom_size);

		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern SNES_REGION snes_get_region();

		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int snes_get_memory_size(SNES_MEMORY id);
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr snes_get_memory_data(SNES_MEMORY id);

		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int snes_serialize_size();
    
		[return: MarshalAs(UnmanagedType.U1)]
    [DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool snes_serialize(IntPtr data, int size);
		
		[return: MarshalAs(UnmanagedType.U1)]
		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool snes_unserialize(IntPtr data, int size);

		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern void snes_set_layer_enable(int layer, int priority,
			[MarshalAs(UnmanagedType.U1)]
			bool enable
			);

		[DllImport("snes.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int snes_peek_logical_register(SNES_REG reg);

		public enum SNES_REG : int
		{
			//$2105
			BG_MODE = 0,
			BG3_PRIORITY = 1,
			BG1_TILESIZE = 2,
			BG2_TILESIZE = 3,
			BG3_TILESIZE = 4,
			BG4_TILESIZE = 5,
			//$2107
			BG1_SCADDR = 10,
			BG1_SCSIZE = 11,
			//$2108
			BG2_SCADDR = 12,
			BG2_SCSIZE = 13,
			//$2109
			BG3_SCADDR = 14,
			BG3_SCSIZE = 15,
			//$210A
			BG4_SCADDR = 16,
			BG4_SCSIZE = 17,
			//$210B
			BG1_TDADDR = 20,
			BG2_TDADDR = 21,
			//$210C
			BG3_TDADDR = 22,
			BG4_TDADDR = 23
		}
		
		public enum SNES_MEMORY : uint
		{
			CARTRIDGE_RAM = 0,
			CARTRIDGE_RTC = 1,
			BSX_RAM = 2,
			BSX_PRAM = 3,
			SUFAMI_TURBO_A_RAM = 4,
			SUFAMI_TURBO_B_RAM = 5,
			GAME_BOY_RAM = 6,
			GAME_BOY_RTC = 7,

			WRAM = 100,
			APURAM = 101,
			VRAM = 102,
			OAM = 103,
			CGRAM = 104,
		}

		public enum SNES_REGION : uint
		{
			NTSC = 0,
			PAL = 1,
		}

		public enum SNES_DEVICE : uint
		{
			NONE = 0,
			JOYPAD = 1,
			MULTITAP = 2,
			MOUSE = 3,
			SUPER_SCOPE = 4,
			JUSTIFIER = 5,
			JUSTIFIERS = 6,
			SERIAL_CABLE = 7
		}

		public enum SNES_DEVICE_ID : uint
		{
			JOYPAD_B = 0,
			JOYPAD_Y = 1,
			JOYPAD_SELECT = 2,
			JOYPAD_START = 3,
			JOYPAD_UP = 4,
			JOYPAD_DOWN = 5,
			JOYPAD_LEFT = 6,
			JOYPAD_RIGHT = 7,
			JOYPAD_A = 8,
			JOYPAD_X = 9,
			JOYPAD_L = 10,
			JOYPAD_R = 11
		}
	}


	public unsafe class LibsnesCore : IEmulator, IVideoProvider, ISoundProvider
	{
		bool disposed = false;
		public void Dispose()
		{
			if (disposed) return;
			disposed = true;

			BizHawk.Emulation.Consoles.Nintendo.SNES.LibsnesDll.snes_set_video_refresh(null);
			BizHawk.Emulation.Consoles.Nintendo.SNES.LibsnesDll.snes_set_input_poll(null);
			BizHawk.Emulation.Consoles.Nintendo.SNES.LibsnesDll.snes_set_input_state(null);
			BizHawk.Emulation.Consoles.Nintendo.SNES.LibsnesDll.snes_set_audio_sample(null);

			LibsnesDll.snes_term();
		}

		//we can only have one active snes core at a time, due to libsnes being so static.
		//so we'll track the current one here and detach the previous one whenever a new one is booted up.
		static LibsnesCore CurrLibsnesCore;

		public LibsnesCore(byte[] romData)
		{
			//attach this core as the current
			if(CurrLibsnesCore != null)
				CurrLibsnesCore.Dispose();
			CurrLibsnesCore = this;

			LibsnesDll.snes_init();

			vidcb = new LibsnesDll.snes_video_refresh_t(snes_video_refresh);
			BizHawk.Emulation.Consoles.Nintendo.SNES.LibsnesDll.snes_set_video_refresh(vidcb);

			pollcb = new LibsnesDll.snes_input_poll_t(snes_input_poll);
			BizHawk.Emulation.Consoles.Nintendo.SNES.LibsnesDll.snes_set_input_poll(pollcb);

			inputcb = new LibsnesDll.snes_input_state_t(snes_input_state);
			BizHawk.Emulation.Consoles.Nintendo.SNES.LibsnesDll.snes_set_input_state(inputcb);

			soundcb = new LibsnesDll.snes_audio_sample_t(snes_audio_sample);
			BizHawk.Emulation.Consoles.Nintendo.SNES.LibsnesDll.snes_set_audio_sample(soundcb);

			// start up audio resampler
			resampler.StartSession(resamplingfactor);

			//strip header
			if ((romData.Length & 0x7FFF) == 512)
			{
				var newData = new byte[romData.Length - 512];
				Array.Copy(romData, 512, newData, 0, newData.Length);
				romData = newData;
			}
			LibsnesDll.snes_load_cartridge_normal(null, romData, romData.Length);

			SetupMemoryDomains(romData);
		}

		//must keep references to these so that they wont get garbage collected
		LibsnesDll.snes_video_refresh_t vidcb;
		LibsnesDll.snes_input_poll_t pollcb;
		LibsnesDll.snes_input_state_t inputcb;
		LibsnesDll.snes_audio_sample_t soundcb;

		ushort snes_input_state(int port, int device, int index, int id)
		{
			//Console.WriteLine("{0} {1} {2} {3}", port, device, index, id);

			string key = "P" + (1 + port) + " ";
			if ((LibsnesDll.SNES_DEVICE)device == LibsnesDll.SNES_DEVICE.JOYPAD)
			{
				switch ((LibsnesDll.SNES_DEVICE_ID)id)
				{
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_A: key += "A"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_B: key += "B"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_X: key += "X"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_Y: key += "Y"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_UP: key += "Up"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_DOWN: key += "Down"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_LEFT: key += "Left"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_RIGHT: key += "Right"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_L: key += "L"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_R: key += "R"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_SELECT: key += "Select"; break;
					case LibsnesDll.SNES_DEVICE_ID.JOYPAD_START: key += "Start"; break;
				}

				return (ushort)(Controller[key] ? 1 : 0);
			}

			return 0;

		}

		void snes_input_poll()
		{
		}

		void snes_video_refresh(int* data, int width, int height)
		{
			vidWidth = width;
			vidHeight = height;
			int size = vidWidth * vidHeight;
			if (vidBuffer.Length != size)
				vidBuffer = new int[size];
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
				{
					int si = y * 1024 + x;
					int di = y * vidWidth + x;
					int rgb = data[si];
					vidBuffer[di] = rgb;
				}
		}

		public void FrameAdvance(bool render)
		{
			LibsnesDll.snes_set_layer_enable(0, 0, CoreInputComm.SNES_ShowBG1_0);
			LibsnesDll.snes_set_layer_enable(0, 1, CoreInputComm.SNES_ShowBG1_1);
			LibsnesDll.snes_set_layer_enable(1, 0, CoreInputComm.SNES_ShowBG2_0);
			LibsnesDll.snes_set_layer_enable(1, 1, CoreInputComm.SNES_ShowBG2_1);
			LibsnesDll.snes_set_layer_enable(2, 0, CoreInputComm.SNES_ShowBG3_0);
			LibsnesDll.snes_set_layer_enable(2, 1, CoreInputComm.SNES_ShowBG3_1);
			LibsnesDll.snes_set_layer_enable(3, 0, CoreInputComm.SNES_ShowBG4_0);
			LibsnesDll.snes_set_layer_enable(3, 1, CoreInputComm.SNES_ShowBG4_1);
			LibsnesDll.snes_set_layer_enable(4, 0, CoreInputComm.SNES_ShowOBJ_0);
			LibsnesDll.snes_set_layer_enable(4, 1, CoreInputComm.SNES_ShowOBJ_1);
			LibsnesDll.snes_set_layer_enable(4, 2, CoreInputComm.SNES_ShowOBJ_2);
			LibsnesDll.snes_set_layer_enable(4, 3, CoreInputComm.SNES_ShowOBJ_3);

			//apparently this is one frame?
			timeFrameCounter++;
			LibsnesDll.snes_run();
		}

		//video provider
		int IVideoProvider.BackgroundColor { get { return 0; } }
		int[] IVideoProvider.GetVideoBuffer() { return vidBuffer; }
		int IVideoProvider.VirtualWidth { get { return vidWidth; } }
		int IVideoProvider.BufferWidth { get { return vidWidth; } }
		int IVideoProvider.BufferHeight { get { return vidHeight; } }

		int[] vidBuffer = new int[256 * 256];
		int vidWidth = 256, vidHeight = 256;

		public IVideoProvider VideoProvider { get { return this; } }
		public ISoundProvider SoundProvider { get { return this; } }

		public ControllerDefinition ControllerDefinition { get { return SNESController; } }
		IController controller;
		public IController Controller
		{
			get { return controller; }
			set { controller = value; }
		}

		public static readonly ControllerDefinition SNESController =
			new ControllerDefinition
			{
				Name = "SNES Controller",
				BoolButtons = {
					"P1 Up", "P1 Down", "P1 Left", "P1 Right", "P1 Select", "P1 Start", "P1 B", "P1 A", "P1 X", "P1 Y", "P1 L", "P1 R", "Reset",
				}
			};

		int timeFrameCounter;
		public int Frame { get { return timeFrameCounter; } }
		public int LagCount { get; set; }
		public bool IsLagFrame { get; private set; }
		public string SystemId { get { return "SNES"; } }
		public bool DeterministicEmulation { get; set; }
		public bool SaveRamModified
		{
			set { }
			get
			{
				return LibsnesDll.snes_get_memory_size(LibsnesDll.SNES_MEMORY.CARTRIDGE_RAM) != 0;
			}
		}

		public byte[] ReadSaveRam { get { return snes_get_memory_data_read(LibsnesDll.SNES_MEMORY.CARTRIDGE_RAM); } }
		public static byte[] snes_get_memory_data_read(LibsnesDll.SNES_MEMORY id)
		{
			var size = (int)LibsnesDll.snes_get_memory_size(id);
			if (size == 0) return new byte[0];
			var data = LibsnesDll.snes_get_memory_data(id);
			var ret = new byte[size];
			Marshal.Copy(data, ret, 0, size);
			return ret;
		}

		public void StoreSaveRam(byte[] data)
		{
			var size = (int)LibsnesDll.snes_get_memory_size(LibsnesDll.SNES_MEMORY.CARTRIDGE_RAM);
			if (size == 0) return;
			var emudata = LibsnesDll.snes_get_memory_data(LibsnesDll.SNES_MEMORY.CARTRIDGE_RAM);
			Marshal.Copy(data, 0, emudata, size);
		}

		public void ResetFrameCounter() { timeFrameCounter = 0; }
		public void SaveStateText(TextWriter writer)
		{
			var temp = SaveStateBinary();
			temp.SaveAsHex(writer);
		}
		public void LoadStateText(TextReader reader)
		{
			string hex = reader.ReadLine();
			byte[] state = new byte[hex.Length / 2];
			state.ReadFromHex(hex);
			LoadStateBinary(new BinaryReader(new MemoryStream(state)));
		}

		public void SaveStateBinary(BinaryWriter writer)
		{
			int size = LibsnesDll.snes_serialize_size();
			byte[] buf = new byte[size];
			fixed (byte* pbuf = &buf[0])
				LibsnesDll.snes_serialize(new IntPtr(pbuf), size);
			writer.Write(buf);
			writer.Flush();
		}
		public void LoadStateBinary(BinaryReader reader)
		{
			int size = LibsnesDll.snes_serialize_size();
			var ms = new MemoryStream();
			reader.BaseStream.CopyTo(ms);
			var buf = ms.ToArray();
			fixed (byte* pbuf = &buf[0])
				LibsnesDll.snes_unserialize(new IntPtr(pbuf), size);
		}
		public byte[] SaveStateBinary()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			SaveStateBinary(bw);
			bw.Flush();
			return ms.ToArray();
		}

		// Arbitrary extensible core comm mechanism
		public CoreInputComm CoreInputComm { get; set; }
		public CoreOutputComm CoreOutputComm { get { return _CoreOutputComm; } }
		CoreOutputComm _CoreOutputComm = new CoreOutputComm();

		// ----- Client Debugging API stuff -----
		unsafe MemoryDomain MakeMemoryDomain(string name, LibsnesDll.SNES_MEMORY id, Endian endian)
		{
			IntPtr block = LibsnesDll.snes_get_memory_data(LibsnesDll.SNES_MEMORY.WRAM);
			int size = LibsnesDll.snes_get_memory_size(id);
			int mask = size - 1;
			byte* blockptr = (byte*)block.ToPointer();
			MemoryDomain md;

			//have to bitmask these somehow because it's unmanaged memory and we would hate to clobber things or make them nondeterministic
			if (Util.IsPowerOfTwo(size))
			{
				//can &mask for speed
				md = new MemoryDomain(name, size, endian,
					 (addr) => blockptr[addr & mask],
					 (addr, value) => blockptr[addr & mask] = value);
			}
			else
			{
				//have to use % (only OAM needs this, it seems)
				md = new MemoryDomain(name, size, endian,
					 (addr) => blockptr[addr % size],
					 (addr, value) => blockptr[addr % size] = value);
			}

			MemoryDomains.Add(md);

			return md;

			//doesnt cache the addresses. safer. slower. necessary? don't know
			//return new MemoryDomain(name, size, endian,
			//  (addr) => Peek(LibsnesDll.SNES_MEMORY.WRAM, addr & mask),
			//  (addr, value) => Poke(LibsnesDll.SNES_MEMORY.WRAM, addr & mask, value));
		}

		void SetupMemoryDomains(byte[] romData)
		{
			MemoryDomains = new List<MemoryDomain>();

			var romDomain = new MemoryDomain("CARTROM", romData.Length, Endian.Little,
				(addr) => romData[addr],
				(addr, value) => romData[addr] = value);

			MainMemory = MakeMemoryDomain("WRAM", LibsnesDll.SNES_MEMORY.WRAM, Endian.Little);
			MemoryDomains.Add(romDomain);
			MakeMemoryDomain("CARTRAM", LibsnesDll.SNES_MEMORY.CARTRIDGE_RAM, Endian.Little);
			MakeMemoryDomain("VRAM", LibsnesDll.SNES_MEMORY.VRAM, Endian.Little);
			MakeMemoryDomain("OAM", LibsnesDll.SNES_MEMORY.OAM, Endian.Little);
			MakeMemoryDomain("CGRAM", LibsnesDll.SNES_MEMORY.CGRAM, Endian.Little);
			MakeMemoryDomain("APURAM", LibsnesDll.SNES_MEMORY.APURAM, Endian.Little);
		}
		public IList<MemoryDomain> MemoryDomains { get; private set; }
		public MemoryDomain MainMemory { get; private set; }




		#region audio stuff

		/// <summary>stores input samples as they come from the libsnes core</summary>
		Queue<short> AudioInBuffer = new Queue<short>();
		/// <summary>stores samples that have been converted to 44100hz</summary>
		Queue<short> AudioOutBuffer = new Queue<short>();

		GCHandle _gc_snes_audio_sample;

		/// <summary>total number of samples (left and right combined) in the InBuffer before we ask for resampling</summary>
		const int resamplechunk = 1000;
		/// <summary>actual sampling factor used</summary>
		const double resamplingfactor = 44100.0 / 32040.5;

		//Sound.Utilities.IStereoResampler resampler = new Sound.Utilities.BizhawkResampler(false);
		//Sound.Utilities.IStereoResampler resampler = new Sound.Utilities.SinkResampler(12);
		//Sound.Utilities.IStereoResampler resampler = new Sound.Utilities.CubicResampler();
		//Sound.Utilities.IStereoResampler resampler = new Sound.Utilities.LinearResampler();
		Sound.Utilities.SpeexResampler resampler = new Sound.Utilities.SpeexResampler(6);

		Sound.MetaspuSoundProvider metaspu = new Sound.MetaspuSoundProvider(Sound.ESynchMethod.ESynchMethod_V);

		void snes_audio_sample(ushort left, ushort right)
		{

			AudioInBuffer.Enqueue((short)left);
			AudioInBuffer.Enqueue((short)right);

			/*
			try
			{
				// fixme: i get all sorts of crashes if i do the resampling in here.  what?

				
				if (AudioInBuffer.Count >= resamplechunk)
				{
					resampler.ResampleChunk(AudioInBuffer, AudioOutBuffer, false);
					// drain into the metaspu immediately
					// we could skip this step and drain directly by changing SampleBuffers
					while (AudioOutBuffer.Count > 0)
						metaspu.buffer.enqueue_sample(AudioOutBuffer.Dequeue(), AudioOutBuffer.Dequeue());
				}
				 
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show(e.ToString());
				AudioOutBuffer.Clear();
			}
			 */
	
			
		}


		//BinaryWriter dbgs = new BinaryWriter(File.Open("dbgwav.raw", FileMode.Create, FileAccess.Write));

		public void GetSamples(short[] samples)
		{
			// do resampling here, instead of when the samples are generated; see "fixme" comment in snes_audio_sample()

			if (true)
			{
				//lock (AudioInBuffer)
				//{
					resampler.ResampleChunk(AudioInBuffer, AudioOutBuffer, false);
				//}
				// drain into the metaspu immediately
				// we could skip this step and drain directly by changing SampleBuffers implementation
				while (AudioOutBuffer.Count > 0)
				{
					short l = AudioOutBuffer.Dequeue();
					short r = AudioOutBuffer.Dequeue();
					//metaspu.buffer.enqueue_sample(AudioOutBuffer.Dequeue(), AudioOutBuffer.Dequeue());
					metaspu.buffer.enqueue_sample(l, r);
					//dbgs.Write(l);
					//dbgs.Write(r);
				}
			}
			metaspu.GetSamples(samples);
		}

		public void DiscardSamples()
		{
			metaspu.DiscardSamples();
		}

		public int MaxVolume { get; set; }

		#endregion audio stuff

	}
}