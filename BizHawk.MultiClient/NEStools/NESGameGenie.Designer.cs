﻿namespace BizHawk.MultiClient
{
    partial class NESGameGenie
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
            this.GameGenieCode = new System.Windows.Forms.TextBox();
            this.GameGenieCodeBox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CompareBox = new System.Windows.Forms.TextBox();
            this.ValueBox = new System.Windows.Forms.TextBox();
            this.AddressBox = new System.Windows.Forms.TextBox();
            this.AddCheat = new System.Windows.Forms.Button();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.N = new System.Windows.Forms.Button();
            this.V = new System.Windows.Forms.Button();
            this.S = new System.Windows.Forms.Button();
            this.K = new System.Windows.Forms.Button();
            this.U = new System.Windows.Forms.Button();
            this.X = new System.Windows.Forms.Button();
            this.O = new System.Windows.Forms.Button();
            this.E = new System.Windows.Forms.Button();
            this.Y = new System.Windows.Forms.Button();
            this.T = new System.Windows.Forms.Button();
            this.I = new System.Windows.Forms.Button();
            this.G = new System.Windows.Forms.Button();
            this.L = new System.Windows.Forms.Button();
            this.Z = new System.Windows.Forms.Button();
            this.P = new System.Windows.Forms.Button();
            this.A = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Encoding = new System.Windows.Forms.CheckBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.GameGenieCodeBox.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GameGenieCode
            // 
            this.GameGenieCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.GameGenieCode.Location = new System.Drawing.Point(6, 19);
            this.GameGenieCode.MaxLength = 8;
            this.GameGenieCode.Name = "GameGenieCode";
            this.GameGenieCode.Size = new System.Drawing.Size(86, 20);
            this.GameGenieCode.TabIndex = 0;
            this.GameGenieCode.TextChanged += new System.EventHandler(this.GameGenieCode_TextChanged);
            this.GameGenieCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GameGenieCode_KeyPress);
            // 
            // GameGenieCodeBox
            // 
            this.GameGenieCodeBox.Controls.Add(this.GameGenieCode);
            this.GameGenieCodeBox.Location = new System.Drawing.Point(31, 85);
            this.GameGenieCodeBox.Name = "GameGenieCodeBox";
            this.GameGenieCodeBox.Size = new System.Drawing.Size(115, 54);
            this.GameGenieCodeBox.TabIndex = 1;
            this.GameGenieCodeBox.TabStop = false;
            this.GameGenieCodeBox.Text = "Game Genie Code";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Compare";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Address";
            // 
            // CompareBox
            // 
            this.CompareBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.CompareBox.Location = new System.Drawing.Point(87, 39);
            this.CompareBox.MaxLength = 2;
            this.CompareBox.Name = "CompareBox";
            this.CompareBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.CompareBox.Size = new System.Drawing.Size(27, 20);
            this.CompareBox.TabIndex = 2;
            this.CompareBox.TextChanged += new System.EventHandler(this.CompareBox_TextChanged);
            this.CompareBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CompareBox_KeyPress);
            // 
            // ValueBox
            // 
            this.ValueBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.ValueBox.Location = new System.Drawing.Point(87, 65);
            this.ValueBox.MaxLength = 2;
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ValueBox.Size = new System.Drawing.Size(27, 20);
            this.ValueBox.TabIndex = 1;
            this.ValueBox.TextChanged += new System.EventHandler(this.ValueBox_TextChanged);
            this.ValueBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ValueBox_KeyPress);
            // 
            // AddressBox
            // 
            this.AddressBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.AddressBox.Location = new System.Drawing.Point(75, 13);
            this.AddressBox.MaxLength = 4;
            this.AddressBox.Name = "AddressBox";
            this.AddressBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.AddressBox.Size = new System.Drawing.Size(39, 20);
            this.AddressBox.TabIndex = 0;
            this.AddressBox.TextChanged += new System.EventHandler(this.AddressBox_TextChanged);
            this.AddressBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AddressBox_KeyPress);
            // 
            // AddCheat
            // 
            this.AddCheat.Location = new System.Drawing.Point(202, 217);
            this.AddCheat.Name = "AddCheat";
            this.AddCheat.Size = new System.Drawing.Size(69, 21);
            this.AddCheat.TabIndex = 3;
            this.AddCheat.Text = "Add Cheat";
            this.AddCheat.UseVisualStyleBackColor = true;
            this.AddCheat.Click += new System.EventHandler(this.AddCheat_Click);
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ButtonPanel.Controls.Add(this.N);
            this.ButtonPanel.Controls.Add(this.V);
            this.ButtonPanel.Controls.Add(this.S);
            this.ButtonPanel.Controls.Add(this.K);
            this.ButtonPanel.Controls.Add(this.U);
            this.ButtonPanel.Controls.Add(this.X);
            this.ButtonPanel.Controls.Add(this.O);
            this.ButtonPanel.Controls.Add(this.E);
            this.ButtonPanel.Controls.Add(this.Y);
            this.ButtonPanel.Controls.Add(this.T);
            this.ButtonPanel.Controls.Add(this.I);
            this.ButtonPanel.Controls.Add(this.G);
            this.ButtonPanel.Controls.Add(this.L);
            this.ButtonPanel.Controls.Add(this.Z);
            this.ButtonPanel.Controls.Add(this.P);
            this.ButtonPanel.Controls.Add(this.A);
            this.ButtonPanel.Location = new System.Drawing.Point(31, 12);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(240, 67);
            this.ButtonPanel.TabIndex = 4;
            // 
            // N
            // 
            this.N.Location = new System.Drawing.Point(206, 35);
            this.N.Name = "N";
            this.N.Size = new System.Drawing.Size(26, 23);
            this.N.TabIndex = 15;
            this.N.Text = "N";
            this.N.UseVisualStyleBackColor = true;
            this.N.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // V
            // 
            this.V.Location = new System.Drawing.Point(178, 35);
            this.V.Name = "V";
            this.V.Size = new System.Drawing.Size(26, 23);
            this.V.TabIndex = 14;
            this.V.Text = "V";
            this.V.UseVisualStyleBackColor = true;
            this.V.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // S
            // 
            this.S.Location = new System.Drawing.Point(149, 35);
            this.S.Name = "S";
            this.S.Size = new System.Drawing.Size(26, 23);
            this.S.TabIndex = 13;
            this.S.Text = "S";
            this.S.UseVisualStyleBackColor = true;
            this.S.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // K
            // 
            this.K.Location = new System.Drawing.Point(121, 35);
            this.K.Name = "K";
            this.K.Size = new System.Drawing.Size(26, 23);
            this.K.TabIndex = 12;
            this.K.Text = "K";
            this.K.UseVisualStyleBackColor = true;
            this.K.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // U
            // 
            this.U.Location = new System.Drawing.Point(91, 35);
            this.U.Name = "U";
            this.U.Size = new System.Drawing.Size(26, 23);
            this.U.TabIndex = 11;
            this.U.Text = "U";
            this.U.UseVisualStyleBackColor = true;
            this.U.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // X
            // 
            this.X.Location = new System.Drawing.Point(63, 35);
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(26, 23);
            this.X.TabIndex = 10;
            this.X.Text = "X";
            this.X.UseVisualStyleBackColor = true;
            this.X.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // O
            // 
            this.O.Location = new System.Drawing.Point(34, 35);
            this.O.Name = "O";
            this.O.Size = new System.Drawing.Size(26, 23);
            this.O.TabIndex = 9;
            this.O.Text = "O";
            this.O.UseVisualStyleBackColor = true;
            this.O.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // E
            // 
            this.E.Location = new System.Drawing.Point(6, 35);
            this.E.Name = "E";
            this.E.Size = new System.Drawing.Size(26, 23);
            this.E.TabIndex = 8;
            this.E.Text = "E";
            this.E.UseVisualStyleBackColor = true;
            this.E.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // Y
            // 
            this.Y.Location = new System.Drawing.Point(206, 5);
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(26, 23);
            this.Y.TabIndex = 7;
            this.Y.Text = "Y";
            this.Y.UseVisualStyleBackColor = true;
            this.Y.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // T
            // 
            this.T.Location = new System.Drawing.Point(178, 5);
            this.T.Name = "T";
            this.T.Size = new System.Drawing.Size(26, 23);
            this.T.TabIndex = 6;
            this.T.Text = "T";
            this.T.UseVisualStyleBackColor = true;
            this.T.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // I
            // 
            this.I.Location = new System.Drawing.Point(149, 5);
            this.I.Name = "I";
            this.I.Size = new System.Drawing.Size(26, 23);
            this.I.TabIndex = 5;
            this.I.Text = "I";
            this.I.UseVisualStyleBackColor = true;
            this.I.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // G
            // 
            this.G.Location = new System.Drawing.Point(121, 5);
            this.G.Name = "G";
            this.G.Size = new System.Drawing.Size(26, 23);
            this.G.TabIndex = 4;
            this.G.Text = "G";
            this.G.UseVisualStyleBackColor = true;
            this.G.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // L
            // 
            this.L.Location = new System.Drawing.Point(91, 5);
            this.L.Name = "L";
            this.L.Size = new System.Drawing.Size(26, 23);
            this.L.TabIndex = 3;
            this.L.Text = "L";
            this.L.UseVisualStyleBackColor = true;
            this.L.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // Z
            // 
            this.Z.Location = new System.Drawing.Point(63, 5);
            this.Z.Name = "Z";
            this.Z.Size = new System.Drawing.Size(26, 23);
            this.Z.TabIndex = 2;
            this.Z.Text = "Z";
            this.Z.UseVisualStyleBackColor = true;
            this.Z.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // P
            // 
            this.P.Location = new System.Drawing.Point(34, 5);
            this.P.Name = "P";
            this.P.Size = new System.Drawing.Size(26, 23);
            this.P.TabIndex = 1;
            this.P.Text = "P";
            this.P.UseVisualStyleBackColor = true;
            this.P.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // A
            // 
            this.A.Location = new System.Drawing.Point(6, 5);
            this.A.Name = "A";
            this.A.Size = new System.Drawing.Size(26, 23);
            this.A.TabIndex = 0;
            this.A.Text = "A";
            this.A.UseVisualStyleBackColor = true;
            this.A.Click += new System.EventHandler(this.Keypad_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.AddressBox);
            this.groupBox1.Controls.Add(this.ValueBox);
            this.groupBox1.Controls.Add(this.CompareBox);
            this.groupBox1.Location = new System.Drawing.Point(31, 145);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(126, 93);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(69, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "0x";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(69, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "0x";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(18, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "0x";
            // 
            // Encoding
            // 
            this.Encoding.Appearance = System.Windows.Forms.Appearance.Button;
            this.Encoding.AutoSize = true;
            this.Encoding.Location = new System.Drawing.Point(217, 101);
            this.Encoding.Name = "Encoding";
            this.Encoding.Size = new System.Drawing.Size(54, 23);
            this.Encoding.TabIndex = 6;
            this.Encoding.Text = "Encode";
            this.Encoding.UseVisualStyleBackColor = true;
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(217, 130);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(54, 23);
            this.ClearButton.TabIndex = 7;
            this.ClearButton.Text = "&Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // NESGameGenie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 266);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.Encoding);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.AddCheat);
            this.Controls.Add(this.GameGenieCodeBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "NESGameGenie";
            this.ShowIcon = false;
            this.Text = "Game Genie Encoder / Decoder";
            this.Load += new System.EventHandler(this.NESGameGenie_Load);
            this.GameGenieCodeBox.ResumeLayout(false);
            this.GameGenieCodeBox.PerformLayout();
            this.ButtonPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox GameGenieCode;
        private System.Windows.Forms.GroupBox GameGenieCodeBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CompareBox;
        private System.Windows.Forms.TextBox ValueBox;
        private System.Windows.Forms.TextBox AddressBox;
        private System.Windows.Forms.Button AddCheat;
        private System.Windows.Forms.Panel ButtonPanel;
        private System.Windows.Forms.Button L;
        private System.Windows.Forms.Button Z;
        private System.Windows.Forms.Button P;
        private System.Windows.Forms.Button A;
        private System.Windows.Forms.Button Y;
        private System.Windows.Forms.Button T;
        private System.Windows.Forms.Button I;
        private System.Windows.Forms.Button G;
        private System.Windows.Forms.Button N;
        private System.Windows.Forms.Button V;
        private System.Windows.Forms.Button S;
        private System.Windows.Forms.Button K;
        private System.Windows.Forms.Button U;
        private System.Windows.Forms.Button X;
        private System.Windows.Forms.Button O;
        private System.Windows.Forms.Button E;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox Encoding;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ClearButton;
    }
}