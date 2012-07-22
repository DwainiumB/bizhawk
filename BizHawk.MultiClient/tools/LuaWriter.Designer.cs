﻿namespace BizHawk.MultiClient
{
    partial class LuaWriter
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
			this.LuaText = new BizHawk.MultiClient.LuaWriterBox();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// LuaText
			// 
			this.LuaText.AcceptsTab = true;
			this.LuaText.Location = new System.Drawing.Point(12, 12);
			this.LuaText.Name = "LuaText";
			this.LuaText.Size = new System.Drawing.Size(819, 417);
			this.LuaText.TabIndex = 0;
			this.LuaText.Text = "";
			this.LuaText.WordWrap = false;
			this.LuaText.ZoomFactor = 2F;
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// LuaWriter
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(843, 441);
			this.Controls.Add(this.LuaText);
			this.Name = "LuaWriter";
			this.Text = "LuaWriter";
			this.Load += new System.EventHandler(this.LuaWriter_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.LuaWriter_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.LuaWriter_DragEnter);
			this.ResumeLayout(false);

        }

        #endregion

        private LuaWriterBox LuaText;
		private System.Windows.Forms.Timer timer;
    }
}