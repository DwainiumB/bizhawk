﻿using System.IO;
using System.Linq;
using BizHawk.Emulation.Common;
using Jellyfish.Virtu;
using Jellyfish.Virtu.Services;
using System;

namespace BizHawk.Emulation.Cores.Computers.AppleII
{
	[CoreAttributes(
		"Virtu",
		"TODO",
		isPorted: true,
		isReleased: false
		)]
	public partial class AppleII : IEmulator, IStatable
	{
		[CoreConstructor("AppleII")]
		public AppleII(CoreComm comm, GameInfo game, byte[] rom, object Settings)
		{
			var ser = new BasicServiceProvider(this);
			ServiceProvider = ser;
			CoreComm = comm;

			_disk1 = rom;

			// TODO: get from Firmware provider
			_appleIIRom = File.ReadAllBytes("C:\\apple\\AppleIIe.rom");
			_diskIIRom = File.ReadAllBytes("C:\\apple\\DiskII.rom");


			_machine = new Machine();
			
			var vidService = new BizVideoService(_machine);
			_soundService = new BizAudioService(_machine);
			var gpService = new Jellyfish.Virtu.Services.GamePortService(_machine);
			var kbService = new BizKeyboardService(_machine);

			_machine.Services.AddService(typeof(Jellyfish.Virtu.Services.DebugService), new Jellyfish.Virtu.Services.DebugService(_machine));
			_machine.Services.AddService(typeof(Jellyfish.Virtu.Services.AudioService), _soundService);
			_machine.Services.AddService(typeof(Jellyfish.Virtu.Services.VideoService), vidService);
			_machine.Services.AddService(typeof(Jellyfish.Virtu.Services.GamePortService), gpService);
			_machine.Services.AddService(typeof(Jellyfish.Virtu.Services.KeyboardService), kbService);
			_machine.BizInitialize();

			(ServiceProvider as BasicServiceProvider).Register<IVideoProvider>(vidService);

			//make a writeable memory stream cloned from the rom.
			//for junk.dsk the .dsk is important because it determines the format from that
			var ms = new MemoryStream();
			ms.Write(rom,0,rom.Length);
			ms.Position = 0;
			bool writeProtected = false; //!!!!!!!!!!!!!!!!!!!
			Jellyfish.Virtu.Services.StorageService.LoadFile(ms, stream => _machine.BootDiskII.Drives[0].InsertDisk("junk.dsk", stream, writeProtected));
		}

		private readonly Machine _machine;
		private readonly byte[] _disk1;
		private readonly byte[] _appleIIRom;
		private readonly byte[] _diskIIRom;
		private readonly BizAudioService _soundService;

		private static readonly ControllerDefinition AppleIIController =
			new ControllerDefinition
			{
				Name = "Apple II Keyboard",
				BoolButtons =
				{
					"Up", "Down", "Left", "Right",
					"Tab", "Enter", "Escape", "Back", "Space",
					"Ctrl", "Shift", "Caps",
					"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
					"A", "B", "C", "D", "E", "F", "G", "H", "I",
					"J", "K", "L", "M", "N", "O", "P", "Q", "R",
					"S", "T", "U", "V", "W", "X", "Y", "Z"
				}
			};

		private class BizKeyboardService : KeyboardService
		{
			public BizKeyboardService(Machine _machine) : base(_machine) { }
			public override bool IsKeyDown(int key)
			{
                return key > 0;
			}
		}

		private class BizAudioService : AudioService, ISyncSoundProvider
		{
			public BizAudioService(Machine _machine) : base(_machine) { }
			public override void SetVolume(float volume)
			{
				
			}

			public void GetSamples(out short[] samples, out int nsamp)
			{
				samples = Array.ConvertAll(Source, b => (short)b); // TODO: is it really 8 bit?

				nsamp = Source.Length;
			}

			public void DiscardSamples()
			{
				//TODO
				//Source.DiscardSamples();
			}
		}

		private void FrameAdv(bool render, bool rendersound)
		{
			_machine.Buttons = GetButtons();
			_machine.BizFrameAdvance();
			Frame++;
		}

		private Buttons GetButtons()
		{
			Jellyfish.Virtu.Buttons ret = 0;
			if (Controller["Up"]) ret |= Jellyfish.Virtu.Buttons.Up;
			if (Controller["Down"]) ret |= Jellyfish.Virtu.Buttons.Down;
			if (Controller["Left"]) ret |= Jellyfish.Virtu.Buttons.Left;
			if (Controller["Right"]) ret |= Jellyfish.Virtu.Buttons.Right;
			if (Controller["Tab"]) ret |= Jellyfish.Virtu.Buttons.Tab;
			if (Controller["Enter"]) ret |= Jellyfish.Virtu.Buttons.Enter;
			if (Controller["Escape"]) ret |= Jellyfish.Virtu.Buttons.Escape;
			if (Controller["Back"]) ret |= Jellyfish.Virtu.Buttons.Back;
			if (Controller["Space"]) ret |= Jellyfish.Virtu.Buttons.Space;
			if (Controller["Ctrl"]) ret |= Jellyfish.Virtu.Buttons.Ctrl;
			if (Controller["Shift"]) ret |= Jellyfish.Virtu.Buttons.Shift;
			if (Controller["Caps"]) ret |= Jellyfish.Virtu.Buttons.Caps;
			if (Controller["1"]) ret |= Jellyfish.Virtu.Buttons.Key1;
			if (Controller["2"]) ret |= Jellyfish.Virtu.Buttons.Key2;
			if (Controller["3"]) ret |= Jellyfish.Virtu.Buttons.Key3;
			if (Controller["4"]) ret |= Jellyfish.Virtu.Buttons.Key4;
			if (Controller["5"]) ret |= Jellyfish.Virtu.Buttons.Key5;
			if (Controller["6"]) ret |= Jellyfish.Virtu.Buttons.Key6;
			if (Controller["7"]) ret |= Jellyfish.Virtu.Buttons.Key7;
			if (Controller["8"]) ret |= Jellyfish.Virtu.Buttons.Key8;
			if (Controller["9"]) ret |= Jellyfish.Virtu.Buttons.Key9;
			if (Controller["0"]) ret |= Jellyfish.Virtu.Buttons.Key0;
			if (Controller["A"]) ret |= Jellyfish.Virtu.Buttons.KeyA;
			if (Controller["B"]) ret |= Jellyfish.Virtu.Buttons.KeyB;
			if (Controller["C"]) ret |= Jellyfish.Virtu.Buttons.KeyC;
			if (Controller["D"]) ret |= Jellyfish.Virtu.Buttons.KeyD;
			if (Controller["E"]) ret |= Jellyfish.Virtu.Buttons.KeyE;
			if (Controller["F"]) ret |= Jellyfish.Virtu.Buttons.KeyF;
			if (Controller["G"]) ret |= Jellyfish.Virtu.Buttons.KeyG;
			if (Controller["H"]) ret |= Jellyfish.Virtu.Buttons.KeyH;
			if (Controller["I"]) ret |= Jellyfish.Virtu.Buttons.KeyI;
			if (Controller["J"]) ret |= Jellyfish.Virtu.Buttons.KeyJ;
			if (Controller["K"]) ret |= Jellyfish.Virtu.Buttons.KeyK;
			if (Controller["L"]) ret |= Jellyfish.Virtu.Buttons.KeyL;
			if (Controller["M"]) ret |= Jellyfish.Virtu.Buttons.KeyM;
			if (Controller["N"]) ret |= Jellyfish.Virtu.Buttons.KeyN;
			if (Controller["O"]) ret |= Jellyfish.Virtu.Buttons.KeyO;
			if (Controller["P"]) ret |= Jellyfish.Virtu.Buttons.KeyP;
			if (Controller["Q"]) ret |= Jellyfish.Virtu.Buttons.KeyQ;
			if (Controller["R"]) ret |= Jellyfish.Virtu.Buttons.KeyR;
			if (Controller["S"]) ret |= Jellyfish.Virtu.Buttons.KeyS;
			if (Controller["T"]) ret |= Jellyfish.Virtu.Buttons.KeyT;
			if (Controller["U"]) ret |= Jellyfish.Virtu.Buttons.KeyU;
			if (Controller["V"]) ret |= Jellyfish.Virtu.Buttons.KeyV;
			if (Controller["W"]) ret |= Jellyfish.Virtu.Buttons.KeyW;
			if (Controller["X"]) ret |= Jellyfish.Virtu.Buttons.KeyX;
			if (Controller["Y"]) ret |= Jellyfish.Virtu.Buttons.KeyY;
			if (Controller["Z"]) ret |= Jellyfish.Virtu.Buttons.KeyZ;

			return ret;
		}
	}
}
