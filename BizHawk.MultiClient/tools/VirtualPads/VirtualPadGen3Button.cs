﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace BizHawk.MultiClient
{
	class VirtualPadGen3Button : VirtualPad
	{
		public VirtualPadGen3Button()
		{
			ButtonPoints[0] = new Point(14, 2);
			ButtonPoints[1] = new Point(14, 46);
			ButtonPoints[2] = new Point(2, 24);
			ButtonPoints[3] = new Point(24, 24);
			ButtonPoints[4] = new Point(146, 36);
			ButtonPoints[5] = new Point(122, 2);
			ButtonPoints[6] = new Point(122, 36);
			ButtonPoints[7] = new Point(98, 36);

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			BorderStyle = BorderStyle.Fixed3D;
			Paint += VirtualPad_Paint;
			Size = new Size(174, 74);

			PU = new CheckBox
				{
					Appearance = Appearance.Button,
					AutoSize = true,
					Image = Properties.Resources.BlueUp,
					ImageAlign = ContentAlignment.BottomRight,
					Location = ButtonPoints[0],
					TabIndex = 1,
					UseVisualStyleBackColor = true
				};
			PU.CheckedChanged += Buttons_CheckedChanged;

			PD = new CheckBox
				{
					Appearance = Appearance.Button,
					AutoSize = true,
					Image = Properties.Resources.BlueDown,
					ImageAlign = ContentAlignment.BottomRight,
					Location = ButtonPoints[1],
					TabIndex = 4,
					UseVisualStyleBackColor = true
				};
			PD.CheckedChanged += Buttons_CheckedChanged;

			PR = new CheckBox
				{
					Appearance = Appearance.Button,
					AutoSize = true,
					Image = Properties.Resources.Forward,
					ImageAlign = ContentAlignment.BottomRight,
					Location = ButtonPoints[3],
					TabIndex = 3,
					UseVisualStyleBackColor = true
				};
			PR.CheckedChanged += Buttons_CheckedChanged;

			PL = new CheckBox
				{
					Appearance = Appearance.Button,
					AutoSize = true,
					Image = Properties.Resources.Back,
					ImageAlign = ContentAlignment.BottomRight,
					Location = ButtonPoints[2],
					TabIndex = 2,
					UseVisualStyleBackColor = true
				};
			PL.CheckedChanged += Buttons_CheckedChanged;

			B1 = new CheckBox
				{
					Appearance = Appearance.Button,
					AutoSize = true,
					Location = ButtonPoints[4],
					TabIndex = 5,
					Text = "C",
					TextAlign = ContentAlignment.BottomCenter,
					UseVisualStyleBackColor = true
				};
			B1.CheckedChanged += Buttons_CheckedChanged;

			B2 = new CheckBox
				{
					Appearance = Appearance.Button,
					AutoSize = true,
					Location = ButtonPoints[5],
					TabIndex = 6,
					Text = "S",
					TextAlign = ContentAlignment.BottomCenter,
					UseVisualStyleBackColor = true
				};
			B2.CheckedChanged += Buttons_CheckedChanged;

			B3 = new CheckBox
				{
					Appearance = Appearance.Button,
					AutoSize = true,
					Location = ButtonPoints[6],
					TabIndex = 7,
					Text = "B",
					TextAlign = ContentAlignment.BottomCenter,
					UseVisualStyleBackColor = true
				};
			B3.CheckedChanged += Buttons_CheckedChanged;

			B4 = new CheckBox
				{
					Appearance = Appearance.Button,
					AutoSize = true,
					Location = ButtonPoints[7],
					TabIndex = 8,
					Text = "A",
					TextAlign = ContentAlignment.BottomCenter,
					UseVisualStyleBackColor = true
				};
			B4.CheckedChanged += Buttons_CheckedChanged;

			Controls.Add(PU);
			Controls.Add(PD);
			Controls.Add(PL);
			Controls.Add(PR);
			Controls.Add(B1);
			Controls.Add(B2);
			Controls.Add(B3);
			Controls.Add(B4);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Up:
					Refresh();
					break;
				case Keys.Down:
					Refresh();
					break;
				case Keys.Left:
					Refresh();
					break;
				case Keys.Right:
					Refresh();
					break;
				case Keys.Tab:
					Refresh();
					break;
			}
			return true;
		}

		private void VirtualPad_Paint(object sender, PaintEventArgs e)
		{

		}

		public override string GetMnemonic()
		{
			StringBuilder input = new StringBuilder("");
			input.Append(PU.Checked ? "U" : ".");
			input.Append(PD.Checked ? "D" : ".");
			input.Append(PL.Checked ? "L" : ".");
			input.Append(PR.Checked ? "R" : ".");

			input.Append(B1.Checked ? "C" : ".");
			input.Append(B2.Checked ? "S" : ".");
			input.Append(B3.Checked ? "B" : ".");
			input.Append(B4.Checked ? "A" : ".");
			input.Append("|");
			return input.ToString();
		}

		public override void SetButtons(string buttons)
		{
			if (buttons.Length < 8) return;
            if (buttons[0] == '.') PU.Checked = false; else PU.Checked = true;
            if (buttons[1] == '.') PD.Checked = false; else PD.Checked = true;
            if (buttons[2] == '.') PL.Checked = false; else PL.Checked = true;
            if (buttons[3] == '.') PR.Checked = false; else PR.Checked = true;

            if (buttons[4] == '.') B1.Checked = false; else B1.Checked = true;
            if (buttons[5] == '.') B2.Checked = false; else B2.Checked = true;
            if (buttons[6] == '.') B3.Checked = false; else B3.Checked = true;
            if (buttons[7] == '.') B4.Checked = false; else B4.Checked = true;
        }

		private void Buttons_CheckedChanged(object sender, EventArgs e)
		{
			if (GlobalWinF.Emulator.SystemId != "GEN") return;
			if (sender == PU)
				GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Up", PU.Checked);
			else if (sender == PD)
				GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Down", PD.Checked);
			else if (sender == PL)
				GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Left", PL.Checked);
			else if (sender == PR)
				GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Right", PR.Checked);
			else if (sender == B1)
				GlobalWinF.StickyXORAdapter.SetSticky(Controller + " C", B1.Checked);
			else if (sender == B2)
				GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Start", B2.Checked);
			else if (sender == B3)
				GlobalWinF.StickyXORAdapter.SetSticky(Controller + " B", B3.Checked);
			else if (sender == B4)
				GlobalWinF.StickyXORAdapter.SetSticky(Controller + " A", B4.Checked);
		}

		public override void Clear()
		{
			if (GlobalWinF.Emulator.SystemId != "GEN") return;

			if (PU.Checked) GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Up", false);
			if (PD.Checked) GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Down", false);
			if (PL.Checked) GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Left", false);
			if (PR.Checked) GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Right", false);
			if (B1.Checked) GlobalWinF.StickyXORAdapter.SetSticky(Controller + " C", false);
			if (B2.Checked) GlobalWinF.StickyXORAdapter.SetSticky(Controller + " Start", false);
			if (B3.Checked) GlobalWinF.StickyXORAdapter.SetSticky(Controller + " B", false);
			if (B4.Checked) GlobalWinF.StickyXORAdapter.SetSticky(Controller + " A", false);

			PU.Checked = false;
			PD.Checked = false;
			PL.Checked = false;
			PR.Checked = false;

			B1.Checked = false;
			B2.Checked = false;
			B3.Checked = false;
			B4.Checked = false;
		}
	}
}
