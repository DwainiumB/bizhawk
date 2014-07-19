﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class PlaybackBox : UserControl
	{
		public TAStudio Tastudio { get; set; }

		public PlaybackBox()
		{
			InitializeComponent();
		}

		private void PreviousMarkerButton_Click(object sender, EventArgs e)
		{
			Tastudio.GoToPreviousMarker();
		}

		private void RewindButton_Click(object sender, EventArgs e)
		{
			Tastudio.GoToPreviousFrame();
		}

		private void PauseButton_Click(object sender, EventArgs e)
		{
			Tastudio.TogglePause();
		}

		private void FrameAdvanceButton_Click(object sender, EventArgs e)
		{
			Tastudio.GoToNextFrame();
		}

		private void NextMarkerButton_Click(object sender, EventArgs e)
		{
			Tastudio.GoToNextMarker();
		}
	}
}
