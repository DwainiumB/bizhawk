﻿namespace BizHawk.MultiClient
{
	partial class RewindConfig
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RewindConfig));
			this.OK = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.SmallLabel1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label10 = new System.Windows.Forms.Label();
			this.LargeStateEnabledBox = new System.Windows.Forms.CheckBox();
			this.MediumStateEnabledBox = new System.Windows.Forms.CheckBox();
			this.SmallStateEnabledBox = new System.Windows.Forms.CheckBox();
			this.LargeLabel2 = new System.Windows.Forms.Label();
			this.LargeLabel3 = new System.Windows.Forms.Label();
			this.LargeSavestateNumeric = new System.Windows.Forms.NumericUpDown();
			this.LargeLabel1 = new System.Windows.Forms.Label();
			this.MediumLabel2 = new System.Windows.Forms.Label();
			this.MediumLabel3 = new System.Windows.Forms.Label();
			this.MediumSavestateNumeric = new System.Windows.Forms.NumericUpDown();
			this.MediumLabel1 = new System.Windows.Forms.Label();
			this.SmallLabel2 = new System.Windows.Forms.Label();
			this.SmallLabel3 = new System.Windows.Forms.Label();
			this.SmallSavestateNumeric = new System.Windows.Forms.NumericUpDown();
			this.UseDeltaCompression = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.StateSizeLabel = new System.Windows.Forms.Label();
			this.MediumStateTrackbar = new System.Windows.Forms.TrackBar();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.LargeStateUpDown = new System.Windows.Forms.NumericUpDown();
			this.MediumStateUpDown = new System.Windows.Forms.NumericUpDown();
			this.LargeStateSizeLabel = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.LargeStateTrackbar = new System.Windows.Forms.TrackBar();
			this.MediumStateSizeLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.BufferSizeUpDown = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.DiskBufferCheckbox = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.LargeSavestateNumeric)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MediumSavestateNumeric)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SmallSavestateNumeric)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MediumStateTrackbar)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.LargeStateUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MediumStateUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.LargeStateTrackbar)).BeginInit();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.BufferSizeUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// OK
			// 
			this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OK.Location = new System.Drawing.Point(215, 369);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(75, 23);
			this.OK.TabIndex = 0;
			this.OK.Text = "&Ok";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new System.EventHandler(this.OK_Click);
			// 
			// Cancel
			// 
			this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new System.Drawing.Point(296, 369);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(75, 23);
			this.Cancel.TabIndex = 1;
			this.Cancel.Text = "&Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// SmallLabel1
			// 
			this.SmallLabel1.AutoSize = true;
			this.SmallLabel1.Location = new System.Drawing.Point(40, 40);
			this.SmallLabel1.Name = "SmallLabel1";
			this.SmallLabel1.Size = new System.Drawing.Size(164, 13);
			this.SmallLabel1.TabIndex = 2;
			this.SmallLabel1.Text = "Small savestates (less than 32kb)";
			this.SmallLabel1.Click += new System.EventHandler(this.SmallLabel1_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.LargeStateEnabledBox);
			this.groupBox1.Controls.Add(this.MediumStateEnabledBox);
			this.groupBox1.Controls.Add(this.SmallStateEnabledBox);
			this.groupBox1.Controls.Add(this.LargeLabel2);
			this.groupBox1.Controls.Add(this.LargeLabel3);
			this.groupBox1.Controls.Add(this.LargeSavestateNumeric);
			this.groupBox1.Controls.Add(this.LargeLabel1);
			this.groupBox1.Controls.Add(this.MediumLabel2);
			this.groupBox1.Controls.Add(this.MediumLabel3);
			this.groupBox1.Controls.Add(this.MediumSavestateNumeric);
			this.groupBox1.Controls.Add(this.MediumLabel1);
			this.groupBox1.Controls.Add(this.SmallLabel2);
			this.groupBox1.Controls.Add(this.SmallLabel3);
			this.groupBox1.Controls.Add(this.SmallSavestateNumeric);
			this.groupBox1.Controls.Add(this.SmallLabel1);
			this.groupBox1.Location = new System.Drawing.Point(12, 128);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(359, 118);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Rewind frequency";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6, 22);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(40, 13);
			this.label10.TabIndex = 17;
			this.label10.Text = "Enable";
			// 
			// LargeStateEnabledBox
			// 
			this.LargeStateEnabledBox.AutoSize = true;
			this.LargeStateEnabledBox.Location = new System.Drawing.Point(9, 87);
			this.LargeStateEnabledBox.Name = "LargeStateEnabledBox";
			this.LargeStateEnabledBox.Size = new System.Drawing.Size(15, 14);
			this.LargeStateEnabledBox.TabIndex = 16;
			this.LargeStateEnabledBox.UseVisualStyleBackColor = true;
			this.LargeStateEnabledBox.CheckStateChanged += new System.EventHandler(this.LargeStateEnabledBox_CheckStateChanged);
			// 
			// MediumStateEnabledBox
			// 
			this.MediumStateEnabledBox.AutoSize = true;
			this.MediumStateEnabledBox.Location = new System.Drawing.Point(9, 63);
			this.MediumStateEnabledBox.Name = "MediumStateEnabledBox";
			this.MediumStateEnabledBox.Size = new System.Drawing.Size(15, 14);
			this.MediumStateEnabledBox.TabIndex = 15;
			this.MediumStateEnabledBox.UseVisualStyleBackColor = true;
			this.MediumStateEnabledBox.CheckStateChanged += new System.EventHandler(this.MediumStateEnabledBox_CheckStateChanged);
			// 
			// SmallStateEnabledBox
			// 
			this.SmallStateEnabledBox.AutoSize = true;
			this.SmallStateEnabledBox.Location = new System.Drawing.Point(9, 39);
			this.SmallStateEnabledBox.Name = "SmallStateEnabledBox";
			this.SmallStateEnabledBox.Size = new System.Drawing.Size(15, 14);
			this.SmallStateEnabledBox.TabIndex = 14;
			this.SmallStateEnabledBox.UseVisualStyleBackColor = true;
			this.SmallStateEnabledBox.CheckStateChanged += new System.EventHandler(this.SmallStateEnabledBox_CheckStateChanged);
			// 
			// LargeLabel2
			// 
			this.LargeLabel2.AutoSize = true;
			this.LargeLabel2.Location = new System.Drawing.Point(227, 88);
			this.LargeLabel2.Name = "LargeLabel2";
			this.LargeLabel2.Size = new System.Drawing.Size(33, 13);
			this.LargeLabel2.TabIndex = 13;
			this.LargeLabel2.Text = "every";
			// 
			// LargeLabel3
			// 
			this.LargeLabel3.AutoSize = true;
			this.LargeLabel3.Location = new System.Drawing.Point(307, 88);
			this.LargeLabel3.Name = "LargeLabel3";
			this.LargeLabel3.Size = new System.Drawing.Size(38, 13);
			this.LargeLabel3.TabIndex = 12;
			this.LargeLabel3.Text = "frames";
			// 
			// LargeSavestateNumeric
			// 
			this.LargeSavestateNumeric.Location = new System.Drawing.Point(263, 84);
			this.LargeSavestateNumeric.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
			this.LargeSavestateNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.LargeSavestateNumeric.Name = "LargeSavestateNumeric";
			this.LargeSavestateNumeric.Size = new System.Drawing.Size(38, 20);
			this.LargeSavestateNumeric.TabIndex = 11;
			this.LargeSavestateNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// LargeLabel1
			// 
			this.LargeLabel1.AutoSize = true;
			this.LargeLabel1.Location = new System.Drawing.Point(40, 88);
			this.LargeLabel1.Name = "LargeLabel1";
			this.LargeLabel1.Size = new System.Drawing.Size(177, 13);
			this.LargeLabel1.TabIndex = 10;
			this.LargeLabel1.Text = "Large savestates (more than 100kb)";
			this.LargeLabel1.Click += new System.EventHandler(this.LargeLabel1_Click);
			// 
			// MediumLabel2
			// 
			this.MediumLabel2.AutoSize = true;
			this.MediumLabel2.Location = new System.Drawing.Point(227, 64);
			this.MediumLabel2.Name = "MediumLabel2";
			this.MediumLabel2.Size = new System.Drawing.Size(33, 13);
			this.MediumLabel2.TabIndex = 9;
			this.MediumLabel2.Text = "every";
			// 
			// MediumLabel3
			// 
			this.MediumLabel3.AutoSize = true;
			this.MediumLabel3.Location = new System.Drawing.Point(307, 64);
			this.MediumLabel3.Name = "MediumLabel3";
			this.MediumLabel3.Size = new System.Drawing.Size(38, 13);
			this.MediumLabel3.TabIndex = 8;
			this.MediumLabel3.Text = "frames";
			// 
			// MediumSavestateNumeric
			// 
			this.MediumSavestateNumeric.Location = new System.Drawing.Point(263, 60);
			this.MediumSavestateNumeric.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
			this.MediumSavestateNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.MediumSavestateNumeric.Name = "MediumSavestateNumeric";
			this.MediumSavestateNumeric.Size = new System.Drawing.Size(38, 20);
			this.MediumSavestateNumeric.TabIndex = 7;
			this.MediumSavestateNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// MediumLabel1
			// 
			this.MediumLabel1.AutoSize = true;
			this.MediumLabel1.Location = new System.Drawing.Point(40, 64);
			this.MediumLabel1.Name = "MediumLabel1";
			this.MediumLabel1.Size = new System.Drawing.Size(158, 13);
			this.MediumLabel1.TabIndex = 6;
			this.MediumLabel1.Text = "Medium savestates (32 - 100kb)";
			this.MediumLabel1.Click += new System.EventHandler(this.MediumLabel1_Click);
			// 
			// SmallLabel2
			// 
			this.SmallLabel2.AutoSize = true;
			this.SmallLabel2.Location = new System.Drawing.Point(227, 40);
			this.SmallLabel2.Name = "SmallLabel2";
			this.SmallLabel2.Size = new System.Drawing.Size(33, 13);
			this.SmallLabel2.TabIndex = 5;
			this.SmallLabel2.Text = "every";
			// 
			// SmallLabel3
			// 
			this.SmallLabel3.AutoSize = true;
			this.SmallLabel3.Location = new System.Drawing.Point(307, 40);
			this.SmallLabel3.Name = "SmallLabel3";
			this.SmallLabel3.Size = new System.Drawing.Size(38, 13);
			this.SmallLabel3.TabIndex = 4;
			this.SmallLabel3.Text = "frames";
			// 
			// SmallSavestateNumeric
			// 
			this.SmallSavestateNumeric.Location = new System.Drawing.Point(263, 36);
			this.SmallSavestateNumeric.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
			this.SmallSavestateNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.SmallSavestateNumeric.Name = "SmallSavestateNumeric";
			this.SmallSavestateNumeric.Size = new System.Drawing.Size(38, 20);
			this.SmallSavestateNumeric.TabIndex = 3;
			this.SmallSavestateNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// UseDeltaCompression
			// 
			this.UseDeltaCompression.AutoSize = true;
			this.UseDeltaCompression.Location = new System.Drawing.Point(9, 39);
			this.UseDeltaCompression.Name = "UseDeltaCompression";
			this.UseDeltaCompression.Size = new System.Drawing.Size(133, 17);
			this.UseDeltaCompression.TabIndex = 4;
			this.UseDeltaCompression.Text = "Use delta compression";
			this.UseDeltaCompression.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Current Savestate Size:";
			// 
			// StateSizeLabel
			// 
			this.StateSizeLabel.AutoSize = true;
			this.StateSizeLabel.Location = new System.Drawing.Point(142, 9);
			this.StateSizeLabel.Name = "StateSizeLabel";
			this.StateSizeLabel.Size = new System.Drawing.Size(28, 13);
			this.StateSizeLabel.TabIndex = 6;
			this.StateSizeLabel.Text = "0 kb";
			// 
			// MediumStateTrackbar
			// 
			this.MediumStateTrackbar.LargeChange = 256;
			this.MediumStateTrackbar.Location = new System.Drawing.Point(71, 19);
			this.MediumStateTrackbar.Maximum = 4096;
			this.MediumStateTrackbar.Minimum = 1;
			this.MediumStateTrackbar.Name = "MediumStateTrackbar";
			this.MediumStateTrackbar.Size = new System.Drawing.Size(186, 45);
			this.MediumStateTrackbar.TabIndex = 7;
			this.MediumStateTrackbar.TickFrequency = 256;
			this.MediumStateTrackbar.Value = 1;
			this.MediumStateTrackbar.ValueChanged += new System.EventHandler(this.MediumStateTrackbar_ValueChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.LargeStateUpDown);
			this.groupBox2.Controls.Add(this.MediumStateUpDown);
			this.groupBox2.Controls.Add(this.LargeStateSizeLabel);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.LargeStateTrackbar);
			this.groupBox2.Controls.Add(this.MediumStateSizeLabel);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.MediumStateTrackbar);
			this.groupBox2.Location = new System.Drawing.Point(12, 252);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(359, 105);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "State size definition";
			// 
			// LargeStateUpDown
			// 
			this.LargeStateUpDown.Location = new System.Drawing.Point(263, 64);
			this.LargeStateUpDown.Maximum = new decimal(new int[] {
            16384,
            0,
            0,
            0});
			this.LargeStateUpDown.Minimum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.LargeStateUpDown.Name = "LargeStateUpDown";
			this.LargeStateUpDown.Size = new System.Drawing.Size(52, 20);
			this.LargeStateUpDown.TabIndex = 14;
			this.LargeStateUpDown.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.LargeStateUpDown.ValueChanged += new System.EventHandler(this.LargeStateUpDown_ValueChanged);
			// 
			// MediumStateUpDown
			// 
			this.MediumStateUpDown.Location = new System.Drawing.Point(263, 28);
			this.MediumStateUpDown.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
			this.MediumStateUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.MediumStateUpDown.Name = "MediumStateUpDown";
			this.MediumStateUpDown.Size = new System.Drawing.Size(52, 20);
			this.MediumStateUpDown.TabIndex = 13;
			this.MediumStateUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.MediumStateUpDown.ValueChanged += new System.EventHandler(this.MediumStateUpDown_ValueChanged);
			// 
			// LargeStateSizeLabel
			// 
			this.LargeStateSizeLabel.AutoSize = true;
			this.LargeStateSizeLabel.Location = new System.Drawing.Point(316, 68);
			this.LargeStateSizeLabel.Name = "LargeStateSizeLabel";
			this.LargeStateSizeLabel.Size = new System.Drawing.Size(19, 13);
			this.LargeStateSizeLabel.TabIndex = 12;
			this.LargeStateSizeLabel.Text = "kb";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(11, 71);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(34, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "Large";
			// 
			// LargeStateTrackbar
			// 
			this.LargeStateTrackbar.LargeChange = 1024;
			this.LargeStateTrackbar.Location = new System.Drawing.Point(71, 55);
			this.LargeStateTrackbar.Maximum = 16384;
			this.LargeStateTrackbar.Minimum = 256;
			this.LargeStateTrackbar.Name = "LargeStateTrackbar";
			this.LargeStateTrackbar.Size = new System.Drawing.Size(186, 45);
			this.LargeStateTrackbar.TabIndex = 10;
			this.LargeStateTrackbar.TickFrequency = 1024;
			this.LargeStateTrackbar.Value = 256;
			this.LargeStateTrackbar.ValueChanged += new System.EventHandler(this.LargeStateTrackbar_ValueChanged);
			// 
			// MediumStateSizeLabel
			// 
			this.MediumStateSizeLabel.AutoSize = true;
			this.MediumStateSizeLabel.Location = new System.Drawing.Point(317, 32);
			this.MediumStateSizeLabel.Name = "MediumStateSizeLabel";
			this.MediumStateSizeLabel.Size = new System.Drawing.Size(19, 13);
			this.MediumStateSizeLabel.TabIndex = 9;
			this.MediumStateSizeLabel.Text = "kb";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Medium";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.BufferSizeUpDown);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.DiskBufferCheckbox);
			this.groupBox3.Controls.Add(this.UseDeltaCompression);
			this.groupBox3.Location = new System.Drawing.Point(12, 25);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(359, 97);
			this.groupBox3.TabIndex = 9;
			this.groupBox3.TabStop = false;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(148, 70);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(21, 13);
			this.label4.TabIndex = 16;
			this.label4.Text = "mb";
			// 
			// BufferSizeUpDown
			// 
			this.BufferSizeUpDown.Location = new System.Drawing.Point(90, 68);
			this.BufferSizeUpDown.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
			this.BufferSizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.BufferSizeUpDown.Name = "BufferSizeUpDown";
			this.BufferSizeUpDown.Size = new System.Drawing.Size(52, 20);
			this.BufferSizeUpDown.TabIndex = 15;
			this.BufferSizeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(11, 70);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(81, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Max buffer size:";
			// 
			// DiskBufferCheckbox
			// 
			this.DiskBufferCheckbox.AutoSize = true;
			this.DiskBufferCheckbox.Location = new System.Drawing.Point(9, 16);
			this.DiskBufferCheckbox.Name = "DiskBufferCheckbox";
			this.DiskBufferCheckbox.Size = new System.Drawing.Size(188, 17);
			this.DiskBufferCheckbox.TabIndex = 5;
			this.DiskBufferCheckbox.Text = "Use disk for buffer instead of RAM";
			this.DiskBufferCheckbox.UseVisualStyleBackColor = true;
			// 
			// RewindConfig
			// 
			this.AcceptButton = this.OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel;
			this.ClientSize = new System.Drawing.Size(383, 404);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.StateSizeLabel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.OK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RewindConfig";
			this.Text = "Rewind Settings";
			this.Load += new System.EventHandler(this.RewindConfig_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.LargeSavestateNumeric)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MediumSavestateNumeric)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SmallSavestateNumeric)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MediumStateTrackbar)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.LargeStateUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MediumStateUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.LargeStateTrackbar)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.BufferSizeUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.Label SmallLabel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label SmallLabel2;
		private System.Windows.Forms.Label SmallLabel3;
		private System.Windows.Forms.NumericUpDown SmallSavestateNumeric;
		private System.Windows.Forms.Label LargeLabel2;
		private System.Windows.Forms.Label LargeLabel3;
		private System.Windows.Forms.NumericUpDown LargeSavestateNumeric;
		private System.Windows.Forms.Label LargeLabel1;
		private System.Windows.Forms.Label MediumLabel2;
		private System.Windows.Forms.Label MediumLabel3;
		private System.Windows.Forms.NumericUpDown MediumSavestateNumeric;
		private System.Windows.Forms.Label MediumLabel1;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox LargeStateEnabledBox;
		private System.Windows.Forms.CheckBox MediumStateEnabledBox;
		private System.Windows.Forms.CheckBox SmallStateEnabledBox;
		private System.Windows.Forms.CheckBox UseDeltaCompression;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label StateSizeLabel;
		private System.Windows.Forms.TrackBar MediumStateTrackbar;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label MediumStateSizeLabel;
		private System.Windows.Forms.Label LargeStateSizeLabel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TrackBar LargeStateTrackbar;
		private System.Windows.Forms.NumericUpDown LargeStateUpDown;
		private System.Windows.Forms.NumericUpDown MediumStateUpDown;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox DiskBufferCheckbox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown BufferSizeUpDown;
		private System.Windows.Forms.Label label3;
	}
}