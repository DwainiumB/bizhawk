﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace BizHawk.Emulation.Computers.Commodore64
{
	public partial class  C64 : IEmulator
	{
		private uint cyclesPerFrame;
		private string extension;
		private byte[] inputFile;

		public C64(CoreComm comm, GameInfo game, byte[] rom, string romextension)
		{
			CoreComm = comm;
			inputFile = rom;
			extension = romextension;
			Init(Region.PAL);
			cyclesPerFrame = (uint)board.vic.CyclesPerFrame;
			CoreComm.UsesDriveLed = true;
			SetupMemoryDomains();
		}

		// internal variables
		private bool _islag = true;
		private int _lagcount = 0;
		private int _frame = 0;

		// bizhawk I/O
		public CoreComm CoreComm { get; private set; }
		
		// game/rom specific
		public GameInfo game;
		public string SystemId { get { return "C64"; } }

		// memory domains
		public MemoryDomain MainMemory { get { return memoryDomains[0]; } }
		private IList<MemoryDomain> memoryDomains;
		public IList<MemoryDomain> MemoryDomains { get { return memoryDomains; } }

		// running state
		public bool DeterministicEmulation { get { return true; } set { ; } }
		public int Frame { get { return _frame; } set { _frame = value; } }
		public bool IsLagFrame { get { return _islag; } }
		public int LagCount { get { return _lagcount; } set { _lagcount = value; } }
		public void ResetFrameCounter()
		{
			_frame = 0;
			_lagcount = 0;
			_islag = false;
		}

		// audio/video
		public void EndAsyncSound() { } //TODO
		public ISoundProvider SoundProvider { get { return null; } }
		public bool StartAsyncSound() { return false; } //TODO
		public ISyncSoundProvider SyncSoundProvider { get { return board.sid.resampler; } }
		public IVideoProvider VideoProvider { get { return board.vic; } }

		// controller
		public ControllerDefinition ControllerDefinition { get { return C64ControllerDefinition; } }
		public IController Controller { get { return board.controller; } set { board.controller = value; } }
		public static readonly ControllerDefinition C64ControllerDefinition = new ControllerDefinition
		{
			Name = "Commodore 64 Controller", //TODO
			BoolButtons =
			{
				"Key Insert/Delete", "Key Return", "Key Cursor Left/Right", "Key F7", "Key F1", "Key F3", "Key F5", "Key Cursor Up/Down",
				"Key 3", "Key W", "Key A", "Key 4", "Key Z", "Key S", "Key E", "Key Left Shift",
				"Key 5", "Key R", "Key D", "Key 6", "Key C", "Key F", "Key T", "Key X",
				"Key 7", "Key Y", "Key G", "Key 8", "Key B", "Key H", "Key U", "Key V",
				"Key 9", "Key I", "Key J", "Key 0", "Key M", "Key K", "Key O", "Key N",
				"Key Plus", "Key P", "Key L", "Key Minus", "Key Period", "Key Colon", "Key At", "Key Comma",
				"Key Pound", "Key Asterisk", "Key Semicolon", "Key Clear/Home", "Key Right Shift", "Key Equal", "Key Up Arrow", "Key Slash",
				"Key 1", "Key Left Arrow", "Key Control", "Key 2", "Key Space", "Key Commodore", "Key Q", "Key Run/Stop",
				"P1 Up", "P1 Down", "P1 Left", "P1 Right", "P1 Button",
				"P2 Up", "P2 Down", "P2 Left", "P2 Right", "P2 Button",
				"Key Restore", "Key Lck"
			}
		};

		// framework
		public void Dispose()
		{
			if (board.sid != null)
			{
				board.sid.Dispose();
				board.sid = null;
			}
		}

		// process frame
		public void FrameAdvance(bool render, bool rendersound)
		{
			// load PRG file if needed
			if (loadPrg)
			{
				if (board.pla.Peek(0x04C8) == 0x12 &&
					board.pla.Peek(0x04C9) == 0x05 &&
					board.pla.Peek(0x04CA) == 0x01 &&
					board.pla.Peek(0x04CB) == 0x04 &&
					board.pla.Peek(0x04CC) == 0x19 &&
					board.pla.Peek(0x04CD) == 0x2E)
				{
					Media.PRG.Load(board.pla, inputFile);
					loadPrg = false;
				}
			}

			board.PollInput();
			for (uint count = cyclesPerFrame; count > 0; count--)
			{
				disk.Execute();
				board.Execute();
			}
			_islag = !board.inputRead;

			if (_islag)
				LagCount++;
			_frame++;

			Console.WriteLine("CPUPC: " + C64Util.ToHex(board.cpu.PC, 4) + " 1541PC: " + C64Util.ToHex(disk.PC, 4));

			CoreComm.DriveLED = DriveLED;
		}

		private void HandleFirmwareError(string file)
		{
			System.Windows.Forms.MessageBox.Show("the C64 core is referencing a firmware file which could not be found. Please make sure it's in your configured C64 firmwares folder. The referenced filename is: " + file);
			throw new FileNotFoundException();
		}

		public byte[] SaveStateBinary()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			SaveStateBinary(bw);
			bw.Flush();
			return ms.ToArray();
		}

		private void SetupMemoryDomains()
		{
			// chips must be initialized before this code runs!
			var domains = new List<MemoryDomain>(1);
			domains.Add(new MemoryDomain("System Bus", 0x10000, Endian.Little, new Func<int, byte>(board.cpu.Peek), new Action<int, byte>(board.cpu.Poke)));
			domains.Add(new MemoryDomain("RAM", 0x10000, Endian.Little, new Func<int, byte>(board.ram.Peek), new Action<int, byte>(board.ram.Poke)));
			domains.Add(new MemoryDomain("CIA0", 0x10, Endian.Little, new Func<int, byte>(board.cia0.Peek), new Action<int, byte>(board.cia0.Poke)));
			domains.Add(new MemoryDomain("CIA1", 0x10, Endian.Little, new Func<int, byte>(board.cia1.Peek), new Action<int, byte>(board.cia1.Poke)));
			domains.Add(new MemoryDomain("VIC", 0x40, Endian.Little, new Func<int, byte>(board.vic.Peek), new Action<int, byte>(board.vic.Poke)));
			domains.Add(new MemoryDomain("SID", 0x20, Endian.Little, new Func<int, byte>(board.sid.Peek), new Action<int, byte>(board.sid.Poke)));
			domains.Add(new MemoryDomain("1541 Bus", 0x10000, Endian.Little, new Func<int, byte>(disk.Peek), new Action<int, byte>(disk.Poke)));
			domains.Add(new MemoryDomain("1541 VIA0", 0x10, Endian.Little, new Func<int, byte>(disk.PeekVia0), new Action<int, byte>(disk.PokeVia0)));
			domains.Add(new MemoryDomain("1541 VIA1", 0x10, Endian.Little, new Func<int, byte>(disk.PeekVia1), new Action<int, byte>(disk.PokeVia1)));
			domains.Add(new MemoryDomain("1541 RAM", 0x1000, Endian.Little, new Func<int, byte>(disk.PeekRam), new Action<int, byte>(disk.PokeRam)));
			memoryDomains = domains.AsReadOnly();
		}
	}
}
