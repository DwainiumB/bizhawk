﻿using System;
using System.Text;
using System.Windows.Forms;

namespace BizHawk.MultiClient
{
	public partial class VirtualPadA78Control : UserControl, IVirtualPad
	{
		public VirtualPadA78Control()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			BorderStyle = BorderStyle.Fixed3D;
			Paint += VirtualPad_Paint;
			InitializeComponent();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Up)
			{
				//TODO: move to next logical key
				Refresh();
			}
			else if (keyData == Keys.Down)
			{
				Refresh();
			}
			else if (keyData == Keys.Left)
			{
				Refresh();
			}
			else if (keyData == Keys.Right)
			{
				Refresh();
			}
			else if (keyData == Keys.Tab)
			{
				Refresh();
			}

			return true;
		}

		private void VirtualPad_Paint(object sender, PaintEventArgs e)
		{

		}

		public string GetMnemonic()
		{
			StringBuilder input = new StringBuilder("");
			input.Append(B1.Checked ? "P" : ".");
			input.Append(B2.Checked ? "r" : ".");
			input.Append(B3.Checked ? "s" : ".");
			input.Append(B4.Checked ? "p" : ".");
			input.Append("|");
			return input.ToString();
		}

		public void SetButtons(string buttons)
		{
			if (buttons.Length < 4) return;
			if (buttons[0] == '.') B1.Checked = false; else B1.Checked = true;
			if (buttons[1] == '.') B2.Checked = false; else B2.Checked = true;
			if (buttons[2] == '.') B3.Checked = false; else B3.Checked = true;
			if (buttons[3] == '.') B4.Checked = false; else B4.Checked = true;
		}

		private void Buttons_CheckedChanged(object sender, EventArgs e)
		{
			if (GlobalWinF.Emulator.SystemId != "A78")
			{
				return;
			}
			else if (sender == B1)
			{
				GlobalWinF.StickyXORAdapter.SetSticky("Power", B1.Checked);
			}
			else if (sender == B2)
			{
				GlobalWinF.StickyXORAdapter.SetSticky("Reset", B2.Checked);
			}
			else if (sender == B3)
			{
				GlobalWinF.StickyXORAdapter.SetSticky("Select", B3.Checked);
			}
			else if (sender == B4)
			{
				GlobalWinF.StickyXORAdapter.SetSticky("Pause", B4.Checked);
			}
		}

		public void Clear()
		{
			if (GlobalWinF.Emulator.SystemId != "A78") return;

			if (B1.Checked) GlobalWinF.StickyXORAdapter.SetSticky("Power", false);
			if (B2.Checked) GlobalWinF.StickyXORAdapter.SetSticky("Reset", false);
			if (B3.Checked) GlobalWinF.StickyXORAdapter.SetSticky("Select", false);
			if (B4.Checked) GlobalWinF.StickyXORAdapter.SetSticky("Pause", false);

			B1.Checked = false;
			B2.Checked = false;
			B3.Checked = false;
			B4.Checked = false;
		}

		private void VirtualPadA78Control_Load(object sender, EventArgs e)
		{

		}
	}
}
