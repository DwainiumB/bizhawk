﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace BizHawk.MultiClient
{
	public class VirtualPadSNES : VirtualPad
	{

		public VirtualPadSNES()
		{
			ButtonPoints[0] = new Point(14, 2);
			ButtonPoints[1] = new Point(14, 46);
			ButtonPoints[2] = new Point(2, 24);
			ButtonPoints[3] = new Point(24, 24);
			ButtonPoints[4] = new Point(56, 42);
			ButtonPoints[5] = new Point(78, 42);

			
			
			ButtonPoints[6] = new Point(122, 12); //X
			ButtonPoints[7] = new Point(146, 22); //A
			ButtonPoints[8] = new Point(110, 34); //Y
			ButtonPoints[9] = new Point(134, 48); //B

			ButtonPoints[10] = new Point(42, 0); //L
			ButtonPoints[11] = new Point(94, 0); //R

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			this.BorderStyle = BorderStyle.Fixed3D;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.VirtualPad_Paint);
			this.Size = new Size(174, 74);

			Point n = new Point(this.Size);

			this.PU = new CheckBox();
			this.PU.Appearance = System.Windows.Forms.Appearance.Button;
			this.PU.AutoSize = true;
			this.PU.Image = global::BizHawk.MultiClient.Properties.Resources.BlueUp;
			this.PU.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
			this.PU.Location = ButtonPoints[0];
			this.PU.TabIndex = 1;
			this.PU.UseVisualStyleBackColor = true;
			this.PU.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.PD = new CheckBox();
			this.PD.Appearance = System.Windows.Forms.Appearance.Button;
			this.PD.AutoSize = true;
			this.PD.Image = global::BizHawk.MultiClient.Properties.Resources.BlueDown;
			this.PD.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
			this.PD.Location = ButtonPoints[1];
			this.PD.TabIndex = 4;
			this.PD.UseVisualStyleBackColor = true;
			this.PD.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.PR = new CheckBox();
			this.PR.Appearance = System.Windows.Forms.Appearance.Button;
			this.PR.AutoSize = true;
			this.PR.Image = global::BizHawk.MultiClient.Properties.Resources.Forward;
			this.PR.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
			this.PR.Location = ButtonPoints[3];
			this.PR.TabIndex = 3;
			this.PR.UseVisualStyleBackColor = true;
			this.PR.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.PL = new CheckBox();
			this.PL.Appearance = System.Windows.Forms.Appearance.Button;
			this.PL.AutoSize = true;
			this.PL.Image = global::BizHawk.MultiClient.Properties.Resources.Back;
			this.PL.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
			this.PL.Location = ButtonPoints[2];
			this.PL.TabIndex = 2;
			this.PL.UseVisualStyleBackColor = true;
			this.PL.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.B1 = new CheckBox();
			this.B1.Appearance = System.Windows.Forms.Appearance.Button;
			this.B1.AutoSize = true;
			this.B1.Location = ButtonPoints[4];
			this.B1.TabIndex = 5;
			this.B1.Text = "s";
			this.B1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.B1.UseVisualStyleBackColor = true;
			this.B1.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.B2 = new CheckBox();
			this.B2.Appearance = System.Windows.Forms.Appearance.Button;
			this.B2.AutoSize = true;
			this.B2.Location = ButtonPoints[5];
			this.B2.TabIndex = 6;
			this.B2.Text = "S";
			this.B2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.B2.UseVisualStyleBackColor = true;
			this.B2.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.B3 = new CheckBox();
			this.B3.Appearance = System.Windows.Forms.Appearance.Button;
			this.B3.AutoSize = true;
			this.B3.Location = ButtonPoints[9];
			this.B3.TabIndex = 7;
			this.B3.Text = "B";
			this.B3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.B3.UseVisualStyleBackColor = true;
			this.B3.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.B4 = new CheckBox();
			this.B4.Appearance = System.Windows.Forms.Appearance.Button;
			this.B4.AutoSize = true;
			this.B4.Location = ButtonPoints[7];
			this.B4.TabIndex = 8;
			this.B4.Text = "A";
			this.B4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.B4.UseVisualStyleBackColor = true;
			this.B4.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.B5 = new CheckBox();
			this.B5.Appearance = System.Windows.Forms.Appearance.Button;
			this.B5.AutoSize = true;
			this.B5.Location = ButtonPoints[6];
			this.B5.TabIndex = 8;
			this.B5.Text = "X";
			this.B5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.B5.UseVisualStyleBackColor = true;
			this.B5.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.B6 = new CheckBox();
			this.B6.Appearance = System.Windows.Forms.Appearance.Button;
			this.B6.AutoSize = true;
			this.B6.Location = ButtonPoints[8];
			this.B6.TabIndex = 8;
			this.B6.Text = "Y";
			this.B6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.B6.UseVisualStyleBackColor = true;
			this.B6.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.B7 = new CheckBox();
			this.B7.Appearance = System.Windows.Forms.Appearance.Button;
			this.B7.AutoSize = true;
			this.B7.Location = ButtonPoints[10];
			this.B7.TabIndex = 8;
			this.B7.Text = "L";
			this.B7.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.B7.UseVisualStyleBackColor = true;
			this.B7.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);

			this.B8 = new CheckBox();
			this.B8.Appearance = System.Windows.Forms.Appearance.Button;
			this.B8.AutoSize = true;
			this.B8.Location = ButtonPoints[11];
			this.B8.TabIndex = 8;
			this.B8.Text = "R";
			this.B8.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.B8.UseVisualStyleBackColor = true;
			this.B8.CheckedChanged += new System.EventHandler(this.Buttons_CheckedChanged);


			this.Controls.Add(this.PU);
			this.Controls.Add(this.PD);
			this.Controls.Add(this.PL);
			this.Controls.Add(this.PR);
			this.Controls.Add(this.B1);
			this.Controls.Add(this.B2);
			this.Controls.Add(this.B3);
			this.Controls.Add(this.B4);
			this.Controls.Add(this.B5);
			this.Controls.Add(this.B6);
			this.Controls.Add(this.B7);
			this.Controls.Add(this.B8);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Up)
			{
				//TODO: move to next logical key
				this.Refresh();
			}
			else if (keyData == Keys.Down)
			{
				this.Refresh();
			}
			else if (keyData == Keys.Left)
			{
				this.Refresh();
			}
			else if (keyData == Keys.Right)
			{
				this.Refresh();
			}
			else if (keyData == Keys.Tab)
			{
				this.Refresh();
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

			input.Append(B1.Checked ? "s" : ".");
			input.Append(B2.Checked ? "S" : ".");
			input.Append(B3.Checked ? "B" : ".");
			input.Append(B4.Checked ? "A" : ".");
			input.Append(B5.Checked ? "X" : ".");
			input.Append(B6.Checked ? "Y" : ".");
			input.Append(B7.Checked ? "L" : ".");
			input.Append(B8.Checked ? "R" : ".");
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
			if (buttons[8] == '.') B5.Checked = false; else B5.Checked = true;
			if (buttons[9] == '.') B6.Checked = false; else B6.Checked = true;
			if (buttons[10] == '.') B7.Checked = false; else B7.Checked = true;
			if (buttons[11] == '.') B8.Checked = false; else B8.Checked = true;
		}

		private void Buttons_CheckedChanged(object sender, EventArgs e)
		{
			if (Global.Emulator.SystemId != "SNES")
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
				Global.StickyXORAdapter.SetSticky(Controller + " Select", B1.Checked);
			}
			else if (sender == B2)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Start", B2.Checked);
			}
			else if (sender == B3)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " B", B3.Checked);
			}
			else if (sender == B4)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " A", B4.Checked);
			}
			else if (sender == B5)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " X", B5.Checked);
			}
			else if (sender == B6)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " Y", B6.Checked);
			}

			else if (sender == B7)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " L", B7.Checked);
			}
			else if (sender == B8)
			{
				Global.StickyXORAdapter.SetSticky(Controller + " R", B8.Checked);
			}
		}

		public override void Clear()
		{
			if (Global.Emulator.SystemId != "SNES")
			{
				return;
			}
			else
			{
				PU.Checked = false;
				PD.Checked = false;
				PL.Checked = false;
				PR.Checked = false;

				B1.Checked = false;
				B2.Checked = false;
				B3.Checked = false;
				B4.Checked = false;

				B5.Checked = false;
				B6.Checked = false;
				B7.Checked = false;
				B8.Checked = false;

				Global.StickyXORAdapter.SetSticky(Controller + " Up", false);
				Global.StickyXORAdapter.SetSticky(Controller + " Down", false);
				Global.StickyXORAdapter.SetSticky(Controller + " Left", false);
				Global.StickyXORAdapter.SetSticky(Controller + " Right", false);
				Global.StickyXORAdapter.SetSticky(Controller + " Select", false);
				Global.StickyXORAdapter.SetSticky(Controller + " Start", false);
				Global.StickyXORAdapter.SetSticky(Controller + " B", false);
				Global.StickyXORAdapter.SetSticky(Controller + " A", false);
				Global.StickyXORAdapter.SetSticky(Controller + " X", false);
				Global.StickyXORAdapter.SetSticky(Controller + " Y", false);
				Global.StickyXORAdapter.SetSticky(Controller + " L", false);
				Global.StickyXORAdapter.SetSticky(Controller + " R", false);
			}
		}
	}
}
