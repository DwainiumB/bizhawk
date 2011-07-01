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
	public partial class MessageConfig : Form
	{
		//TODO: 
		//Implement message position as a variable
		//Make a checkbox to enable/disable the stacking effect of message label
		//Deal with typing into Numerics properly
		//Have some method of binding a display object to top/bottom/left/right instead of an absolute position
		//Bug: restore defaults doesn't restore the y value of whatever radio is checked
		//Implement Multitrack messages counter config

		int DispFPSx = Global.Config.DispFPSx;
		int DispFPSy = Global.Config.DispFPSy;
		int DispFrameCx = Global.Config.DispFrameCx;
		int DispFrameCy = Global.Config.DispFrameCy;
		int DispLagx = Global.Config.DispLagx;
		int DispLagy = Global.Config.DispLagy;
		int DispInpx = Global.Config.DispInpx;
		int DispInpy = Global.Config.DispInpy;
		int DispRerecx = Global.Config.DispRecx;
		int DispRerecy = Global.Config.DispRecy;
		int MessageColor = Global.Config.MessagesColor;
		int AlertColor = Global.Config.AlertMessageColor;
		int LastInputColor = Global.Config.LastInputColor;
		int DispRecx = Global.Config.DispRecx;
		int DispRecy = Global.Config.DispRecy;

		int DispFPSanchor = Global.Config.DispFPSanchor;
		int DispFrameanchor = Global.Config.DispFrameanchor;
		int DispLaganchor = Global.Config.DispLaganchor;
		int DispInputanchor = Global.Config.DispInpanchor;
		int DispRecanchor = Global.Config.DispRecanchor;

		public Brush brush = Brushes.Black;
		int px = 0;
		int py = 0;
		bool mousedown = false;

		public MessageConfig()
		{
			InitializeComponent();
		}

		private void MessageConfig_Load(object sender, EventArgs e)
		{
			SetMaxXY();
			MessageColorDialog.Color = Color.FromArgb(MessageColor);
			AlertColorDialog.Color = Color.FromArgb(AlertColor);
			LInputColorDialog.Color = Color.FromArgb(LastInputColor);
			SetColorBox();
			SetPositionInfo();
		}

		private void SetMaxXY()
		{
			XNumeric.Maximum = Global.Emulator.VideoProvider.BufferWidth - 8;
			YNumeric.Maximum = Global.Emulator.VideoProvider.BufferHeight - 8;
			PositionPanel.Size = new Size(Global.Emulator.VideoProvider.BufferWidth, Global.Emulator.VideoProvider.BufferHeight);

			int width;
			if (Global.Emulator.VideoProvider.BufferWidth > 128)
				width = Global.Emulator.VideoProvider.BufferWidth + 44;
			else
				width = 128 + 44;

			PositionGroupBox.Size = new Size(width, Global.Emulator.VideoProvider.BufferHeight + 52);
		}

		private void SetColorBox()
		{
			MessageColor = MessageColorDialog.Color.ToArgb();
			ColorPanel.BackColor = MessageColorDialog.Color;
			ColorText.Text = String.Format("{0:X8}", MessageColor);

			AlertColor = AlertColorDialog.Color.ToArgb();
			AlertColorPanel.BackColor = AlertColorDialog.Color;
			AlertColorText.Text = String.Format("{0:X8}", AlertColor);

			LastInputColor = LInputColorDialog.Color.ToArgb();
			LInputColorPanel.BackColor = LInputColorDialog.Color;
			LInputText.Text = String.Format("{0:X8}", LastInputColor);
		}

		private void SetAnchorRadio(int anchor)
		{
			switch (anchor)
			{
				default:
				case 0:
					TL.Checked = true; break;
				case 1:
					TR.Checked = true; break;
				case 2:
					BL.Checked = true; break;
				case 3:
					BR.Checked = true; break;
			}
		}

		private void SetPositionInfo()
		{
			if (FPSRadio.Checked)
			{
				XNumeric.Value = DispFPSx;
				YNumeric.Value = DispFPSy;
				px = DispFPSx;
				py = DispFPSy;
				SetAnchorRadio(DispFPSanchor);
			}
			else if (FrameCounterRadio.Checked)
			{
				XNumeric.Value = DispFrameCx;
				YNumeric.Value = DispFrameCy;
				px = DispFrameCx;
				py = DispFrameCy;
				SetAnchorRadio(DispFrameanchor);
			}
			else if (LagCounterRadio.Checked)
			{
				XNumeric.Value = DispLagx;
				YNumeric.Value = DispLagy;
				px = DispLagx;
				py = DispLagy;
				SetAnchorRadio(DispLaganchor);
			}
			else if (InputDisplayRadio.Checked)
			{
				XNumeric.Value = DispInpx;
				XNumeric.Value = DispInpy;
				px = DispInpx;
				py = DispInpy;
				SetAnchorRadio(DispInputanchor);
			}
			else if (MessagesRadio.Checked)
			{
				XNumeric.Value = 0;
				YNumeric.Value = 0;
				px = 0;
				py = 0;
			}
			else if (RerecordsRadio.Checked)
			{
				XNumeric.Value = DispRecx;
				YNumeric.Value = DispRecy;
				px = DispRecx;
				py = DispRecy;
				SetAnchorRadio(DispRecanchor);
			}

			PositionPanel.Refresh();
			XNumeric.Refresh();
			YNumeric.Refresh();
			SetPositionLabels();
		}

		private void SaveSettings()
		{
			Global.Config.DispFPSx = DispFPSx;
			Global.Config.DispFPSy = DispFPSy;
			Global.Config.DispFrameCx = DispFrameCx;
			Global.Config.DispFrameCy = DispFrameCy;
			Global.Config.DispLagx = DispLagx;
			Global.Config.DispLagy = DispLagy;
			Global.Config.DispInpx = DispInpx;
			Global.Config.DispInpy = DispInpy;
			Global.Config.DispRecx = DispRecx;
			Global.Config.DispRecy = DispRecy;
			Global.Config.MessagesColor = MessageColor;
			Global.Config.AlertMessageColor = AlertColor;
			Global.Config.LastInputColor = LastInputColor;

			Global.Config.DispFPSanchor = DispFPSanchor;
			Global.Config.DispFrameanchor = DispFrameanchor;
			Global.Config.DispLaganchor = DispLaganchor;
			Global.Config.DispInpanchor = DispInputanchor;
			Global.Config.DispRecanchor = DispRecanchor;
		}

		private void OK_Click(object sender, EventArgs e)
		{
			SaveSettings();
			this.Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (MessageColorDialog.ShowDialog() == DialogResult.OK)
				SetColorBox();
		}

		private void FPSRadio_CheckedChanged(object sender, EventArgs e)
		{
			SetPositionInfo();
		}

		private void FrameCounterRadio_CheckedChanged(object sender, EventArgs e)
		{
			SetPositionInfo();
		}

		private void LagCounterRadio_CheckedChanged(object sender, EventArgs e)
		{
			SetPositionInfo();
		}

		private void InputDisplayRadio_CheckedChanged(object sender, EventArgs e)
		{
			SetPositionInfo();
		}

		private void MessagesRadio_CheckedChanged(object sender, EventArgs e)
		{
			SetPositionInfo();
		}


		private void RerecordsRadio_CheckedChanged(object sender, EventArgs e)
		{
			SetPositionInfo();
		}

		private void XNumericChange()
		{
			px = (int)XNumeric.Value;
			SetPositionLabels();
			PositionPanel.Refresh();
		}

		private void YNumericChange()
		{
			py = (int)YNumeric.Value;
			SetPositionLabels();
			PositionPanel.Refresh();
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void PositionPanel_MouseEnter(object sender, EventArgs e)
		{
			this.Cursor = Cursors.Hand;
		}

		private void PositionPanel_MouseLeave(object sender, EventArgs e)
		{
			this.Cursor = Cursors.Default;
		}

		private void PositionPanel_Paint(object sender, PaintEventArgs e)
		{
			Pen p = new Pen(brush);
			e.Graphics.DrawLine(p, new Point(px - 2, py - 2), new Point(px + 2, py + 2));
			e.Graphics.DrawLine(p, new Point(px + 2, py - 2), new Point(px - 2, py + 2));
		}

		private void PositionPanel_MouseDown(object sender, MouseEventArgs e)
		{
			this.Cursor = Cursors.Arrow;
			mousedown = true;
			SetNewPosition(e.X, e.Y);
		}

		private void PositionPanel_MouseUp(object sender, MouseEventArgs e)
		{
			this.Cursor = Cursors.Hand;
			mousedown = false;
		}

		private void SetNewPosition(int mx, int my)
		{
			if (mx < 0) mx = 0;
			if (my < 0) my = 0;
			if (mx > XNumeric.Maximum) mx = (int)XNumeric.Maximum;
			if (my > YNumeric.Maximum) my = (int)YNumeric.Maximum;
			XNumeric.Value = mx;
			YNumeric.Value = my;
			px = mx;
			py = my;
			PositionPanel.Refresh();
			SetPositionLabels();
		}

		private void PositionPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (mousedown)
			{
				SetNewPosition(e.X, e.Y);
			}
		}

		private void SetPositionLabels()
		{
			if (FPSRadio.Checked)
			{
				DispFPSx = px;
				DispFPSy = py;
			}
			else if (FrameCounterRadio.Checked)
			{
				DispFrameCx = px;
				DispFrameCy = py;
			}
			else if (LagCounterRadio.Checked)
			{
				DispLagx = px;
				DispLagy = py;
			}
			else if (InputDisplayRadio.Checked)
			{
				DispInpx = px;
				DispInpy = py;
			}
			else if (MessagesRadio.Checked)
			{
				//TODO
			}
			else if (RerecordsRadio.Checked)
			{
				DispRecx = px;
				DispRecy = py;
			}
			FpsPosLabel.Text = DispFPSx.ToString() + ", " + DispFPSy.ToString();
			FCLabel.Text = DispFrameCx.ToString() + ", " + DispFrameCy.ToString();
			LagLabel.Text = DispLagx.ToString() + ", " + DispLagy.ToString();
			InpLabel.Text = DispInpx.ToString() + ", " + DispInpy.ToString();
			RerecLabel.Text = DispRecx.ToString() + ", " + DispRecy.ToString();
			MessLabel.Text = "0, 0";
		}

		private void ResetDefaultsButton_Click(object sender, EventArgs e)
		{
			Global.Config.DispFPSx = 0;
			Global.Config.DispFPSy = 0;
			Global.Config.DispFrameCx = 0;
			Global.Config.DispFrameCy = 12;
			Global.Config.DispLagx = 0;
			Global.Config.DispLagy = 36;
			Global.Config.DispInpx = 0;
			Global.Config.DispInpy = 24;
			Global.Config.DispRecx = 0;
			Global.Config.DispRecy = 48;
			Global.Config.MessagesColor = -1;
			Global.Config.AlertMessageColor = -65536;
			Global.Config.LastInputColor = -23296;

			Global.Config.DispFPSanchor = 0;
			Global.Config.DispFrameanchor = 0;
			Global.Config.DispLaganchor = 0;
			Global.Config.DispInpanchor = 0;
			Global.Config.DispRecanchor = 0;
			
			DispFPSx = Global.Config.DispFPSx;
			DispFPSy = Global.Config.DispFPSy;
			DispFrameCx = Global.Config.DispFrameCx;
			DispFrameCy = Global.Config.DispFrameCy;
			DispLagx = Global.Config.DispLagx;
			DispLagy = Global.Config.DispLagy;
			DispInpx = Global.Config.DispInpx;
			DispInpy = Global.Config.DispInpy;
			DispRecx = Global.Config.DispRecx;
			DispRecy = Global.Config.DispRecy;

			MessageColor = Global.Config.MessagesColor;
			AlertColor = Global.Config.AlertMessageColor;
			LastInputColor = Global.Config.LastInputColor;

			DispFPSanchor = Global.Config.DispFPSanchor;
			DispFrameanchor = Global.Config.DispFrameanchor;
			DispLaganchor = Global.Config.DispLaganchor;
			DispInputanchor = Global.Config.DispInpanchor;
			DispRecanchor = Global.Config.DispRecanchor;
			
			SetMaxXY();
			MessageColorDialog.Color = Color.FromArgb(MessageColor);
			AlertColorDialog.Color = Color.FromArgb(AlertColor);
			LInputColorDialog.Color = Color.FromArgb(LastInputColor);
			SetColorBox();
			SetPositionInfo();
		}

		private void ColorPanel_DoubleClick(object sender, EventArgs e)
		{
			if (MessageColorDialog.ShowDialog() == DialogResult.OK)
				SetColorBox();
		}

		private void ChangeAlertColor_Click(object sender, EventArgs e)
		{
			if (AlertColorDialog.ShowDialog() == DialogResult.OK)
				SetColorBox();
		}

		private void ChangeLInput_Click(object sender, EventArgs e)
		{
			if (LInputColorDialog.ShowDialog() == DialogResult.OK)
				SetColorBox();
		}

		private void TL_CheckedChanged(object sender, EventArgs e)
		{
			if (TL.Checked)
			{
				if (FPSRadio.Checked)
					DispFPSanchor = 0;
				else if (FrameCounterRadio.Checked)
					DispFrameanchor = 0;
				else if (LagCounterRadio.Checked)
					DispLaganchor = 0;
				else if (InputDisplayRadio.Checked)
					DispInputanchor = 0;
				else if (RerecordsRadio.Checked)
					DispRecanchor = 0;
			}
		}

		private void TR_CheckedChanged(object sender, EventArgs e)
		{
			if (TR.Checked)
			{
				if (FPSRadio.Checked)
					DispFPSanchor = 1;
				else if (FrameCounterRadio.Checked)
					DispFrameanchor = 1;
				else if (LagCounterRadio.Checked)
					DispLaganchor = 1;
				else if (InputDisplayRadio.Checked)
					DispInputanchor = 1;
				else if (RerecordsRadio.Checked)
					DispRecanchor = 1;
			}
		}

		private void BL_CheckedChanged(object sender, EventArgs e)
		{
			if (BL.Checked)
			{
				if (FPSRadio.Checked)
					DispFPSanchor = 2;
				else if (FrameCounterRadio.Checked)
					DispFrameanchor = 2;
				else if (LagCounterRadio.Checked)
					DispLaganchor = 2;
				else if (InputDisplayRadio.Checked)
					DispInputanchor = 2;
				else if (RerecordsRadio.Checked)
					DispRecanchor = 2;
			}
		}

		private void BR_CheckedChanged(object sender, EventArgs e)
		{
			if (BR.Checked)
			{
				if (FPSRadio.Checked)
					DispFPSanchor = 3;
				else if (FrameCounterRadio.Checked)
					DispFrameanchor = 3;
				else if (LagCounterRadio.Checked)
					DispLaganchor = 3;
				else if (InputDisplayRadio.Checked)
					DispInputanchor = 3;
				else if (RerecordsRadio.Checked)
					DispRecanchor = 3;
			}
		}

		private void XNumeric_Click(object sender, EventArgs e)
		{
			XNumericChange();
		}

		private void YNumeric_Click(object sender, EventArgs e)
		{
			YNumericChange();
		}
	}
}
