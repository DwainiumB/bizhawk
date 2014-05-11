﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BizHawk.Client.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class DualGBFileSelector : UserControl
	{
		public string GetName()
		{
			return textBox1.Text;
		}

		public event EventHandler NameChanged;

		private void HandleLabelTextChanged(object sender, EventArgs e)
		{
			this.OnNameChanged(EventArgs.Empty);
		}

		public DualGBFileSelector()
		{
			InitializeComponent();
			textBox1.TextChanged += this.HandleLabelTextChanged;
		}

		protected virtual void OnNameChanged(EventArgs e)
		{
			EventHandler handler = this.NameChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		private void textBox1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
				((string[])e.Data.GetData(DataFormats.FileDrop)).Length == 1)
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void textBox1_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var ff = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (ff.Length == 1)
				{
					textBox1.Text = ff[0];
				}
			}				
		}

		private void button1_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog())
			{
				ofd.InitialDirectory = PathManager.MakeAbsolutePath(Global.Config.PathEntries["GB", "Palettes"].Path, "GB");
				ofd.Filter = "GB Roms (*.gb,*.gbc)|*.gb;*.gbc|All Files|*.*";
				ofd.RestoreDirectory = true;
				var result = ofd.ShowDialog(this);
				if (result == DialogResult.OK)
				{
					textBox1.Text = ofd.FileName;
				}
			}
		}

		private void UseCurrentRomButton_Click(object sender, EventArgs e)
		{
			textBox1.Text = GlobalWin.MainForm.CurrentlyOpenRom;
		}

		private void DualGBFileSelector_Load(object sender, EventArgs e)
		{
			Update();
		}

		public void Update()
		{
			UseCurrentRomButton.Enabled = Global.Emulator != null && // For the designer
				!(Global.Emulator is NullEmulator) &&
				!string.IsNullOrEmpty(GlobalWin.MainForm.CurrentlyOpenRom) &&
				!GlobalWin.MainForm.CurrentlyOpenRom.Contains('|') && // Can't be archive
				!GlobalWin.MainForm.CurrentlyOpenRom.Contains(".xml"); // Can't already be an xml
		}
	}
}
