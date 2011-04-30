﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BizHawk.Emulation.Consoles.Nintendo;
using BizHawk.Emulation.Consoles.Calculator;

namespace BizHawk.MultiClient
{
    public partial class ToolBox : Form
    {
        public ToolBox()
        {
            InitializeComponent();
        }

        private void ToolBox_Load(object sender, EventArgs e)
        {
            int x = Global.MainForm.Location.X + Global.MainForm.Size.Width;
            int y = Global.MainForm.Location.Y;
            Location = new Point(x, y);
            if (Global.Emulator is NES)
            {
                NESPPU.Visible = true;
                NESDebugger.Visible = true;
                NESGameGenie.Visible = true;
                NESNameTable.Visible = true;
            }
            else
            {
                NESPPU.Visible = false;
                NESDebugger.Visible = false;
                NESGameGenie.Visible = false;
                NESNameTable.Visible = false;
            }
            if (Global.Emulator is TI83)
            {

            }
            else
            {

            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Global.MainForm.LoadCheatsWindow();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Global.MainForm.LoadRamWatch();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Global.MainForm.LoadRamSearch();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            RamPoke r = new RamPoke();
            r.Show();
        }

        private void HexEditor_Click(object sender, EventArgs e)
        {
            Global.MainForm.LoadHexEditor();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            var window = new BizHawk.MultiClient.tools.LuaWindow();
            window.Show();
        }

        private void NESPPU_Click(object sender, EventArgs e)
        {
            Global.MainForm.LoadNESPPU();
        }

        private void NESDebugger_Click(object sender, EventArgs e)
        {
            Global.MainForm.LoadNESDebugger();
        }

        private void NESGameGenie_Click(object sender, EventArgs e)
        {
            Global.MainForm.LoadGameGenieEC();
        }

        private void NESNameTable_Click(object sender, EventArgs e)
        {
            Global.MainForm.LoadNESNameTable();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (!(Global.Emulator is TI83))
                return;
            Global.MainForm.LoadTI83KeyPad();
        }
    }
}
