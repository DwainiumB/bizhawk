﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BizHawk.MultiClient
{
	public partial class TAStudio : Form
	{
		//TODO:
		//If null emulator do a base virtualpad so getmnemonic doesn't fail
		//Right-click - Go to current frame
		//Clicking a frame should go there
		//Multiple timeline system
		//Macro listview
		//	Double click brings up a macro editing window
		//NES Controls virtualpad (Power-on & Reset, eventually FDS options)
		//SMS virtualpad
		//PCE virtualpad
		//Dynamic virtualpad system based on platform
		//ensureVisible when recording
        //Allow hotkeys when TAStudio has focus

		int defaultWidth;     //For saving the default size of the dialog, so the user can restore if desired
		int defaultHeight;
		
		public bool Engaged; //When engaged the Client will listen to TAStudio for input
		List<VirtualPad> Pads = new List<VirtualPad>();

		//Movie header object - to have the main project header data
		//List<string> MacroFiles - list of .macro files (simply log files)
		//List<string> TimeLines - list of movie files
		//List<string> Bookmarks - list of savestate files

		public TAStudio()
		{
			InitializeComponent();
			Closing += (o, e) => SaveConfigSettings();
			TASView.QueryItemText += new QueryItemTextHandler(TASView_QueryItemText);
			TASView.QueryItemBkColor += new QueryItemBkColorHandler(TASView_QueryItemBkColor);
			TASView.VirtualMode = true;
		}

		public void UpdateValues()
		{
			if (!this.IsHandleCreated || this.IsDisposed) return;
			TASView.BlazingFast = true;
			if (Global.MovieSession.Movie.Mode == MOVIEMODE.INACTIVE)
				TASView.ItemCount = 0;
			else
				DisplayList();

			if (Global.MovieSession.Movie.Mode == MOVIEMODE.PLAY)
			{
				string str = Global.MovieSession.Movie.GetInputFrame(Global.Emulator.Frame);
				if (Global.Config.TASUpdatePads == true && str != "")
				{
					switch (Global.Emulator.SystemId)
					{
						case "NES":
							Pads[0].SetButtons(str.Substring(3, 8));
							Pads[1].SetButtons(str.Substring(12, 8));
							Pads[2].SetButtons(str[1].ToString());
							break;
						case "SMS":
						case "GG":
						case "SG":
							Pads[0].SetButtons(str.Substring(0, 6));
							Pads[1].SetButtons(str.Substring(7, 6));
							Pads[2].SetButtons(str.Substring(14, 2));
							break;
						case "PCE":
						case "SGX":
							Pads[0].SetButtons(str.Substring(3, 8));
							Pads[1].SetButtons(str.Substring(12, 8));
							Pads[2].SetButtons(str.Substring(21, 8));
							Pads[3].SetButtons(str.Substring(30, 8));
							break;
						default:
							break;
					}
				}
				TASView.BlazingFast = false;
			}
		}

		public string GetMnemonic()
		{
			StringBuilder str = new StringBuilder("|"); //TODO: Control Command virtual pad
			
			//TODO: remove this hack with a nes controls pad 
			if (Global.Emulator.SystemId == "NES")
				str.Append("0|");

			for (int x = 0; x < Pads.Count; x++)
				str.Append(Pads[x].GetMnemonic());
			return str.ToString();
		}

		private void TASView_QueryItemBkColor(int index, int column, ref Color color)
		{
			if (index < Global.MainForm.RewindBufferCount())
				color = Color.LightGreen;
			else if (Global.MovieSession.Movie.GetInputFrame(index)[1] == 'L')
				color = Color.Pink;
		}

		private void TASView_QueryItemText(int index, int column, out string text)
		{
			text = "";
			if (column == 0)
				text = String.Format("{0:#,##0}", index);
			if (column == 1)
				text = Global.MovieSession.Movie.GetInputFrame(index);
		}

		private void DisplayList()
		{
			TASView.ItemCount = Global.MovieSession.Movie.Length();
            TASView.ensureVisible(Global.Emulator.Frame-1);
		}

		public void Restart()
		{
			if (!this.IsHandleCreated || this.IsDisposed) return;
			TASView.Items.Clear();
			ControllerBox.Controls.Clear();
			ClearPads();
			Pads.Clear();
			LoadTAStudio();
		}

		public void LoadTAStudio()
		{
			//TODO: don't engage until new/open project
			//
			Engaged = true;
			Global.OSD.AddMessage("TAStudio engaged");

			LoadConfigSettings();
			ReadOnlyCheckBox.Checked = Global.MainForm.ReadOnly;
			DisplayList();

			//Add virtual pads
			switch (Global.Emulator.SystemId)
			{
				case "NULL":
				default:
					break;
				case "A26":
					VirtualPadA26 ataripad1 = new VirtualPadA26();
					ataripad1.Location = new Point(8, 19);
					ataripad1.Controller = "P1";
					VirtualPadA26 ataripad2 = new VirtualPadA26();
					ataripad2.Location = new Point(188, 19);
					ataripad2.Controller = "P2";
					Pads.Add(ataripad1);
					Pads.Add(ataripad2);
					ControllerBox.Controls.Add(Pads[0]);
					ControllerBox.Controls.Add(Pads[1]);
					VirtualPadA26Control ataricontrols = new VirtualPadA26Control();
					ataricontrols.Location = new Point(8, 109);
					Pads.Add(ataricontrols);
					ControllerBox.Controls.Add(Pads[2]);
					break;
				case "NES":
					VirtualPadNES nespad1 = new VirtualPadNES();
					nespad1.Location = new Point(8, 19);
					nespad1.Controller = "P1";
					VirtualPadNES nespad2 = new VirtualPadNES();
					nespad2.Location = new Point(188, 19);
					nespad2.Controller = "P2";
					Pads.Add(nespad1);
					Pads.Add(nespad2);
					ControllerBox.Controls.Add(Pads[0]);
					ControllerBox.Controls.Add(Pads[1]);
					VirtualPadNESControl controlpad1 = new VirtualPadNESControl();
					controlpad1.Location = new Point(8, 109);
					Pads.Add(controlpad1);
					ControllerBox.Controls.Add(Pads[2]);
					break;
				case "SMS":
				case "SG":
				case "GG":
					VirtualPadSMS smspad1 = new VirtualPadSMS();
					smspad1.Location = new Point(8, 19);
					smspad1.Controller = "P1";
					VirtualPadSMS smspad2 = new VirtualPadSMS();
					smspad2.Location = new Point(188, 19);
					smspad2.Controller = "P2";
					Pads.Add(smspad1);
					Pads.Add(smspad2);
					ControllerBox.Controls.Add(Pads[0]);
					ControllerBox.Controls.Add(Pads[1]);
					VirtualPadSMSControl controlpad2 = new VirtualPadSMSControl();
					controlpad2.Location = new Point(8, 109);
					Pads.Add(controlpad2);
					ControllerBox.Controls.Add(Pads[2]);
					break;
				case "PCE":
					VirtualPadPCE pcepad1 = new VirtualPadPCE();
					pcepad1.Location = new Point(8, 19);
					pcepad1.Controller = "P1";
					VirtualPadPCE pcepad2 = new VirtualPadPCE();
					pcepad2.Location = new Point(188, 19);
					pcepad2.Controller = "P2";
					VirtualPadPCE pcepad3 = new VirtualPadPCE();
					pcepad3.Location = new Point(8, 109);
					pcepad3.Controller = "P3";
					VirtualPadPCE pcepad4 = new VirtualPadPCE();
					pcepad4.Location = new Point(188, 109);
					pcepad4.Controller = "P4";
					Pads.Add(pcepad1);
					Pads.Add(pcepad2);
					Pads.Add(pcepad3);
					Pads.Add(pcepad4);
					ControllerBox.Controls.Add(Pads[0]);
					ControllerBox.Controls.Add(Pads[1]);
					ControllerBox.Controls.Add(Pads[2]);
					ControllerBox.Controls.Add(Pads[3]);
					break;
			}
		}

		private void TAStudio_Load(object sender, EventArgs e)
		{
			if (!Global.MainForm.INTERIM)
			{
				newProjectToolStripMenuItem.Enabled = false;
				openProjectToolStripMenuItem.Enabled = false;
				saveProjectToolStripMenuItem.Enabled = false;
				saveProjectAsToolStripMenuItem.Enabled = false;
				recentToolStripMenuItem.Enabled = false;
				importTASFileToolStripMenuItem.Enabled = false;
				insertFrameToolStripMenuItem.Enabled = false;
			}

			LoadTAStudio();
		}

		private void LoadConfigSettings()
		{
			defaultWidth = Size.Width;     //Save these first so that the user can restore to its original size
			defaultHeight = Size.Height;

			if (Global.Config.TAStudioSaveWindowPosition && Global.Config.TASWndx >= 0 && Global.Config.TASWndy >= 0)
				this.Location = new Point(Global.Config.TASWndx, Global.Config.TASWndy);

			if (Global.Config.TASWidth >= 0 && Global.Config.TASHeight >= 0)
			{
				this.Size = new System.Drawing.Size(Global.Config.TASWidth, Global.Config.TASHeight);
			}

		}

		private void SaveConfigSettings()
		{
			Engaged = false;
			Global.Config.TASWndx = this.Location.X;
			Global.Config.TASWndy = this.Location.Y;
			Global.Config.TASWidth = this.Right - this.Left;
			Global.Config.TASHeight = this.Bottom - this.Top;
			ClearPads();
		}

		public void ClearPads()
		{
			for (int x = 0; x < Pads.Count; x++)
				Pads[x].Clear();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void settingsToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			saveWindowPositionToolStripMenuItem.Checked = Global.Config.TAStudioSaveWindowPosition;
			autoloadToolStripMenuItem.Checked = Global.Config.AutoloadTAStudio;
			updatePadsOnMovePlaybackToolStripMenuItem.Checked = Global.Config.TASUpdatePads;
		}

		private void saveWindowPositionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.TAStudioSaveWindowPosition ^= true;
		}

		private void restoreWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Size = new System.Drawing.Size(defaultWidth, defaultHeight);
		}

		private void StopButton_Click(object sender, EventArgs e)
		{
			Global.MainForm.StopMovie();
			Restart();
		}

		private void FrameAdvanceButton_Click(object sender, EventArgs e)
		{
			Global.MainForm.PressFrameAdvance = true;
		}

		private void RewindButton_Click(object sender, EventArgs e)
		{
			Global.MainForm.PressRewind = true;
		}

		private void PauseButton_Click(object sender, EventArgs e)
		{
			Global.MainForm.TogglePause();
		}

		private void autoloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.AutoloadTAStudio ^= true;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (ReadOnlyCheckBox.Checked)
			{
                Global.MovieSession.Movie.Mode = MOVIEMODE.PLAY;
				ReadOnlyCheckBox.BackColor = System.Drawing.SystemColors.Control;
				toolTip1.SetToolTip(this.ReadOnlyCheckBox, "Currently Read-Only Mode");
			}
			else
			{
                Global.MovieSession.Movie.Mode = MOVIEMODE.RECORD;
                ReadOnlyCheckBox.BackColor = Color.LightCoral;
				toolTip1.SetToolTip(this.ReadOnlyCheckBox, "Currently Read+Write Mode");
			}
		}


		private void RewindToBeginning_Click(object sender, EventArgs e)
		{
			Global.MainForm.Rewind(Global.Emulator.Frame);
			DisplayList();
		}

		private void FastForwardToEnd_Click(object sender, EventArgs e)
		{
            Global.MainForm.StopOnEnd ^= true;
            this.FastFowardToEnd.Checked ^= true;
            Global.MainForm.FastForward = this.FastFowardToEnd.Checked;
            if (true == this.FastFowardToEnd.Checked)
            {
                this.FastForward.Checked = false;
                this.TurboFastForward.Checked = false;
            }
        }

		private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			if (Global.MainForm.ReadOnly)
			{
				insertFrameToolStripMenuItem.Enabled = false;
			}
			else
			{
				insertFrameToolStripMenuItem.Enabled = true;
			}
		}

		private void insertFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Global.MainForm.ReadOnly)
				return;
		}

		private void updatePadsOnMovePlaybackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.TASUpdatePads ^= true;
		}

		private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
            Global.MainForm.RecordMovie();
        }

		private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
            Global.MainForm.PlayMovie();
        }

		private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
            Global.MovieSession.Movie.WriteMovie();
		}

		private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
            string fileName = Movie.SaveRecordingAs();

            if ("" != fileName)
            {
                Global.MovieSession.Movie.UpdateFileName(fileName);
                Global.MovieSession.Movie.WriteMovie();
            }
		}

		private void ClearVirtualPadHolds()
		{
			foreach (var controller in ControllerBox.Controls)
			{
				if (controller is VirtualPad)
					((VirtualPad)controller).Clear();
			}
		}

		private void clearVirtualPadsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ClearVirtualPadHolds();
		}

		private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			ClearVirtualPadHolds();
		}

        private void FastForward_Click(object sender, EventArgs e)
        {
            this.FastForward.Checked ^= true;
            Global.MainForm.FastForward = this.FastForward.Checked;
            if (true == this.FastForward.Checked)
            {
                this.TurboFastForward.Checked = false;
                this.FastFowardToEnd.Checked = false;
            }
        }

        private void TurboFastForward_Click(object sender, EventArgs e)
        {
            Global.MainForm.TurboFastForward ^= true;
            this.TurboFastForward.Checked ^= true;
            if (true == this.TurboFastForward.Checked)
            {
                this.FastForward.Checked = false;
                this.FastFowardToEnd.Checked = false;
            }
        }
	}
}
