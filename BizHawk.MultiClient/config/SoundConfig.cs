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
	public partial class SoundConfig : Form
	{
		public SoundConfig()
		{
			InitializeComponent();
		}

		private void SoundConfig_Load(object sender, EventArgs e)
		{
			SoundOnCheckBox.Checked = Global.Config.SoundEnabled;
			MuteFrameAdvance.Checked = Global.Config.MuteFrameAdvance;
			SoundVolBar.Value = Global.Config.SoundVolume;
			SoundVolNumeric.Value = Global.Config.SoundVolume;
			UpdateSoundDialog();
		}

		private void OK_Click(object sender, EventArgs e)
		{
			Global.Config.SoundEnabled = SoundOnCheckBox.Checked;
			Global.Config.MuteFrameAdvance = MuteFrameAdvance.Checked;
			Global.Config.SoundVolume = SoundVolBar.Value;
			Global.Sound.ChangeVolume(Global.Config.SoundVolume);
			Global.Sound.UpdateSoundSettings();
			Global.Sound.StartSound();
			Global.OSD.AddMessage("Sound settings saved");
			this.Close();
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			SoundVolNumeric.Value = SoundVolBar.Value;
		}

		private void SoundVolNumeric_ValueChanged(object sender, EventArgs e)
		{
			SoundVolBar.Value = (int)SoundVolNumeric.Value;
		}

		private void SoundOnCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			UpdateSoundDialog();
		}

		private void UpdateSoundDialog()
		{
			if (SoundOnCheckBox.Checked)
			{
				SoundVolGroup.Enabled = true;
				MuteFrameAdvance.Enabled = true;
			}
			else
			{
				SoundVolGroup.Enabled = false;
				MuteFrameAdvance.Enabled = false;
			}
		}
	}
}
