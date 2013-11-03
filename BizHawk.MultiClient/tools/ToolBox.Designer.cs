﻿namespace BizHawk.Client.EmuHawk
{
	partial class ToolBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolBox));
			this.toolStrip1 = new ToolStripEx();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.HexEditor = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
			this.TAStudioButton = new System.Windows.Forms.ToolStripButton();
			this.NESDebugger = new System.Windows.Forms.ToolStripButton();
			this.NESPPU = new System.Windows.Forms.ToolStripButton();
			this.NESNameTable = new System.Windows.Forms.ToolStripButton();
			this.NESGameGenie = new System.Windows.Forms.ToolStripButton();
			this.KeypadTool = new System.Windows.Forms.ToolStripButton();
			this.SNESGraphicsDebuggerButton = new System.Windows.Forms.ToolStripButton();
			this.SNESGameGenie = new System.Windows.Forms.ToolStripButton();
			this.GGGameGenie = new System.Windows.Forms.ToolStripButton();
			this.GBGameGenie = new System.Windows.Forms.ToolStripButton();
			this.GameboyDebuggerTool = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
			this.toolStrip1.ClickThrough = true;
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.HexEditor,
            this.toolStripButton5,
            this.toolStripButton6,
            this.TAStudioButton,
            this.NESDebugger,
            this.NESPPU,
            this.NESNameTable,
            this.NESGameGenie,
            this.KeypadTool,
            this.SNESGraphicsDebuggerButton,
            this.SNESGameGenie,
            this.GGGameGenie,
            this.GBGameGenie,
            this.GameboyDebuggerTool});
			this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
			this.toolStrip1.Location = new System.Drawing.Point(9, 11);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(100, 394);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.TabStop = true;
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.Freeze;
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(63, 20);
			this.toolStripButton1.Text = "Cheats";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// toolStripButton2
			// 
			this.toolStripButton2.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.FindHS;
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(88, 20);
			this.toolStripButton2.Text = "Ram Watch";
			this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.search;
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(89, 20);
			this.toolStripButton3.Text = "Ram Search";
			this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
			// 
			// HexEditor
			// 
			this.HexEditor.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.poke;
			this.HexEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.HexEditor.Name = "HexEditor";
			this.HexEditor.Size = new System.Drawing.Size(81, 20);
			this.HexEditor.Text = "Hex Editor";
			this.HexEditor.Click += new System.EventHandler(this.HexEditor_Click);
			// 
			// toolStripButton5
			// 
			this.toolStripButton5.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.textdoc;
			this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton5.Name = "toolStripButton5";
			this.toolStripButton5.Size = new System.Drawing.Size(92, 20);
			this.toolStripButton5.Text = "Lua Console";
			this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
			// 
			// toolStripButton6
			// 
			this.toolStripButton6.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.TAStudio;
			this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton6.Name = "toolStripButton6";
			this.toolStripButton6.Size = new System.Drawing.Size(76, 20);
			this.toolStripButton6.Text = "TAStudio";
			this.toolStripButton6.Click += new System.EventHandler(this.TAStudioButton_Click);
			// 
			// TAStudioButton
			// 
			this.TAStudioButton.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.GameController;
			this.TAStudioButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TAStudioButton.Name = "TAStudioButton";
			this.TAStudioButton.Size = new System.Drawing.Size(86, 20);
			this.TAStudioButton.Text = "VirtualPads";
			this.TAStudioButton.Click += new System.EventHandler(this.VirtualPadButton_Click);
			// 
			// NESDebugger
			// 
			this.NESDebugger.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.NESControllerIcon;
			this.NESDebugger.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.NESDebugger.Name = "NESDebugger";
			this.NESDebugger.Size = new System.Drawing.Size(79, 20);
			this.NESDebugger.Text = "Debugger";
			this.NESDebugger.Click += new System.EventHandler(this.NESDebugger_Click);
			// 
			// NESPPU
			// 
			this.NESPPU.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.NESControllerIcon;
			this.NESPPU.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.NESPPU.Name = "NESPPU";
			this.NESPPU.Size = new System.Drawing.Size(49, 20);
			this.NESPPU.Text = "PPU";
			this.NESPPU.Click += new System.EventHandler(this.NESPPU_Click);
			// 
			// NESNameTable
			// 
			this.NESNameTable.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.NESControllerIcon;
			this.NESNameTable.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.NESNameTable.Name = "NESNameTable";
			this.NESNameTable.Size = new System.Drawing.Size(85, 20);
			this.NESNameTable.Text = "Nametable";
			this.NESNameTable.Click += new System.EventHandler(this.NESNameTable_Click);
			// 
			// NESGameGenie
			// 
			this.NESGameGenie.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.NESControllerIcon;
			this.NESGameGenie.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.NESGameGenie.Name = "NESGameGenie";
			this.NESGameGenie.Size = new System.Drawing.Size(91, 20);
			this.NESGameGenie.Text = "Game Genie";
			this.NESGameGenie.Click += new System.EventHandler(this.NESGameGenie_Click);
			// 
			// KeypadTool
			// 
			this.KeypadTool.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.calculator;
			this.KeypadTool.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.KeypadTool.Name = "KeypadTool";
			this.KeypadTool.Size = new System.Drawing.Size(66, 20);
			this.KeypadTool.Text = "Keypad";
			this.KeypadTool.Click += new System.EventHandler(this.KeyPadTool_Click);
			// 
			// SNESGraphicsDebuggerButton
			// 
			this.SNESGraphicsDebuggerButton.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.SNESControllerIcon;
			this.SNESGraphicsDebuggerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SNESGraphicsDebuggerButton.Name = "SNESGraphicsDebuggerButton";
			this.SNESGraphicsDebuggerButton.Size = new System.Drawing.Size(99, 20);
			this.SNESGraphicsDebuggerButton.Text = "Gfx Debugger";
			this.SNESGraphicsDebuggerButton.Click += new System.EventHandler(this.SNESGraphicsDebuggerButton_Click);
			// 
			// SNESGameGenie
			// 
			this.SNESGameGenie.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.SNESControllerIcon;
			this.SNESGameGenie.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SNESGameGenie.Name = "SNESGameGenie";
			this.SNESGameGenie.Size = new System.Drawing.Size(91, 20);
			this.SNESGameGenie.Text = "Game Genie";
			this.SNESGameGenie.Click += new System.EventHandler(this.SNESGameGenie_Click);
			// 
			// GGGameGenie
			// 
			this.GGGameGenie.Image = ((System.Drawing.Image)(resources.GetObject("GGGameGenie.Image")));
			this.GGGameGenie.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.GGGameGenie.Name = "GGGameGenie";
			this.GGGameGenie.Size = new System.Drawing.Size(91, 20);
			this.GGGameGenie.Text = "Game Genie";
			this.GGGameGenie.Click += new System.EventHandler(this.GGGameGenie_Click);
			// 
			// GBGameGenie
			// 
			this.GBGameGenie.Image = ((System.Drawing.Image)(resources.GetObject("GBGameGenie.Image")));
			this.GBGameGenie.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.GBGameGenie.Name = "GBGameGenie";
			this.GBGameGenie.Size = new System.Drawing.Size(91, 20);
			this.GBGameGenie.Text = "Game Genie";
			this.GBGameGenie.Click += new System.EventHandler(this.GBGameGenie_Click);
			// 
			// GameboyDebuggerTool
			// 
			this.GameboyDebuggerTool.Name = "GameboyDebuggerTool";
			this.GameboyDebuggerTool.Size = new System.Drawing.Size(23, 4);
			// 
			// ToolBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(160, 449);
			this.Controls.Add(this.toolStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(129, 62);
			this.Name = "ToolBox";
			this.Text = "Tool Box";
			this.Load += new System.EventHandler(this.ToolBox_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ToolStripEx toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
		private System.Windows.Forms.ToolStripButton HexEditor;
		private System.Windows.Forms.ToolStripButton toolStripButton5;
		private System.Windows.Forms.ToolStripButton NESPPU;
		private System.Windows.Forms.ToolStripButton NESDebugger;
		private System.Windows.Forms.ToolStripButton NESGameGenie;
		private System.Windows.Forms.ToolStripButton NESNameTable;
		private System.Windows.Forms.ToolStripButton KeypadTool;
		private System.Windows.Forms.ToolStripButton TAStudioButton;
		private System.Windows.Forms.ToolStripButton GameboyDebuggerTool;
		private System.Windows.Forms.ToolStripButton SNESGraphicsDebuggerButton;
		private System.Windows.Forms.ToolStripButton SNESGameGenie;
		private System.Windows.Forms.ToolStripButton toolStripButton6;
		private System.Windows.Forms.ToolStripButton GGGameGenie;
		private System.Windows.Forms.ToolStripButton GBGameGenie;

	}
}