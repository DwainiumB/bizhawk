﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BizHawk.Emulation.Common;
using BizHawk.Emulation.Common.IEmulatorExtensions;

using BizHawk.Client.Common;
using BizHawk.Client.Common.MovieConversionExtensions;
using BizHawk.Client.EmuHawk.ToolExtensions;
using BizHawk.Client.EmuHawk.WinFormExtensions;

namespace BizHawk.Client.EmuHawk
{
	public partial class TAStudio
	{
		#region File Menu

		private void FileSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			ToBk2MenuItem.Enabled =
				!string.IsNullOrWhiteSpace(CurrentTasMovie.Filename) &&
				(CurrentTasMovie.Filename != DefaultTasProjName());

			SaveTASMenuItem.Enabled =
				!string.IsNullOrWhiteSpace(CurrentTasMovie.Filename) &&
				(CurrentTasMovie.Filename != DefaultTasProjName());
		}

		private void RecentSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			RecentSubMenu.DropDownItems.Clear();
			RecentSubMenu.DropDownItems.AddRange(
				Settings.RecentTas.RecentMenu(DummyLoadProject, true));
		}

		private void NewTasMenuItem_Click(object sender, EventArgs e)
		{
			if (GlobalWin.MainForm.GameIsClosing)
			{
				Close();
			}
			else
			{
				StartNewTasMovie();
			}
		}

		private void OpenTasMenuItem_Click(object sender, EventArgs e)
		{
			if (AskSaveChanges())
			{
				var filename = CurrentTasMovie.Filename;
				if (string.IsNullOrWhiteSpace(filename) || filename == DefaultTasProjName())
				{
					filename = "";
				}

				var file = ToolHelpers.GetTasProjFileFromUser(filename);
				if (file != null)
				{
					LoadFile(file);
				}
			}
		}

		private bool _exiting = false;

		private void SaveTasMenuItem_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(CurrentTasMovie.Filename) ||
				CurrentTasMovie.Filename == DefaultTasProjName())
			{
				SaveAsTasMenuItem_Click(sender, e);
			}
			else
			{
				if (_exiting)
				{
					CurrentTasMovie.Save();
				}
				else
				{
					_saveBackgroundWorker.RunWorkerAsync();
				}
				Settings.RecentTas.Add(CurrentTasMovie.Filename);
			}
		}

		private void SaveAsTasMenuItem_Click(object sender, EventArgs e)
		{
			var filename = CurrentTasMovie.Filename;
			if (string.IsNullOrWhiteSpace(filename) || filename == DefaultTasProjName())
			{
				filename = SuggestedTasProjName();
			}

			var file = ToolHelpers.GetTasProjSaveFileFromUser(filename);
			if (file != null)
			{
				CurrentTasMovie.Filename = file.FullName;

				if (_exiting)
				{
					CurrentTasMovie.Save();
				}
				else
				{
					_saveBackgroundWorker.RunWorkerAsync();
				}

				Settings.RecentTas.Add(CurrentTasMovie.Filename);
				SetTextProperty();
			}
		}

		private void ToBk2MenuItem_Click(object sender, EventArgs e)
		{
			var bk2 = CurrentTasMovie.ToBk2(true);
			bk2.Save();
			MessageStatusLabel.Text = Path.GetFileName(bk2.Filename) + " created.";

		}

		private void ExitMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion

		#region Edit

		private void UndoMenuItem_Click(object sender, EventArgs e)
		{
			if (CurrentTasMovie.ChangeLog.Undo() < Emulator.Frame)
				GoToFrame(CurrentTasMovie.ChangeLog.PreviousUndoFrame);
			else
				RefreshDialog();

			// Currently I don't have a way to easily detect when CanUndo changes, so this button should be enabled always.
			//UndoMenuItem.Enabled = CurrentTasMovie.ChangeLog.CanUndo;
			RedoMenuItem.Enabled = CurrentTasMovie.ChangeLog.CanRedo;
		}

		private void RedoMenuItem_Click(object sender, EventArgs e)
		{
			if (CurrentTasMovie.ChangeLog.Redo() < Emulator.Frame)
				GoToFrame(CurrentTasMovie.ChangeLog.PreviousRedoFrame);
			else
				RefreshDialog();

			//UndoMenuItem.Enabled = CurrentTasMovie.ChangeLog.CanUndo;
			RedoMenuItem.Enabled = CurrentTasMovie.ChangeLog.CanRedo;
		}

		private void showUndoHistoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			undoForm = new UndoHistoryForm(this);
			undoForm.Show();
			undoForm.UpdateValues();
		}

		private void EditSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			DeselectMenuItem.Enabled =
			SelectBetweenMarkersMenuItem.Enabled =
			CopyMenuItem.Enabled =
			CutMenuItem.Enabled =
			ClearMenuItem.Enabled =
			DeleteFramesMenuItem.Enabled =
			CloneMenuItem.Enabled =
			TruncateMenuItem.Enabled =
				TasView.SelectedRows.Any();
			ReselectClipboardMenuItem.Enabled =
				PasteMenuItem.Enabled =
				PasteInsertMenuItem.Enabled =
				_tasClipboard.Any();

			ClearGreenzoneMenuItem.Enabled =
				CurrentTasMovie != null && CurrentTasMovie.TasStateManager.Any();

			GreenzoneICheckSeparator.Visible =
				GreenZoneIntegrityCheckMenuItem.Visible =
				VersionInfo.DeveloperBuild;
		}

		private void DeselectMenuItem_Click(object sender, EventArgs e)
		{
			TasView.DeselectAll();
			RefreshTasView();
		}

		private void SelectAllMenuItem_Click(object sender, EventArgs e)
		{
			TasView.SelectAll();
			RefreshTasView();
		}

		private void SelectBetweenMarkersMenuItem_Click(object sender, EventArgs e)
		{
			if (TasView.SelectedRows.Any())
			{
				var prevMarker = CurrentTasMovie.Markers.PreviousOrCurrent(TasView.LastSelectedIndex.Value);
				var nextMarker = CurrentTasMovie.Markers.Next(TasView.LastSelectedIndex.Value);

				int prev = prevMarker != null ? prevMarker.Frame : 0;
				int next = nextMarker != null ? nextMarker.Frame : CurrentTasMovie.InputLogLength;

				for (int i = prev; i < next; i++)
				{
					TasView.SelectRow(i, true);
				}
			}
		}

		private void ReselectClipboardMenuItem_Click(object sender, EventArgs e)
		{
			TasView.DeselectAll();
			foreach (var item in _tasClipboard)
			{
				TasView.SelectRow(item.Frame, true);
			}
		}

		private void CopyMenuItem_Click(object sender, EventArgs e)
		{
			if (TasView.SelectedRows.Any())
			{
				_tasClipboard.Clear();
				var list = TasView.SelectedRows.ToList();
				var sb = new StringBuilder();

				foreach (var index in list)
				{
					var input = CurrentTasMovie.GetInputState(index);
					_tasClipboard.Add(new TasClipboardEntry(index, input));
					var lg = CurrentTasMovie.LogGeneratorInstance();
					lg.SetSource(input);
					sb.AppendLine(lg.GenerateLogEntry());
				}

				Clipboard.SetDataObject(sb.ToString());
				SetSplicer();
			}
		}

		private void PasteMenuItem_Click(object sender, EventArgs e)
		{
			// TODO: if highlighting 2 rows and pasting 3, only paste 2 of them
			// FCEUX Taseditor does't do this, but I think it is the expected behavior in editor programs

			var wasPaused = GlobalWin.MainForm.EmulatorPaused;

			if (_tasClipboard.Any())
			{
				var needsToRollback = TasView.FirstSelectedIndex < Emulator.Frame;

				CurrentTasMovie.CopyOverInput(TasView.FirstSelectedIndex.Value, _tasClipboard.Select(x => x.ControllerState));

				if (needsToRollback)
				{
					GoToLastEmulatedFrameIfNecessary(TasView.FirstSelectedIndex.Value);
					if (wasPaused)
					{
						DoAutoRestore();
					}
					else
					{
						GlobalWin.MainForm.UnpauseEmulator();
					}
				}
				else
				{
					RefreshDialog();
				}
			}
		}

		private void PasteInsertMenuItem_Click(object sender, EventArgs e)
		{
			var wasPaused = GlobalWin.MainForm.EmulatorPaused;

			if (_tasClipboard.Any())
			{
				var needsToRollback = TasView.FirstSelectedIndex < Emulator.Frame;

				CurrentTasMovie.InsertInput(TasView.FirstSelectedIndex.Value, _tasClipboard.Select(x => x.ControllerState));

				if (needsToRollback)
				{
					GoToLastEmulatedFrameIfNecessary(TasView.FirstSelectedIndex.Value);
					if (wasPaused)
					{
						DoAutoRestore();
					}
					else
					{
						GlobalWin.MainForm.UnpauseEmulator();
					}
				}
				else
				{
					RefreshDialog();
				}
			}
		}

		private void CutMenuItem_Click(object sender, EventArgs e)
		{
			if (TasView.SelectedRows.Any())
			{
				var wasPaused = GlobalWin.MainForm.EmulatorPaused;
				var needsToRollback = TasView.FirstSelectedIndex < Emulator.Frame;
				var rollBackFrame = TasView.FirstSelectedIndex.Value;

				_tasClipboard.Clear();
				var list = TasView.SelectedRows.ToArray();
				var sb = new StringBuilder();
				for (var i = 0; i < list.Length; i++)
				{
					var input = CurrentTasMovie.GetInputState(i);
					_tasClipboard.Add(new TasClipboardEntry(list[i], input));
					var lg = CurrentTasMovie.LogGeneratorInstance();
					lg.SetSource(input);
					sb.AppendLine(lg.GenerateLogEntry());
				}

				Clipboard.SetDataObject(sb.ToString());
				CurrentTasMovie.RemoveFrames(list);
				SetSplicer();
				TasView.DeselectAll();

				if (needsToRollback)
				{
					GoToLastEmulatedFrameIfNecessary(rollBackFrame);
					if (wasPaused)
					{
						DoAutoRestore();
					}
					else
					{
						GlobalWin.MainForm.UnpauseEmulator();
					}
				}
				else
				{
					RefreshDialog();
				}
			}
		}

		private void ClearMenuItem_Click(object sender, EventArgs e)
		{
			if (TasView.SelectedRows.Any())
			{
				var wasPaused = GlobalWin.MainForm.EmulatorPaused;
				var needsToRollback = !(TasView.FirstSelectedIndex > Emulator.Frame);
				var rollBackFrame = TasView.FirstSelectedIndex.Value;

				foreach (var frame in TasView.SelectedRows)
				{
					CurrentTasMovie.ClearFrame(frame);
				}

				if (needsToRollback)
				{
					GoToLastEmulatedFrameIfNecessary(rollBackFrame);
					if (wasPaused)
					{
						DoAutoRestore();
					}
					else
					{
						GlobalWin.MainForm.UnpauseEmulator();
					}
				}
				else
				{
					RefreshDialog();
				}
			}
		}

		private void DeleteFramesMenuItem_Click(object sender, EventArgs e)
		{
			if (TasView.SelectedRows.Any())
			{
				var wasPaused = GlobalWin.MainForm.EmulatorPaused;
				var needsToRollback = TasView.FirstSelectedIndex < Emulator.Frame;
				var rollBackFrame = TasView.FirstSelectedIndex.Value;
				if (rollBackFrame >= CurrentTasMovie.InputLogLength)
				{ // Cannot delete non-existant frames
					RefreshDialog();
					return;
				}

				CurrentTasMovie.RemoveFrames(TasView.SelectedRows.ToArray());
				SetSplicer();

				if (needsToRollback)
				{
					GoToLastEmulatedFrameIfNecessary(rollBackFrame);
					if (wasPaused)
					{
						DoAutoRestore();
					}
					else
					{
						GlobalWin.MainForm.UnpauseEmulator();
					}
				}
				else
				{
					RefreshDialog();
				}
			}
		}

		private void CloneMenuItem_Click(object sender, EventArgs e)
		{
			if (TasView.SelectedRows.Any())
			{
				var wasPaused = GlobalWin.MainForm.EmulatorPaused;
				var framesToInsert = TasView.SelectedRows.ToList();
				var insertionFrame = Math.Min(TasView.LastSelectedIndex.Value + 1, CurrentTasMovie.InputLogLength);
				var needsToRollback = TasView.FirstSelectedIndex < Emulator.Frame;

				var inputLog = framesToInsert
					.Select(frame => CurrentTasMovie.GetInputLogEntry(frame))
					.ToList();

				CurrentTasMovie.InsertInput(insertionFrame, inputLog);

				if (needsToRollback)
				{
					GoToLastEmulatedFrameIfNecessary(insertionFrame);
					if (wasPaused)
					{
						DoAutoRestore();
					}
					else
					{
						GlobalWin.MainForm.UnpauseEmulator();
					}
				}
				else
				{
					RefreshDialog();
				}
			}
		}

		private void InsertFrameMenuItem_Click(object sender, EventArgs e)
		{
			var wasPaused = GlobalWin.MainForm.EmulatorPaused;
			var insertionFrame = TasView.SelectedRows.Any() ? TasView.FirstSelectedIndex.Value : 0;
			var needsToRollback = TasView.FirstSelectedIndex < Emulator.Frame;

			CurrentTasMovie.InsertEmptyFrame(insertionFrame);

			if (needsToRollback)
			{
				GoToLastEmulatedFrameIfNecessary(insertionFrame);
				if (wasPaused)
				{
					DoAutoRestore();
				}
				else
				{
					GlobalWin.MainForm.UnpauseEmulator();
				}
			}
			else
			{
				RefreshDialog();
			}
		}

		private void InsertNumFramesMenuItem_Click(object sender, EventArgs e)
		{
			var wasPaused = GlobalWin.MainForm.EmulatorPaused;
			var insertionFrame = TasView.SelectedRows.Any() ? TasView.FirstSelectedIndex.Value : 0;
			var needsToRollback = TasView.FirstSelectedIndex < Emulator.Frame;

			var framesPrompt = new FramesPrompt();
			var result = framesPrompt.ShowDialog();
			if (result == DialogResult.OK)
			{
				CurrentTasMovie.InsertEmptyFrame(insertionFrame, framesPrompt.Frames);
			}

			if (needsToRollback)
			{
				GoToLastEmulatedFrameIfNecessary(insertionFrame);
				if (wasPaused)
				{
					DoAutoRestore();
				}
				else
				{
					GlobalWin.MainForm.UnpauseEmulator();
				}
			}
			else
			{
				RefreshDialog();
			}
		}

		private void TruncateMenuItem_Click(object sender, EventArgs e)
		{
			if (TasView.SelectedRows.Any())
			{
				var rollbackFrame = TasView.LastSelectedIndex.Value;
				var needsToRollback = TasView.FirstSelectedIndex < Emulator.Frame;

				CurrentTasMovie.Truncate(rollbackFrame);
				MarkerControl.MarkerInputRoll.TruncateSelection(CurrentTasMovie.Markers.Count - 1);

				if (needsToRollback)
				{
					GoToFrame(rollbackFrame);
				}
				else
				{
					RefreshDialog();
				}
			}
		}

		private void SetMarkersMenuItem_Click(object sender, EventArgs e)
		{
			foreach (var index in TasView.SelectedRows)
			{
				CallAddMarkerPopUp(index);
			}
		}

		private void RemoveMarkersMenuItem_Click(object sender, EventArgs e)
		{
			IEnumerable<TasMovieMarker> markers = CurrentTasMovie.Markers.Where(m => TasView.SelectedRows.Contains(m.Frame));
			foreach (TasMovieMarker m in markers)
			{
				RemoveMarker(m);
			}
			RefreshDialog();
		}

		private void ClearGreenzoneMenuItem_Click(object sender, EventArgs e)
		{
			CurrentTasMovie.ClearGreenzone();
			RefreshDialog();
		}

		private void GreenZzoneIntegrityCheckMenuItem_Click(object sender, EventArgs e)
		{
			GlobalWin.MainForm.RebootCore();
			GlobalWin.MainForm.FrameAdvance();
			var frame = Emulator.Frame;

			if (CurrentTasMovie.TasStateManager.HasState(frame))
			{
				var state = (byte[])StatableEmulator.SaveStateBinary().Clone();
				var greenzone = CurrentTasMovie.TasStateManager[frame];

				if (!state.SequenceEqual(greenzone.Value))
				{
					MessageBox.Show("bad data at frame: " + frame);
					return;
				}
			}

			MessageBox.Show("Integrity Check passed");
		}

		#endregion

		#region Config

		private void SetMaxUndoLevelsMenuItem_Click(object sender, EventArgs e)
		{
			using (var prompt = new InputPrompt
			{
				TextInputType = InputPrompt.InputType.Unsigned,
				Message = "Number of Undo Levels to keep",
				InitialValue = CurrentTasMovie.ChangeLog.MaxSteps.ToString()
			})
			{
				DialogResult result = prompt.ShowDialog();
				if (result == DialogResult.OK)
				{
					int val = int.Parse(prompt.PromptText);
					if (val > 0)
						CurrentTasMovie.ChangeLog.MaxSteps = val;
				}
			}
		}

		private void ConfigSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			DrawInputByDraggingMenuItem.Checked = Settings.DrawInput;
			AutopauseAtEndOfMovieMenuItem.Checked = Settings.AutoPause;
			EmptyNewMarkerNotesMenuItem.Checked = Settings.EmptyMarkers;
		}

		private void DrawInputByDraggingMenuItem_Click(object sender, EventArgs e)
		{
			TasView.InputPaintingMode = Settings.DrawInput ^= true;
		}

		private void BindMarkersToInputMenuItem_Click(object sender, EventArgs e)
		{
			CurrentTasMovie.BindMarkersToInput = BindMarkersToInputMenuItem.Checked;
		}

		private void EmptyNewMarkerNotesMenuItem_Click(object sender, EventArgs e)
		{
			Settings.EmptyMarkers ^= true;
		}

		private void AutopauseAtEndMenuItem_Click(object sender, EventArgs e)
		{
			Settings.AutoPause ^= true;
		}

		#endregion

		#region Metadata

		private void HeaderMenuItem_Click(object sender, EventArgs e)
		{
			new MovieHeaderEditor(CurrentTasMovie)
			{
				Owner = GlobalWin.MainForm,
				Location = this.ChildPointToScreen(TasView)
			}.Show();
			UpdateChangesIndicator();
		}

		private void StateHistorySettingsMenuItem_Click(object sender, EventArgs e)
		{
			new StateHistorySettingsForm(CurrentTasMovie.TasStateManager.Settings)
			{
				Owner = GlobalWin.MainForm,
				Location = this.ChildPointToScreen(TasView),
				Statable = this.StatableEmulator
			}.Show();
			UpdateChangesIndicator();
		}

		private void CommentsMenuItem_Click(object sender, EventArgs e)
		{
			var form = new EditCommentsForm();
			form.GetMovie(CurrentTasMovie);
			form.ForceReadWrite = true;
			form.Show();
		}

		private void SubtitlesMenuItem_Click(object sender, EventArgs e)
		{
			var form = new EditSubtitlesForm { ReadOnly = false };
			form.GetMovie(Global.MovieSession.Movie);
			form.ShowDialog();
		}

		private void DefaultStateSettingsMenuItem_Click(object sender, EventArgs e)
		{
			new DefaultGreenzoneSettings
			{
				Location = this.ChildPointToScreen(TasView)
			}.ShowDialog();
		}

		#endregion

		#region Settings Menu

		private void SettingsSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			RotateMenuItem.ShortcutKeyDisplayString = TasView.RotateHotkeyStr;
		}

		private void HideLagFramesSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			HideLagFrames0.Checked = TasView.LagFramesToHide == 0;
			HideLagFrames1.Checked = TasView.LagFramesToHide == 1;
			HideLagFrames2.Checked = TasView.LagFramesToHide == 2;
			HideLagFrames3.Checked = TasView.LagFramesToHide == 3;
		}

		private void RotateMenuItem_Click(object sender, EventArgs e)
		{
			TasView.HorizontalOrientation ^= true;
			CurrentTasMovie.FlagChanges();
		}

		private void HideLagFramesX_Click(object sender, EventArgs e)
		{
			TasView.LagFramesToHide = (int)(sender as ToolStripMenuItem).Tag;
			if (TasPlaybackBox.FollowCursor)
			{
				SetVisibleIndex();
			}
			RefreshDialog();
		}

		#endregion

		#region Columns

		private void ColumnsSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			ColumnsSubMenu.DropDownItems.Clear();

			var columns = TasView.AllColumns
				.Where(x => !string.IsNullOrWhiteSpace(x.Text))
				.Where(x => x.Name != "FrameColumn");

			foreach (var column in columns)
			{
				var dummyColumnObject = column;

				var menuItem = new ToolStripMenuItem
				{
					Text = column.Text + " (" + column.Name + ")",
					Checked = column.Visible
				};

				menuItem.Click += (o, ev) =>
				{
					dummyColumnObject.Visible ^= true;
					TasView.AllColumns.ColumnsChanged();
					CurrentTasMovie.FlagChanges();
					RefreshTasView();
				};

				ColumnsSubMenu.DropDownItems.Add(menuItem);
			}

			ColumnsSubMenu.DropDownItems.Add(new ToolStripSeparator());
			var defaults = new ToolStripMenuItem
			{
				Name = "RestoreDefaultColumnConfiguration",
				Text = "Restore defaults"
			};

			defaults.Click += (o, ev) =>
			{
				TasView.AllColumns.Clear();
				SetUpColumns();
				RefreshTasView();
				CurrentTasMovie.FlagChanges();
			};

			ColumnsSubMenu.DropDownItems.Add(defaults);
		}

		#endregion

		#region Context Menu

		private void RightClickMenu_Opened(object sender, EventArgs e)
		{
			SetMarkersContextMenuItem.Enabled =
				SelectBetweenMarkersContextMenuItem.Enabled =
				RemoveMarkersContextMenuItem.Enabled =
				DeselectContextMenuItem.Enabled =
				ClearContextMenuItem.Enabled =
				DeleteFramesContextMenuItem.Enabled =
				CloneContextMenuItem.Enabled =
				InsertFrameContextMenuItem.Enabled =
				InsertNumFramesContextMenuItem.Enabled =
				TruncateContextMenuItem.Enabled =
				TasView.SelectedRows.Any();

			StartFromNowSeparator.Visible =
				StartNewProjectFromNowMenuItem.Visible =
				TasView.SelectedRows.Count() == 1 &&
				!CurrentTasMovie.StartsFromSavestate;

			RemoveMarkersContextMenuItem.Enabled = CurrentTasMovie.Markers.Any(m => TasView.SelectedRows.Contains(m.Frame)); // Disable the option to remove markers if no markers are selected (FCEUX does this).

			CancelSeekContextMenuItem.Enabled = GlobalWin.MainForm.PauseOnFrame.HasValue;
		}

		private void CancelSeekContextMenuItem_Click(object sender, EventArgs e)
		{
			GlobalWin.MainForm.PauseOnFrame = null;
			RefreshTasView();
		}

		private void StartNewProjectFromNowMenuItem_Click(object sender, EventArgs e)
		{
			if (TasView.SelectedRows.Count() == 1 &&
				!CurrentTasMovie.StartsFromSavestate)
			{
				if (AskSaveChanges())
				{
					var index = TasView.SelectedRows.First();
					GoToFrame(index);

					var newProject = CurrentTasMovie.ConvertToSavestateAnchoredMovie(
						index,
						(byte[])StatableEmulator.SaveStateBinary().Clone());

					GlobalWin.MainForm.PauseEmulator();
					LoadProject(newProject.Filename);
				}
			}
		}

		#endregion
	}
}
