﻿using System;
using System.Text;
using System.Windows.Forms;

using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class VirtualPadGen6Button : UserControl, IVirtualPad
	{
		public string Controller { get; set; }

		public VirtualPadGen6Button()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			BorderStyle = BorderStyle.Fixed3D;
			InitializeComponent();

			Controller = "P1";
		}

		private void VirtualPadGen6Button_Load(object sender, EventArgs e)
		{

		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Up)
			{
				// TODO: move to next logical key
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

			input.Append(BA.Checked ? "A" : ".");
			input.Append(BB.Checked ? "B" : ".");
			input.Append(BC.Checked ? "C" : ".");

			input.Append(BStart.Checked ? "S" : ".");

			input.Append(BX.Checked ? "X" : ".");
			input.Append(BY.Checked ? "Y" : ".");
			input.Append(BZ.Checked ? "Z" : ".");

			input.Append(BM.Checked ? "M" : ".");

			input.Append("|");
			return input.ToString();
		}

		public void Clear()
		{
			if (PU.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Up", false);
			if (PD.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Down", false);
			if (PL.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Left", false);
			if (PR.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Right", false);

			if (BA.Checked) Global.StickyXORAdapter.SetSticky(Controller + " A", false);
			if (BB.Checked) Global.StickyXORAdapter.SetSticky(Controller + " B", false);
			if (BC.Checked) Global.StickyXORAdapter.SetSticky(Controller + " C", false);

			if (BStart.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Start", false);

			if (BX.Checked) Global.StickyXORAdapter.SetSticky(Controller + " X", false);
			if (BY.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Y", false);
			if (BZ.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Z", false);

			if (BM.Checked) Global.StickyXORAdapter.SetSticky(Controller + " Mode", false);

			PU.Checked = false;
			PD.Checked = false;
			PL.Checked = false;
			PR.Checked = false;

			BStart.Checked = false;

			BA.Checked = false;
			BB.Checked = false;
			BC.Checked = false;

			BX.Checked = false;
			BY.Checked = false;
			BZ.Checked = false;

			BM.Checked = false;
		}

		public void SetButtons(string buttons)
		{
			if (buttons.Length < 12) return;

			if (buttons[0] == '.') PU.Checked = false; else PU.Checked = true;
			if (buttons[1] == '.') PD.Checked = false; else PD.Checked = true;
			if (buttons[2] == '.') PL.Checked = false; else PL.Checked = true;
			if (buttons[3] == '.') PR.Checked = false; else PR.Checked = true;

			if (buttons[4] == '.') BA.Checked = false; else BA.Checked = true;
			if (buttons[5] == '.') BB.Checked = false; else BB.Checked = true;
			if (buttons[6] == '.') BC.Checked = false; else BC.Checked = true;
			
			if (buttons[7] == '.') BStart.Checked = false; else BStart.Checked = true;
			
			if (buttons[8] == '.') BX.Checked = false; else BX.Checked = true;
			if (buttons[9] == '.') BY.Checked = false; else BY.Checked = true;
			if (buttons[10] == '.') BZ.Checked = false; else BZ.Checked = true;
			
			if (buttons[11] == '.') BM.Checked = false; else BM.Checked = true;
		}

		private void Buttons_CheckedChanged(object sender, EventArgs e)
		{
			if (sender == PU)
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

			else if (sender == BA)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " A", BA.Checked);
			}
			else if (sender == BB)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " B", BB.Checked);
			}
			else if (sender == BC)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " C", BC.Checked);
			}

			else if (sender == BStart)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Start", BStart.Checked);
			}
			else if (sender == BX)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " X", BX.Checked);
			}
			else if (sender == BY)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Y", BY.Checked);
			}
			else if (sender == BZ)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Z", BZ.Checked);
			}
			
			else if (sender == BM)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Mode", BM.Checked);
			}
		}
	}
}
