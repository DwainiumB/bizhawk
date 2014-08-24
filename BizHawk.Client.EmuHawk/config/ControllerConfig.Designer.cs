﻿namespace BizHawk.Client.EmuHawk
{
	partial class ControllerConfig
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControllerConfig));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.NormalControlsTab = new System.Windows.Forms.TabPage();
			this.AutofireControlsTab = new System.Windows.Forms.TabPage();
			this.AnalogControlsTab = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBoxAutoTab = new System.Windows.Forms.CheckBox();
			this.checkBoxUDLR = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.buttonLoadDefaults = new System.Windows.Forms.Button();
			this.buttonSaveDefaults = new System.Windows.Forms.Button();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.ClearBtn = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.NormalControlsTab);
			this.tabControl1.Controls.Add(this.AutofireControlsTab);
			this.tabControl1.Controls.Add(this.AnalogControlsTab);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(3, 3);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(562, 493);
			this.tabControl1.TabIndex = 1;
			// 
			// NormalControlsTab
			// 
			this.NormalControlsTab.Location = new System.Drawing.Point(4, 22);
			this.NormalControlsTab.Name = "NormalControlsTab";
			this.NormalControlsTab.Padding = new System.Windows.Forms.Padding(3);
			this.NormalControlsTab.Size = new System.Drawing.Size(554, 467);
			this.NormalControlsTab.TabIndex = 0;
			this.NormalControlsTab.Text = "Normal Controls";
			this.NormalControlsTab.UseVisualStyleBackColor = true;
			// 
			// AutofireControlsTab
			// 
			this.AutofireControlsTab.Location = new System.Drawing.Point(4, 22);
			this.AutofireControlsTab.Name = "AutofireControlsTab";
			this.AutofireControlsTab.Padding = new System.Windows.Forms.Padding(3);
			this.AutofireControlsTab.Size = new System.Drawing.Size(554, 467);
			this.AutofireControlsTab.TabIndex = 1;
			this.AutofireControlsTab.Text = "Autofire Controls";
			this.AutofireControlsTab.UseVisualStyleBackColor = true;
			// 
			// AnalogControlsTab
			// 
			this.AnalogControlsTab.Location = new System.Drawing.Point(4, 22);
			this.AnalogControlsTab.Name = "AnalogControlsTab";
			this.AnalogControlsTab.Size = new System.Drawing.Size(554, 467);
			this.AnalogControlsTab.TabIndex = 2;
			this.AnalogControlsTab.Text = "Analog Controls";
			this.AnalogControlsTab.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 519);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(140, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Escape clears a keybinding.";
			// 
			// checkBoxAutoTab
			// 
			this.checkBoxAutoTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxAutoTab.AutoSize = true;
			this.checkBoxAutoTab.Location = new System.Drawing.Point(187, 517);
			this.checkBoxAutoTab.Name = "checkBoxAutoTab";
			this.checkBoxAutoTab.Size = new System.Drawing.Size(70, 17);
			this.checkBoxAutoTab.TabIndex = 3;
			this.checkBoxAutoTab.Text = "Auto Tab";
			this.checkBoxAutoTab.UseVisualStyleBackColor = true;
			this.checkBoxAutoTab.CheckedChanged += new System.EventHandler(this.CheckBoxAutoTab_CheckedChanged);
			// 
			// checkBoxUDLR
			// 
			this.checkBoxUDLR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxUDLR.AutoSize = true;
			this.checkBoxUDLR.Location = new System.Drawing.Point(263, 517);
			this.checkBoxUDLR.Name = "checkBoxUDLR";
			this.checkBoxUDLR.Size = new System.Drawing.Size(101, 17);
			this.checkBoxUDLR.TabIndex = 4;
			this.checkBoxUDLR.Text = "Allow U+D/L+R";
			this.checkBoxUDLR.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(764, 514);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "&Save";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.ButtonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(845, 514);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 340F));
			this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(908, 499);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(571, 23);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 23, 3, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(334, 473);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// buttonLoadDefaults
			// 
			this.buttonLoadDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonLoadDefaults.Location = new System.Drawing.Point(504, 514);
			this.buttonLoadDefaults.Name = "buttonLoadDefaults";
			this.buttonLoadDefaults.Size = new System.Drawing.Size(70, 23);
			this.buttonLoadDefaults.TabIndex = 8;
			this.buttonLoadDefaults.Text = "Defaults";
			this.toolTip1.SetToolTip(this.buttonLoadDefaults, "Set the default controller configuration to the current.  Note: this affects all " +
        "controllers!");
			this.buttonLoadDefaults.UseVisualStyleBackColor = true;
			this.buttonLoadDefaults.Click += new System.EventHandler(this.ButtonLoadDefaults_Click);
			// 
			// buttonSaveDefaults
			// 
			this.buttonSaveDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSaveDefaults.Location = new System.Drawing.Point(374, 514);
			this.buttonSaveDefaults.Name = "buttonSaveDefaults";
			this.buttonSaveDefaults.Size = new System.Drawing.Size(70, 23);
			this.buttonSaveDefaults.TabIndex = 9;
			this.buttonSaveDefaults.Text = "Save Defs";
			this.toolTip1.SetToolTip(this.buttonSaveDefaults, "Save the current configuration as your default controls. Note: this saves ALL con" +
        "troller information!");
			this.buttonSaveDefaults.UseVisualStyleBackColor = true;
			this.buttonSaveDefaults.Click += new System.EventHandler(this.ButtonSaveDefaults_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// ClearBtn
			// 
			this.ClearBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ClearBtn.Location = new System.Drawing.Point(580, 514);
			this.ClearBtn.Name = "ClearBtn";
			this.ClearBtn.Size = new System.Drawing.Size(75, 23);
			this.ClearBtn.TabIndex = 10;
			this.ClearBtn.Text = "&Clear";
			this.ClearBtn.UseVisualStyleBackColor = true;
			this.ClearBtn.Click += new System.EventHandler(this.ClearBtn_Click);
			// 
			// ControllerConfig
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(932, 544);
			this.Controls.Add(this.ClearBtn);
			this.Controls.Add(this.buttonSaveDefaults);
			this.Controls.Add(this.buttonLoadDefaults);
			this.Controls.Add(this.checkBoxUDLR);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.checkBoxAutoTab);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ControllerConfig";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Controller Config";
			this.Load += new System.EventHandler(this.NewControllerConfig_Load);
			this.tabControl1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage NormalControlsTab;
		private System.Windows.Forms.TabPage AutofireControlsTab;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBoxAutoTab;
		private System.Windows.Forms.CheckBox checkBoxUDLR;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button buttonLoadDefaults;
		private System.Windows.Forms.TabPage AnalogControlsTab;
		private System.Windows.Forms.Button buttonSaveDefaults;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button ClearBtn;
	}
}