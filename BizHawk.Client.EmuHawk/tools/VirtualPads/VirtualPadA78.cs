﻿using System;
using System.Text;
using System.Windows.Forms;

using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class VirtualPadA78 : UserControl, IVirtualPad
	{
		public string Controller { get; set; }

		public VirtualPadA78()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			BorderStyle = BorderStyle.Fixed3D;
			InitializeComponent();

			Controller = "P1";
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

		public string GetMnemonic()
		{
			StringBuilder input = new StringBuilder("");
			input.Append(PU.Checked ? "U" : ".");
			input.Append(PD.Checked ? "D" : ".");
			input.Append(PL.Checked ? "L" : ".");
			input.Append(PR.Checked ? "R" : ".");

			input.Append(B1.Checked ? "1" : ".");
			input.Append(B2.Checked ? "2" : ".");
			input.Append("|");
			return input.ToString();
		}

		public void SetButtons(string buttons)
		{
			if (buttons.Length < 6) return;
			if (buttons[0] == '.') PU.Checked = false; else PU.Checked = true;
			if (buttons[1] == '.') PD.Checked = false; else PD.Checked = true;
			if (buttons[2] == '.') PL.Checked = false; else PL.Checked = true;
			if (buttons[3] == '.') PR.Checked = false; else PR.Checked = true;

			if (buttons[4] == '.') B1.Checked = false; else B1.Checked = true;
			if (buttons[5] == '.') B2.Checked = false; else B2.Checked = true;
		}

		private void Buttons_CheckedChanged(object sender, EventArgs e)
		{
			if (Global.Emulator.SystemId != "A78")
			{
				return;
			}
			else if (sender == PU)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Up", PU.Checked);
			}
			else if (sender == PD)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Down", PD.Checked);
			}
			else if (sender == PL)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Left", PL.Checked);
			}
			else if (sender == PR)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Right", PR.Checked);
			}
			else if (sender == B1)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Trigger", B1.Checked);
			}
			else if (sender == B2)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Trigger 2", B2.Checked);
			}
		}

		public void Clear()
		{
			if (Global.Emulator.SystemId != "A78") return;


			if (PU.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Up", false);
			if (PD.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Down", false);
			if (PL.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Left", false);
			if (PR.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Right", false);
			if (B1.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Trigger", false);
			if (B2.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Trigger 2", false);
			

			PU.Checked = false;
			PD.Checked = false;
			PL.Checked = false;
			PR.Checked = false;
			B1.Checked = false;
			B2.Checked = false;
		}
	}
}
