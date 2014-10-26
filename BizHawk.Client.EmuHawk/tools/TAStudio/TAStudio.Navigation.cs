﻿using System.Linq;
using System.IO;

using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class TAStudio
	{
		/// <summary>
		/// Only goes to go to the frame if it is an event before current emulation, otherwise it is just a future event that can freely be edited
		/// </summary>
		private void GoToLastEmulatedFrameIfNecessary(int frame)
		{
			if (frame != Global.Emulator.Frame) // Don't go to a frame if you are already on it!
			{
				var restoreFrame = Global.Emulator.Frame;

				if (frame <= Global.Emulator.Frame)
				{
					GoToFrame(frame);
				}

				_autoRestoreFrame = restoreFrame;
			}
		}

		private void GoToFrame(int frame)
		{
			// If past greenzone, emulate and capture states
			// If past greenzone AND movie, record input and capture states
			// If in greenzone, loadstate
			// If near a greenzone item, load and emulate
			// Do capturing and recording as needed

			if (frame < CurrentTasMovie.InputLogLength)
			{
				if (frame < Global.Emulator.Frame) // We are rewinding
				{
					var goToFrame = frame == 0 ? 0 : frame - 1;

					if (CurrentTasMovie[goToFrame].HasState) // Go back 1 frame and emulate to get the display (we don't store that)
					{
						CurrentTasMovie.SwitchToPlay();
						LoadState(CurrentTasMovie[goToFrame].State);

						if (frame > 0) // We can't emulate up to frame 0!
						{
							GlobalWin.MainForm.FrameAdvance();
						}

						GlobalWin.DisplayManager.NeedsToPaint = true;
						SetVisibleIndex(frame);
					}
					else // Get as close as we can then emulate there
					{
						StartAtNearestFrameAndEmulate(frame);
						return;
					}
				}
				else // We are going foward
				{
					if (frame == Global.Emulator.Frame + 1) // Just emulate a frame we only have 1 to go!
					{
						GlobalWin.MainForm.FrameAdvance();
					}
					else
					{
						var goToFrame = frame == 0 ? 0 : frame - 1;
						if (CurrentTasMovie[goToFrame].HasState) // Can we go directly there?
						{
							CurrentTasMovie.SwitchToPlay();
							LoadState(CurrentTasMovie[goToFrame].State);
							Global.Emulator.FrameAdvance(true);
							GlobalWin.DisplayManager.NeedsToPaint = true;

							SetVisibleIndex(frame);
						}
						else
						{
							StartAtNearestFrameAndEmulate(frame);
							return;
						}
					}
				}
			}
			else // Emulate to a future frame
			{
				if (frame == Global.Emulator.Frame + 1) // We are at the end of the movie and advancing one frame, therefore we are recording, simply emulate a frame
				{
					GlobalWin.MainForm.FrameAdvance();
				}
				else
				{
					// TODO: get the last greenzone frame and go there
					CurrentTasMovie.SwitchToPlay();

					// no reason to loadstate when we can emulate a frame instead
					var shouldLoadstate = frame - Global.Emulator.Frame != 1;

					if (CurrentTasMovie.TasStateManager.LastEmulatedFrame > 0 && shouldLoadstate)
					{
						LoadState(CurrentTasMovie[CurrentTasMovie.TasStateManager.LastEmulatedFrame].State);
					}

					if (frame != Global.Emulator.Frame) // If we aren't already at our destination, seek
					{
						GlobalWin.MainForm.UnpauseEmulator();
						if (Global.Config.TAStudioAutoPause && frame < CurrentTasMovie.InputLogLength)
						{
							GlobalWin.MainForm.PauseOnFrame = CurrentTasMovie.InputLogLength;
						}
						else
						{
							GlobalWin.MainForm.PauseOnFrame = frame;
						}
					}
				}
			}

			RefreshDialog();
			UpdateOtherTools();
		}

		public void GoToPreviousFrame()
		{
			if (Global.Emulator.Frame > 0)
			{
				GoToFrame(Global.Emulator.Frame - 1);
			}
		}

		public void GoToNextFrame()
		{
			GoToFrame(Global.Emulator.Frame + 1);
		}

		public void GoToPreviousMarker()
		{
			if (Global.Emulator.Frame > 0)
			{
				var prevMarker = CurrentTasMovie.Markers.Previous(Global.Emulator.Frame);
				var prev = prevMarker != null ? prevMarker.Frame : 0;
				GoToFrame(prev);
			}
		}

		public void GoToNextMarker()
		{
			var nextMarker = CurrentTasMovie.Markers.Next(Global.Emulator.Frame);
			var next = nextMarker != null ? nextMarker.Frame : CurrentTasMovie.InputLogLength - 1;
			GoToFrame(next);
		}

		public void GoToMarker(TasMovieMarker marker)
		{
			GoToFrame(marker.Frame);
		}
	}
}
