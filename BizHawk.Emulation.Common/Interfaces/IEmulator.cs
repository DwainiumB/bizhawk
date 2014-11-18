﻿using System;
using System.Collections.Generic;
using System.IO;

namespace BizHawk.Emulation.Common
{
	public interface IEmulator : IDisposable
	{
		/// <summary>
		/// Video provider to the client
		/// </summary>
		IVideoProvider VideoProvider { get; }
		
		/// <summary>
		/// Sound provider for async operation.  this is optional, and is only required after StartAsyncSound() is called and returns true
		/// </summary>
		ISoundProvider SoundProvider { get; }
		
		/// <summary>
		/// sound provider for sync operation.  this is manditory
		/// </summary>
		ISyncSoundProvider SyncSoundProvider { get; }
		
		/// <summary>start async operation.  (on construct, sync operation is assumed).</summary>
		/// <returns>false if core doesn't support async sound; SyncSoundProvider will continue to be used in that case</returns>
		bool StartAsyncSound();
		/// <summary>
		/// end async operation, returning to sync operation.  after this, all sound requests will go to the SyncSoundProvider
		/// </summary>
		void EndAsyncSound();

		/// <summary>
		/// Defines all the possible inputs and types that the core can receive
		/// </summary>
		ControllerDefinition ControllerDefinition { get; }
		IController Controller { get; set; }

		/// <summary>
		// note that (some?) cores expect you to call SoundProvider.GetSamples() after each FrameAdvance()
		// please do this, even when rendersound = false
		/// <summary>
		/// </summary>
		void FrameAdvance(bool render, bool rendersound = true);

		/// <summary>
		/// The frame count
		/// </summary>
		int Frame { get; }

		/// <summary>
		/// The lag count.
		/// </summary>
		int LagCount { get; set; }

		/// <summary>
		/// If the current frame is a lag frame.
		/// All cores should define it the same, a lag frame is a frame in which input was not polled.
		/// </summary>
		bool IsLagFrame { get; }

		/// <summary>
		/// The unique Id of the given core, for instance "NES"
		/// </summary>
		string SystemId { get; }

		/// <summary>
		/// This flag is a contract with the client.  
		/// If true, the core agrees to behave in a completely deterministic manner,
		/// Features like movie recording depend on this.
		/// It is the client's responsibility to manage this flag.
		/// If a core wants to implement non-deterministic features (like speed hacks, frame-skipping), it must be done only when this flag is false
		/// if you want to set this, look in the emulator's constructor or Load() method
		/// </summary>
		bool DeterministicEmulation { get; }

		/// <summary>
		/// identifying information about a "mapper" or similar capability.  null if no such useful distinction can be drawn
		/// </summary>
		string BoardName { get; }

		/// <summary>
		/// return a copy of the saveram.  editing it won't do you any good unless you later call StoreSaveRam()
		/// </summary>
		byte[] CloneSaveRam();

		/// <summary>
		/// store new saveram to the emu core.  the data should be the same size as the return from ReadSaveRam()
		/// </summary>
		void StoreSaveRam(byte[] data);

		/// <summary>
		/// reset saveram to a standard initial state.  this probably shouldn't be used; instantiate a new core to clear persistent things.
		/// </summary>
		void ClearSaveRam();

		/// <summary>
		/// Whether or not Save ram has been modified since the last save
		/// </summary>
		bool SaveRamModified { get; set; }

		/// <summary>
		/// Resets the Frame and Lag counters, and any other similar counters a core might implement
		/// </summary>
		void ResetCounters();

		/// <summary>
		/// Savestate handling methods
		/// </summary>
		/// <param name="writer"></param>
		void SaveStateText(TextWriter writer);
		void LoadStateText(TextReader reader);
		void SaveStateBinary(BinaryWriter writer);
		void LoadStateBinary(BinaryReader reader);
		/// <summary>
		/// save state binary to a byte buffer
		/// </summary>
		/// <returns>you may NOT modify this.  if you call SaveStateBinary() again with the same core, the old data MAY be overwritten.</returns>
		byte[] SaveStateBinary();

		/// <summary>
		/// true if the core would rather give a binary savestate than a text one.  both must function regardless
		/// </summary>
		bool BinarySaveStatesPreferred { get; }

		/// <summary>
		/// the corecomm module in use by this core.
		/// </summary>
		CoreComm CoreComm { get; }
	}

	public interface IDebuggable : IEmulator
	{
		/// <summary>
		/// Returns a list of Cpu registers and their current state
		/// </summary>
		/// <returns></returns>
		Dictionary<string, int> GetCpuFlagsAndRegisters();

		/// <summary>
		/// Sets a given Cpu register to the given value
		/// </summary>
		/// <param name="register"></param>
		/// <param name="value"></param>
		void SetCpuRegister(string register, int value);
	}

	public enum DisplayType { NTSC, PAL, DENDY }
}
