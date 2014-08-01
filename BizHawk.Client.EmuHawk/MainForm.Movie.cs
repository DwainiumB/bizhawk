﻿using System;
using System.IO;

using BizHawk.Client.Common;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Consoles.Nintendo.QuickNES;
using BizHawk.Emulation.Cores.Nintendo.NES;
using BizHawk.Emulation.Cores.Nintendo.SNES9X;
using BizHawk.Emulation.Cores.Nintendo.SNES;

namespace BizHawk.Client.EmuHawk
{
	partial class MainForm
	{
		public void StartNewMovie(IMovie movie, bool record)
		{
			if (!record) // The semantics of record is that we are starting a new movie, and even wiping a pre-existing movie with the same path, but non-record means we are loading an existing movie into playback mode
			{
				movie.Load();
			}

			if (movie.SystemID != Global.Emulator.SystemId)
			{
				GlobalWin.OSD.AddMessage("Movie does not match the currently loaded system, unable to load");
				return;
			}

			//If a movie is already loaded, save it before starting a new movie
			if (Global.MovieSession.Movie.IsActive && !string.IsNullOrEmpty(Global.MovieSession.Movie.Filename))
			{
				Global.MovieSession.Movie.Save();
			}

			Global.MovieSession = new MovieSession
			{
				Movie = movie,
				MovieControllerAdapter = movie.LogGeneratorInstance().MovieControllerAdapter,
				MessageCallback = GlobalWin.OSD.AddMessage,
				AskYesNoCallback = StateErrorAskUser,
				PauseCallback = PauseEmulator,
				ModeChangedCallback = SetMainformMovieInfo
			};

			InputManager.RewireInputChain();

			if (!record)
			{
				Global.MovieSession.MovieLoad(); // TODO this loads it a 2nd time, ugh
			}

			try
			{
				if (!record && Global.Emulator.SystemId == "NES") // For NES we need special logic since the movie will drive which core to load
				{
					var quicknesName = ((CoreAttributes)Attribute.GetCustomAttribute(typeof(QuickNES), typeof(CoreAttributes))).CoreName;
					var neshawkName = ((CoreAttributes)Attribute.GetCustomAttribute(typeof(NES), typeof(CoreAttributes))).CoreName;

					// If either is specified use that, else use whatever is currently set
					if (Global.MovieSession.Movie.Core == quicknesName)
					{
						Global.Config.NES_InQuickNES = true;
					}
					else if (Global.MovieSession.Movie.Core == neshawkName)
					{
						Global.Config.NES_InQuickNES = false;
					}
				}
				else if (!record && Global.Emulator.SystemId == "SNES") // ditto with snes9x vs bsnes
				{
					var snes9xName = ((CoreAttributes)Attribute.GetCustomAttribute(typeof(Snes9x), typeof(CoreAttributes))).CoreName;
					var bsnesName = ((CoreAttributes)Attribute.GetCustomAttribute(typeof(LibsnesCore), typeof(CoreAttributes))).CoreName;

					if (Global.MovieSession.Movie.Core == snes9xName)
					{
						Global.Config.SNES_InSnes9x = true;
					}
					else
					{
						Global.Config.SNES_InSnes9x = false;
					}
				}

				var s = Global.MovieSession.Movie.SyncSettingsJson;
				if (!string.IsNullOrWhiteSpace(s))
				{
					_syncSettingsHack = ConfigService.LoadWithType(s);
				}

				if (record) // This is a hack really, we need to set the movie to its propert state so that it will be considered active later
				{
					Global.MovieSession.Movie.SwitchToRecord();
				}
				else
				{
					Global.MovieSession.Movie.SwitchToRecord();
				}

				LoadRom(GlobalWin.MainForm.CurrentlyOpenRom);
			}
			finally
			{
				// ensure subsequent calls to LoadRom won't get the settings object created here
				this._syncSettingsHack = null;
			}

			Global.Config.RecentMovies.Add(movie.Filename);

			if (Global.MovieSession.Movie.StartsFromSavestate)
			{
				if (Global.MovieSession.Movie.TextSavestate != null)
					Global.Emulator.LoadStateText(new StringReader(Global.MovieSession.Movie.TextSavestate));
				else
					Global.Emulator.LoadStateBinary(new BinaryReader(new MemoryStream(Global.MovieSession.Movie.BinarySavestate, false)));

				Global.Emulator.ResetCounters();
			}

			if (record)
			{
				Global.MovieSession.Movie.StartNewRecording();
				Global.MovieSession.ReadOnly = false;
			}
			else
			{
				Global.MovieSession.Movie.StartNewPlayback();
			}

			SetMainformMovieInfo();
			UpdateStatusSlots();

			GlobalWin.Tools.Restart<VirtualpadTool>();
			GlobalWin.DisplayManager.NeedsToPaint = true;
		}

		public void SetMainformMovieInfo()
		{
			if (Global.MovieSession.Movie.IsPlaying)
			{
				PlayRecordStatusButton.Image = Properties.Resources.Play;
				PlayRecordStatusButton.ToolTipText = "Movie is in playback mode";
				PlayRecordStatusButton.Visible = true;
			}
			else if (Global.MovieSession.Movie.IsRecording)
			{
				PlayRecordStatusButton.Image = Properties.Resources.RecordHS;
				PlayRecordStatusButton.ToolTipText = "Movie is in record mode";
				PlayRecordStatusButton.Visible = true;
			}
			else if (!Global.MovieSession.Movie.IsActive)
			{
				PlayRecordStatusButton.Image = Properties.Resources.Blank;
				PlayRecordStatusButton.ToolTipText = "No movie is active";
				PlayRecordStatusButton.Visible = false;
			}

			SetWindowText();
		}

		public void RestartMovie()
		{
			if (Global.MovieSession.Movie.IsActive)
			{
				StopMovieThenLoadRom(CurrentlyOpenRom);
				if (Global.MovieSession.Movie.StartsFromSavestate)
				{
					// TODO: why does this code exist twice??

					if (Global.MovieSession.Movie.TextSavestate != null)
					{
						Global.Emulator.LoadStateText(new StringReader(Global.MovieSession.Movie.TextSavestate));
					}
					else
					{
						Global.Emulator.LoadStateBinary(new BinaryReader(new MemoryStream(Global.MovieSession.Movie.BinarySavestate, false)));
					}

					//var state = Convert.FromBase64String(Global.MovieSession.Movie.SavestateBinaryBase64Blob);
					//Global.Emulator.LoadStateBinary(new BinaryReader(new MemoryStream(state)));
					Global.Emulator.ResetCounters();
				}

				Global.MovieSession.Movie.StartNewPlayback();
				SetMainformMovieInfo();
				GlobalWin.OSD.AddMessage("Replaying movie file in read-only mode");
				Global.MovieSession.ReadOnly = true;
			}
		}

		public void StopMovie(bool saveChanges = true)
		{
			if (IsSlave && _master.WantsToCOntrolStopMovie)
			{
				_master.StopMovie();
			}
			else
			{
				Global.MovieSession.StopMovie(saveChanges);
				SetMainformMovieInfo();
				UpdateStatusSlots();
			}
		}
	}
}
