﻿namespace BizHawk.MultiClient.tools
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
            this.LuaText = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // LuaText
            // 
            this.LuaText.DetectUrls = false;
            this.LuaText.Location = new System.Drawing.Point(12, 12);
            this.LuaText.Name = "LuaText";
            this.LuaText.Size = new System.Drawing.Size(819, 417);
            this.LuaText.TabIndex = 0;
            this.LuaText.Text = "";
            this.LuaText.ZoomFactor = 1.2F;
            this.LuaText.TextChanged += new System.EventHandler(this.LuaText_TextChanged);
            // 
            // LuaWriter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 441);
            this.Controls.Add(this.LuaText);
            this.Name = "LuaWriter";
            this.Text = "LuaWriter";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox LuaText;
    }
}