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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControllerConfig));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBoxAutoTab = new System.Windows.Forms.CheckBox();
			this.checkBoxUDLR = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.buttonLoadDefaults = new System.Windows.Forms.Button();
			this.buttonSaveDefaults = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(3, 3);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(562, 493);
			this.tabControl1.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(554, 467);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Normal Controls";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(554, 467);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Autofire Controls";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tabPage3
			// 
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(554, 467);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Analog Controls";
			this.tabPage3.UseVisualStyleBackColor = true;
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
			this.checkBoxAutoTab.CheckedChanged += new System.EventHandler(this.checkBoxAutoTab_CheckedChanged);
			// 
			// checkBoxUDLR
			// 
			this.checkBoxUDLR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxUDLR.AutoSize = true;
			this.checkBoxUDLR.Location = new System.Drawing.Point(263, 517);
			this.checkBoxUDLR.Name = "checkBoxUDLR";
			this.checkBoxUDLR.Size = new System.Drawing.Size(84, 17);
			this.checkBoxUDLR.TabIndex = 4;
			this.checkBoxUDLR.Text = "Allow UDLR";
			this.checkBoxUDLR.UseVisualStyleBackColor = true;
			this.checkBoxUDLR.CheckedChanged += new System.EventHandler(this.checkBoxUDLR_CheckedChanged);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(764, 514);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(845, 514);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
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
			this.buttonLoadDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonLoadDefaults.Location = new System.Drawing.Point(683, 514);
			this.buttonLoadDefaults.Name = "buttonLoadDefaults";
			this.buttonLoadDefaults.Size = new System.Drawing.Size(75, 23);
			this.buttonLoadDefaults.TabIndex = 8;
			this.buttonLoadDefaults.Text = "Defaults";
			this.buttonLoadDefaults.UseVisualStyleBackColor = true;
			this.buttonLoadDefaults.Click += new System.EventHandler(this.buttonLoadDefaults_Click);
			// 
			// buttonSaveDefaults
			// 
			this.buttonSaveDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSaveDefaults.Location = new System.Drawing.Point(602, 514);
			this.buttonSaveDefaults.Name = "buttonSaveDefaults";
			this.buttonSaveDefaults.Size = new System.Drawing.Size(75, 23);
			this.buttonSaveDefaults.TabIndex = 9;
			this.buttonSaveDefaults.Text = "Save Defs";
			this.buttonSaveDefaults.UseVisualStyleBackColor = true;
			this.buttonSaveDefaults.Click += new System.EventHandler(this.buttonSaveDefaults_Click);
			// 
			// NewControllerConfig
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(932, 544);
			this.Controls.Add(this.buttonSaveDefaults);
			this.Controls.Add(this.buttonLoadDefaults);
			this.Controls.Add(this.checkBoxUDLR);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.checkBoxAutoTab);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "NewControllerConfig";
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
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBoxAutoTab;
		private System.Windows.Forms.CheckBox checkBoxUDLR;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button buttonLoadDefaults;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Button buttonSaveDefaults;
	}
}