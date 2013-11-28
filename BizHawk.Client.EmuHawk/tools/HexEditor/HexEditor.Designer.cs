﻿namespace BizHawk.Client.EmuHawk
{
	partial class HexEditor
	{
		private const int fontHeight = 14;

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HexEditor));
			this.menuStrip1 = new MenuStripEx();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.dumpToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.findToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findPrevToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.memoryDomainsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.dataSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DataSizeByteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DataSizeWordMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DataSizeDWordMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.enToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.goToAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addToRamWatchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.freezeAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unfreezeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pokeAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.autoloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.customColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setColorsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.resetToDefaultToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.saveWindowsSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.restoreWindowSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.resetToDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ViewerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CopyContextItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PasteContextItem = new System.Windows.Forms.ToolStripMenuItem();
			this.FreezeContextItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AddToRamWatchContextItem = new System.Windows.Forms.ToolStripMenuItem();
			this.UnfreezeAllContextItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PokeContextItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.IncrementContextItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DecrementContextItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.GoToContextItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MemoryViewerBox = new System.Windows.Forms.GroupBox();
			this.AddressLabel = new System.Windows.Forms.Label();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.AddressesLabel = new System.Windows.Forms.Label();
			this.Header = new System.Windows.Forms.Label();
			this.menuStrip1.SuspendLayout();
			this.ViewerContextMenuStrip.SuspendLayout();
			this.MemoryViewerBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.ClickThrough = true;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.settingsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(584, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsBinaryToolStripMenuItem,
            this.dumpToFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			this.fileToolStripMenuItem.DropDownOpened += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpened);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.SaveAs;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsBinaryToolStripMenuItem
			// 
			this.saveAsBinaryToolStripMenuItem.Name = "saveAsBinaryToolStripMenuItem";
			this.saveAsBinaryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.saveAsBinaryToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
			this.saveAsBinaryToolStripMenuItem.Text = "Save as binary...";
			this.saveAsBinaryToolStripMenuItem.Click += new System.EventHandler(this.saveAsBinaryToolStripMenuItem_Click);
			// 
			// dumpToFileToolStripMenuItem
			// 
			this.dumpToFileToolStripMenuItem.Name = "dumpToFileToolStripMenuItem";
			this.dumpToFileToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
			this.dumpToFileToolStripMenuItem.Text = "Save as text...";
			this.dumpToFileToolStripMenuItem.Click += new System.EventHandler(this.dumpToFileToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(226, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator6,
            this.findToolStripMenuItem1,
            this.findNextToolStripMenuItem,
            this.findPrevToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			this.editToolStripMenuItem.DropDownOpened += new System.EventHandler(this.editToolStripMenuItem_DropDownOpened);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.Duplicate;
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.Paste;
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(149, 6);
			// 
			// findToolStripMenuItem1
			// 
			this.findToolStripMenuItem1.Name = "findToolStripMenuItem1";
			this.findToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.findToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
			this.findToolStripMenuItem1.Text = "&Find...";
			this.findToolStripMenuItem1.Click += new System.EventHandler(this.findToolStripMenuItem1_Click);
			// 
			// findNextToolStripMenuItem
			// 
			this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
			this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.findNextToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.findNextToolStripMenuItem.Text = "Find Next";
			this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
			// 
			// findPrevToolStripMenuItem
			// 
			this.findPrevToolStripMenuItem.Name = "findPrevToolStripMenuItem";
			this.findPrevToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.findPrevToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.findPrevToolStripMenuItem.Text = "Find Prev";
			this.findPrevToolStripMenuItem.Click += new System.EventHandler(this.findPrevToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.memoryDomainsToolStripMenuItem,
            this.dataSizeToolStripMenuItem,
            this.enToolStripMenuItem,
            this.toolStripSeparator2,
            this.goToAddressToolStripMenuItem,
            this.addToRamWatchToolStripMenuItem1,
            this.freezeAddressToolStripMenuItem,
            this.unfreezeAllToolStripMenuItem,
            this.pokeAddressToolStripMenuItem});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionsToolStripMenuItem.Text = "&Options";
			this.optionsToolStripMenuItem.DropDownOpened += new System.EventHandler(this.optionsToolStripMenuItem_DropDownOpened);
			// 
			// memoryDomainsToolStripMenuItem
			// 
			this.memoryDomainsToolStripMenuItem.Name = "memoryDomainsToolStripMenuItem";
			this.memoryDomainsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.memoryDomainsToolStripMenuItem.Text = "&Memory Domains";
			this.memoryDomainsToolStripMenuItem.DropDownOpened += new System.EventHandler(this.memoryDomainsToolStripMenuItem_DropDownOpened);
			// 
			// dataSizeToolStripMenuItem
			// 
			this.dataSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DataSizeByteMenuItem,
            this.DataSizeWordMenuItem,
            this.DataSizeDWordMenuItem});
			this.dataSizeToolStripMenuItem.Name = "dataSizeToolStripMenuItem";
			this.dataSizeToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.dataSizeToolStripMenuItem.Text = "Data Size";
			// 
			// DataSizeByteMenuItem
			// 
			this.DataSizeByteMenuItem.Name = "DataSizeByteMenuItem";
			this.DataSizeByteMenuItem.Size = new System.Drawing.Size(152, 22);
			this.DataSizeByteMenuItem.Text = "1 Byte";
			this.DataSizeByteMenuItem.Click += new System.EventHandler(this.byteToolStripMenuItem_Click);
			// 
			// DataSizeWordMenuItem
			// 
			this.DataSizeWordMenuItem.Name = "DataSizeWordMenuItem";
			this.DataSizeWordMenuItem.Size = new System.Drawing.Size(152, 22);
			this.DataSizeWordMenuItem.Text = "2 Byte";
			this.DataSizeWordMenuItem.Click += new System.EventHandler(this.byteToolStripMenuItem1_Click);
			// 
			// DataSizeDWordMenuItem
			// 
			this.DataSizeDWordMenuItem.Name = "DataSizeDWordMenuItem";
			this.DataSizeDWordMenuItem.Size = new System.Drawing.Size(152, 22);
			this.DataSizeDWordMenuItem.Text = "4 Byte";
			this.DataSizeDWordMenuItem.Click += new System.EventHandler(this.byteToolStripMenuItem2_Click);
			// 
			// enToolStripMenuItem
			// 
			this.enToolStripMenuItem.Name = "enToolStripMenuItem";
			this.enToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.enToolStripMenuItem.Text = "Big Endian";
			this.enToolStripMenuItem.Click += new System.EventHandler(this.enToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(216, 6);
			// 
			// goToAddressToolStripMenuItem
			// 
			this.goToAddressToolStripMenuItem.Name = "goToAddressToolStripMenuItem";
			this.goToAddressToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
			this.goToAddressToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.goToAddressToolStripMenuItem.Text = "&Go to Address...";
			this.goToAddressToolStripMenuItem.Click += new System.EventHandler(this.goToAddressToolStripMenuItem_Click);
			// 
			// addToRamWatchToolStripMenuItem1
			// 
			this.addToRamWatchToolStripMenuItem1.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.FindHS;
			this.addToRamWatchToolStripMenuItem1.Name = "addToRamWatchToolStripMenuItem1";
			this.addToRamWatchToolStripMenuItem1.ShortcutKeyDisplayString = "Ctrl+W";
			this.addToRamWatchToolStripMenuItem1.Size = new System.Drawing.Size(219, 22);
			this.addToRamWatchToolStripMenuItem1.Text = "Add to Ram Watch";
			this.addToRamWatchToolStripMenuItem1.Click += new System.EventHandler(this.addToRamWatchToolStripMenuItem1_Click);
			// 
			// freezeAddressToolStripMenuItem
			// 
			this.freezeAddressToolStripMenuItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.Freeze;
			this.freezeAddressToolStripMenuItem.Name = "freezeAddressToolStripMenuItem";
			this.freezeAddressToolStripMenuItem.ShortcutKeyDisplayString = "Space";
			this.freezeAddressToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.freezeAddressToolStripMenuItem.Text = "&Freeze Address";
			this.freezeAddressToolStripMenuItem.Click += new System.EventHandler(this.freezeAddressToolStripMenuItem_Click);
			// 
			// unfreezeAllToolStripMenuItem
			// 
			this.unfreezeAllToolStripMenuItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.Unfreeze;
			this.unfreezeAllToolStripMenuItem.Name = "unfreezeAllToolStripMenuItem";
			this.unfreezeAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Delete)));
			this.unfreezeAllToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.unfreezeAllToolStripMenuItem.Text = "Unfreeze All";
			this.unfreezeAllToolStripMenuItem.Click += new System.EventHandler(this.unfreezeAllToolStripMenuItem_Click);
			// 
			// pokeAddressToolStripMenuItem
			// 
			this.pokeAddressToolStripMenuItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.poke;
			this.pokeAddressToolStripMenuItem.Name = "pokeAddressToolStripMenuItem";
			this.pokeAddressToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.pokeAddressToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.pokeAddressToolStripMenuItem.Text = "&Poke Address";
			this.pokeAddressToolStripMenuItem.Click += new System.EventHandler(this.pokeAddressToolStripMenuItem_Click);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoloadToolStripMenuItem,
            this.customColorsToolStripMenuItem,
            this.saveWindowsSettingsToolStripMenuItem,
            this.toolStripSeparator3,
            this.alwaysOnTopToolStripMenuItem,
            this.restoreWindowSizeToolStripMenuItem});
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.settingsToolStripMenuItem.Text = "&Settings";
			this.settingsToolStripMenuItem.DropDownOpened += new System.EventHandler(this.settingsToolStripMenuItem_DropDownOpened);
			// 
			// autoloadToolStripMenuItem
			// 
			this.autoloadToolStripMenuItem.Name = "autoloadToolStripMenuItem";
			this.autoloadToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.autoloadToolStripMenuItem.Text = "Autoload";
			this.autoloadToolStripMenuItem.Click += new System.EventHandler(this.autoloadToolStripMenuItem_Click);
			// 
			// customColorsToolStripMenuItem
			// 
			this.customColorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setColorsToolStripMenuItem1,
            this.toolStripSeparator8,
            this.resetToDefaultToolStripMenuItem1});
			this.customColorsToolStripMenuItem.Name = "customColorsToolStripMenuItem";
			this.customColorsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.customColorsToolStripMenuItem.Text = "Custom Colors";
			// 
			// setColorsToolStripMenuItem1
			// 
			this.setColorsToolStripMenuItem1.Name = "setColorsToolStripMenuItem1";
			this.setColorsToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
			this.setColorsToolStripMenuItem1.Text = "Set Colors";
			this.setColorsToolStripMenuItem1.Click += new System.EventHandler(this.setColorsToolStripMenuItem1_Click);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(154, 6);
			// 
			// resetToDefaultToolStripMenuItem1
			// 
			this.resetToDefaultToolStripMenuItem1.Name = "resetToDefaultToolStripMenuItem1";
			this.resetToDefaultToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
			this.resetToDefaultToolStripMenuItem1.Text = "Reset to Default";
			this.resetToDefaultToolStripMenuItem1.Click += new System.EventHandler(this.resetToDefaultToolStripMenuItem1_Click);
			// 
			// saveWindowsSettingsToolStripMenuItem
			// 
			this.saveWindowsSettingsToolStripMenuItem.Name = "saveWindowsSettingsToolStripMenuItem";
			this.saveWindowsSettingsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.saveWindowsSettingsToolStripMenuItem.Text = "Save windows settings";
			this.saveWindowsSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveWindowsSettingsToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(189, 6);
			// 
			// alwaysOnTopToolStripMenuItem
			// 
			this.alwaysOnTopToolStripMenuItem.Name = "alwaysOnTopToolStripMenuItem";
			this.alwaysOnTopToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.alwaysOnTopToolStripMenuItem.Text = "Always On Top";
			this.alwaysOnTopToolStripMenuItem.Click += new System.EventHandler(this.alwaysOnTopToolStripMenuItem_Click);
			// 
			// restoreWindowSizeToolStripMenuItem
			// 
			this.restoreWindowSizeToolStripMenuItem.Name = "restoreWindowSizeToolStripMenuItem";
			this.restoreWindowSizeToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.restoreWindowSizeToolStripMenuItem.Text = "&Restore Window Size";
			this.restoreWindowSizeToolStripMenuItem.Click += new System.EventHandler(this.restoreWindowSizeToolStripMenuItem_Click);
			// 
			// setColorsToolStripMenuItem
			// 
			this.setColorsToolStripMenuItem.Name = "setColorsToolStripMenuItem";
			this.setColorsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.setColorsToolStripMenuItem.Text = "Set Colors";
			this.setColorsToolStripMenuItem.Click += new System.EventHandler(this.setColorsToolStripMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(154, 6);
			// 
			// resetToDefaultToolStripMenuItem
			// 
			this.resetToDefaultToolStripMenuItem.Name = "resetToDefaultToolStripMenuItem";
			this.resetToDefaultToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.resetToDefaultToolStripMenuItem.Text = "Reset to Default";
			// 
			// ViewerContextMenuStrip
			// 
			this.ViewerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CopyContextItem,
            this.PasteContextItem,
            this.FreezeContextItem,
            this.AddToRamWatchContextItem,
            this.UnfreezeAllContextItem,
            this.PokeContextItem,
            this.ContextSeparator1,
            this.IncrementContextItem,
            this.DecrementContextItem,
            this.ContextSeparator2,
            this.GoToContextItem});
			this.ViewerContextMenuStrip.Name = "ViewerContextMenuStrip";
			this.ViewerContextMenuStrip.Size = new System.Drawing.Size(220, 214);
			this.ViewerContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ViewerContextMenuStrip_Opening);
			// 
			// CopyContextItem
			// 
			this.CopyContextItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.Duplicate;
			this.CopyContextItem.Name = "CopyContextItem";
			this.CopyContextItem.ShortcutKeyDisplayString = "Ctrl+C";
			this.CopyContextItem.Size = new System.Drawing.Size(219, 22);
			this.CopyContextItem.Text = "&Copy";
			this.CopyContextItem.Click += new System.EventHandler(this.copyToolStripMenuItem1_Click);
			// 
			// PasteContextItem
			// 
			this.PasteContextItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.Paste;
			this.PasteContextItem.Name = "PasteContextItem";
			this.PasteContextItem.ShortcutKeyDisplayString = "Ctrl+V";
			this.PasteContextItem.Size = new System.Drawing.Size(219, 22);
			this.PasteContextItem.Text = "&Paste";
			this.PasteContextItem.Click += new System.EventHandler(this.pasteToolStripMenuItem1_Click);
			// 
			// FreezeContextItem
			// 
			this.FreezeContextItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.Freeze;
			this.FreezeContextItem.Name = "FreezeContextItem";
			this.FreezeContextItem.ShortcutKeyDisplayString = "Space";
			this.FreezeContextItem.Size = new System.Drawing.Size(219, 22);
			this.FreezeContextItem.Text = "&Freeze";
			this.FreezeContextItem.Click += new System.EventHandler(this.freezeToolStripMenuItem_Click);
			// 
			// AddToRamWatchContextItem
			// 
			this.AddToRamWatchContextItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.FindHS;
			this.AddToRamWatchContextItem.Name = "AddToRamWatchContextItem";
			this.AddToRamWatchContextItem.ShortcutKeyDisplayString = "Ctrl+W";
			this.AddToRamWatchContextItem.Size = new System.Drawing.Size(219, 22);
			this.AddToRamWatchContextItem.Text = "&Add to Ram Watch";
			this.AddToRamWatchContextItem.Click += new System.EventHandler(this.addToRamWatchToolStripMenuItem_Click);
			// 
			// UnfreezeAllContextItem
			// 
			this.UnfreezeAllContextItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.Unfreeze;
			this.UnfreezeAllContextItem.Name = "UnfreezeAllContextItem";
			this.UnfreezeAllContextItem.ShortcutKeyDisplayString = "Shift+Del";
			this.UnfreezeAllContextItem.Size = new System.Drawing.Size(219, 22);
			this.UnfreezeAllContextItem.Text = "&Unfreeze All";
			this.UnfreezeAllContextItem.Click += new System.EventHandler(this.unfreezeAllToolStripMenuItem1_Click);
			// 
			// PokeContextItem
			// 
			this.PokeContextItem.Image = global::BizHawk.Client.EmuHawk.Properties.Resources.poke;
			this.PokeContextItem.Name = "PokeContextItem";
			this.PokeContextItem.ShortcutKeyDisplayString = "Ctrl+P";
			this.PokeContextItem.Size = new System.Drawing.Size(219, 22);
			this.PokeContextItem.Text = "&Poke Address";
			this.PokeContextItem.Click += new System.EventHandler(this.pokeAddressToolStripMenuItem1_Click);
			// 
			// ContextSeparator1
			// 
			this.ContextSeparator1.Name = "ContextSeparator1";
			this.ContextSeparator1.Size = new System.Drawing.Size(216, 6);
			// 
			// IncrementContextItem
			// 
			this.IncrementContextItem.Name = "IncrementContextItem";
			this.IncrementContextItem.ShortcutKeyDisplayString = "+";
			this.IncrementContextItem.Size = new System.Drawing.Size(219, 22);
			this.IncrementContextItem.Text = "&Increment";
			this.IncrementContextItem.Click += new System.EventHandler(this.incrementToolStripMenuItem_Click);
			// 
			// DecrementContextItem
			// 
			this.DecrementContextItem.Name = "DecrementContextItem";
			this.DecrementContextItem.ShortcutKeyDisplayString = "-";
			this.DecrementContextItem.Size = new System.Drawing.Size(219, 22);
			this.DecrementContextItem.Text = "&Decrement";
			this.DecrementContextItem.Click += new System.EventHandler(this.decrementToolStripMenuItem_Click);
			// 
			// ContextSeparator2
			// 
			this.ContextSeparator2.Name = "ContextSeparator2";
			this.ContextSeparator2.Size = new System.Drawing.Size(216, 6);
			// 
			// GoToContextItem
			// 
			this.GoToContextItem.Name = "GoToContextItem";
			this.GoToContextItem.ShortcutKeyDisplayString = "Ctrl+G";
			this.GoToContextItem.Size = new System.Drawing.Size(219, 22);
			this.GoToContextItem.Text = "&Go to Address...";
			this.GoToContextItem.Click += new System.EventHandler(this.gotoAddressToolStripMenuItem1_Click);
			// 
			// MemoryViewerBox
			// 
			this.MemoryViewerBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MemoryViewerBox.ContextMenuStrip = this.ViewerContextMenuStrip;
			this.MemoryViewerBox.Controls.Add(this.AddressLabel);
			this.MemoryViewerBox.Controls.Add(this.vScrollBar1);
			this.MemoryViewerBox.Controls.Add(this.AddressesLabel);
			this.MemoryViewerBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MemoryViewerBox.Location = new System.Drawing.Point(12, 27);
			this.MemoryViewerBox.MaximumSize = new System.Drawing.Size(600, 1024);
			this.MemoryViewerBox.MinimumSize = new System.Drawing.Size(260, 180);
			this.MemoryViewerBox.Name = "MemoryViewerBox";
			this.MemoryViewerBox.Size = new System.Drawing.Size(558, 262);
			this.MemoryViewerBox.TabIndex = 2;
			this.MemoryViewerBox.TabStop = false;
			this.MemoryViewerBox.Paint += new System.Windows.Forms.PaintEventHandler(this.MemoryViewerBox_Paint);
			// 
			// AddressLabel
			// 
			this.AddressLabel.AutoSize = true;
			this.AddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AddressLabel.Location = new System.Drawing.Point(3, 30);
			this.AddressLabel.Name = "AddressLabel";
			this.AddressLabel.Size = new System.Drawing.Size(25, 13);
			this.AddressLabel.TabIndex = 2;
			this.AddressLabel.Text = "      ";
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.vScrollBar1.LargeChange = 16;
			this.vScrollBar1.Location = new System.Drawing.Point(539, 8);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(16, 251);
			this.vScrollBar1.TabIndex = 1;
			this.vScrollBar1.ValueChanged += new System.EventHandler(this.vScrollBar1_ValueChanged);
			// 
			// AddressesLabel
			// 
			this.AddressesLabel.AutoSize = true;
			this.AddressesLabel.ContextMenuStrip = this.ViewerContextMenuStrip;
			this.AddressesLabel.Location = new System.Drawing.Point(65, 30);
			this.AddressesLabel.Name = "AddressesLabel";
			this.AddressesLabel.Size = new System.Drawing.Size(31, 13);
			this.AddressesLabel.TabIndex = 0;
			this.AddressesLabel.Text = "RAM";
			this.AddressesLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AddressesLabel_MouseClick);
			this.AddressesLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AddressesLabel_MouseDown);
			this.AddressesLabel.MouseLeave += new System.EventHandler(this.AddressesLabel_MouseLeave);
			this.AddressesLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AddressesLabel_MouseMove);
			this.AddressesLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AddressesLabel_MouseUp);
			// 
			// Header
			// 
			this.Header.AutoSize = true;
			this.Header.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Header.Location = new System.Drawing.Point(28, 44);
			this.Header.Name = "Header";
			this.Header.Size = new System.Drawing.Size(35, 13);
			this.Header.TabIndex = 2;
			this.Header.Text = "label1";
			// 
			// HexEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 301);
			this.Controls.Add(this.Header);
			this.Controls.Add(this.MemoryViewerBox);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.MinimumSize = new System.Drawing.Size(360, 180);
			this.Name = "HexEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "HexEditor";
			this.Load += new System.EventHandler(this.HexEditor_Load);
			this.ResizeEnd += new System.EventHandler(this.HexEditor_ResizeEnd);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HexEditor_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HexEditor_KeyUp);
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.HexEditor_MouseWheel);
			this.Resize += new System.EventHandler(this.HexEditor_Resize);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ViewerContextMenuStrip.ResumeLayout(false);
			this.MemoryViewerBox.ResumeLayout(false);
			this.MemoryViewerBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public MenuStripEx menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dumpToFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem memoryDomainsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dataSizeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem DataSizeByteMenuItem;
		private System.Windows.Forms.ToolStripMenuItem DataSizeWordMenuItem;
		private System.Windows.Forms.ToolStripMenuItem DataSizeDWordMenuItem;
		private System.Windows.Forms.ToolStripMenuItem goToAddressToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem restoreWindowSizeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem autoloadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem enToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ViewerContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem FreezeContextItem;
		private System.Windows.Forms.ToolStripMenuItem AddToRamWatchContextItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem addToRamWatchToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem saveWindowsSettingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem freezeAddressToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		public System.Windows.Forms.GroupBox MemoryViewerBox;
		private System.Windows.Forms.Label AddressesLabel;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.ToolStripMenuItem unfreezeAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem UnfreezeAllContextItem;
		private System.Windows.Forms.ToolStripSeparator ContextSeparator1;
		private System.Windows.Forms.ToolStripMenuItem IncrementContextItem;
		private System.Windows.Forms.ToolStripMenuItem DecrementContextItem;
		private System.Windows.Forms.ToolStripMenuItem GoToContextItem;
		private System.Windows.Forms.ToolStripSeparator ContextSeparator2;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem saveAsBinaryToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setColorsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem resetToDefaultToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem customColorsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setColorsToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem resetToDefaultToolStripMenuItem1;
		public System.Windows.Forms.Label Header;
		private System.Windows.Forms.Label AddressLabel;
		private System.Windows.Forms.ToolStripMenuItem CopyContextItem;
		private System.Windows.Forms.ToolStripMenuItem PasteContextItem;
		private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findPrevToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pokeAddressToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PokeContextItem;
		private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
	}
}