﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using BizHawk;

namespace BizHawk.MultiClient
{
    public partial class NESGameGenie : Form
    {
        //TODO
        //Autoload
        //Save Window Position
        int address = -1;
        int value = -1;
        int compare = -1;
        Dictionary<char, int> GameGenieTable = new Dictionary<char, int>();

        public NESGameGenie()
        {
            InitializeComponent();
        }

        private void NESGameGenie_Load(object sender, EventArgs e)
        {
            GameGenieTable.Add('A', 0);     //0000
            GameGenieTable.Add('P', 1);     //0001
            GameGenieTable.Add('Z', 2);     //0010
            GameGenieTable.Add('L', 3);     //0011
            GameGenieTable.Add('G', 4);     //0100
            GameGenieTable.Add('I', 5);     //0101
            GameGenieTable.Add('T', 6);     //0110
            GameGenieTable.Add('Y', 7);     //0111
            GameGenieTable.Add('E', 8);     //1000
            GameGenieTable.Add('O', 9);     //1001
            GameGenieTable.Add('X', 10);    //1010
            GameGenieTable.Add('U', 11);    //1011
            GameGenieTable.Add('K', 12);    //1100
            GameGenieTable.Add('S', 13);    //1101
            GameGenieTable.Add('V', 14);    //1110
            GameGenieTable.Add('N', 15);    //1111

            AddCheat.Enabled = false;
        }

        private void GameGenieCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Make uppercase
            if (e.KeyChar > 97 && e.KeyChar < 123)
                e.KeyChar -= (char)32;

            if (!(GameGenieTable.ContainsKey(e.KeyChar)))
            {
                if (!(e.KeyChar == (char)Keys.Back)) //Allow backspace
                    e.Handled = true;
            }
            else
            {
                Encoding.Checked = false;
            }
        }

        private int GetBit(int value, int bit)
        {
            return (value >> bit) & 1;
        }

        private void DecodeGameGenieCode(string code)
        {
            //char 3 bit 3 denotes the code length.
            if (code.Length == 6)
            {
                //Char # |   1   |   2   |   3   |   4   |   5   |   6   |
                //Bit  # |3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|
                //maps to|1|6|7|8|H|2|3|4|-|I|J|K|L|A|B|C|D|M|N|O|5|E|F|G|
                value = 0;
                address = 0x8000;
                int x;

                GameGenieTable.TryGetValue(code[0], out x);
                value |= (x & 0x07);
                value |= (x & 0x08) << 4;

                GameGenieTable.TryGetValue(code[1], out x);
                value |= (x & 0x07) << 4;
                address |= (x & 0x08) << 4; 
                
                GameGenieTable.TryGetValue(code[2], out x);
                address |= (x & 0x07) << 4;

                GameGenieTable.TryGetValue(code[3], out x);
                address |= (x & 0x07) << 12;
                address |= (x & 0x08);

                GameGenieTable.TryGetValue(code[4], out x);
                address |= (x & 0x07);
                address |= (x & 0x08) << 8;
                
                GameGenieTable.TryGetValue(code[5], out x);
                address |= (x & 0x07) << 8;
                value |= (x & 0x08);

                SetProperties();

            }
            else if (code.Length == 8)
            {
                //Char # |   1   |   2   |   3   |   4   |   5   |   6   |   7   |   8   |
                //Bit  # |3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|3|2|1|0|
                //maps to|1|6|7|8|H|2|3|4|-|I|J|K|L|A|B|C|D|M|N|O|%|E|F|G|!|^|&|*|5|@|#|$|
                value = 0;
                address = 0x8000;
                compare = 0;
                int x;

                GameGenieTable.TryGetValue(code[0], out x);
                value |= (x & 0x07);
                value |= (x & 0x08) << 4;

                GameGenieTable.TryGetValue(code[1], out x);
                value |= (x & 0x07) << 4;
                address |= (x & 0x08) << 4;

                GameGenieTable.TryGetValue(code[2], out x);
                address |= (x & 0x07) << 4;

                GameGenieTable.TryGetValue(code[3], out x);
                address |= (x & 0x07) << 12;
                address |= (x & 0x08);

                GameGenieTable.TryGetValue(code[4], out x);
                address |= (x & 0x07);
                address |= (x & 0x08) << 8;

                GameGenieTable.TryGetValue(code[5], out x);
                address |= (x & 0x07) << 8;
                compare |= (x & 0x08);

                GameGenieTable.TryGetValue(code[6], out x);
                compare |= (x & 0x07);
                compare |= (x & 0x08) << 4;

                GameGenieTable.TryGetValue(code[7], out x);
                compare |= (x & 0x07) << 4;
                value |= (x & 0x08);
                SetProperties();
            }
        }

        private void SetProperties()
        {
            if (address >= 0)
                AddressBox.Text = String.Format("{0:X4}", address);
            else
                AddressBox.Text = "";

            if (compare >= 0)
                CompareBox.Text = String.Format("{0:X2}", compare);
            else
                CompareBox.Text = "";

            if (value >= 0)
                ValueBox.Text = String.Format("{0:X2}", value);

        }

        private void ClearProperties()
        {
            address = -1;
            value = -1;
            compare = -1;
            AddressBox.Text = "";
            CompareBox.Text = "";
            ValueBox.Text = "";
            AddCheat.Enabled = false;
        }

        private void GameGenieCode_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.Checked == false)
            {
                if (GameGenieCode.Text.Length == 6 || GameGenieCode.Text.Length == 8)
                    DecodeGameGenieCode(GameGenieCode.Text);
                else
                    ClearProperties();
            }
            TryEnableAddCheat();
        }

        private void Keypad_Click(object sender, EventArgs e)
        {
            if (GameGenieCode.Text.Length < 8)
            {
                if (sender == A) GameGenieCode.Text += "A";
                if (sender == P) GameGenieCode.Text += "P";
                if (sender == Z) GameGenieCode.Text += "Z";
                if (sender == L) GameGenieCode.Text += "L";
                if (sender == G) GameGenieCode.Text += "G";
                if (sender == I) GameGenieCode.Text += "I";
                if (sender == T) GameGenieCode.Text += "T";
                if (sender == Y) GameGenieCode.Text += "Y";
                if (sender == E) GameGenieCode.Text += "E";
                if (sender == O) GameGenieCode.Text += "O";
                if (sender == X) GameGenieCode.Text += "X";
                if (sender == U) GameGenieCode.Text += "U";
                if (sender == K) GameGenieCode.Text += "K";
                if (sender == S) GameGenieCode.Text += "S";
                if (sender == V) GameGenieCode.Text += "V";
                if (sender == N) GameGenieCode.Text += "N";
                Encoding.Checked = false;
            }
        }

        private void AddressBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == (char)Keys.Back)) //Allow backspace
            {
                if (InputValidate.IsValidHexNumber(e.KeyChar))
                {
                    Encoding.Checked = true;
                }
                else
                    e.Handled = true;
            }
            else
                Encoding.Checked = true;
        }

        private void CompareBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == (char)Keys.Back)) //Allow backspace
            {
                if (InputValidate.IsValidHexNumber(e.KeyChar))
                {

                    Encoding.Checked = true;
                }
                else
                    e.Handled = true;
            }
            else
                Encoding.Checked = true;
        }

        private void ValueBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == (char)Keys.Back)) //Allow backspace
            {
                if (InputValidate.IsValidHexNumber(e.KeyChar))
                {

                    Encoding.Checked = true;
                }
                else
                    e.Handled = true;
            }
            else
                Encoding.Checked = true;
        }

        private void AddressBox_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.Checked && AddressBox.Text.Length > 0)
            {
                int a = int.Parse(AddressBox.Text, NumberStyles.HexNumber); //TODO: try/catch just in case?
                if (ValueBox.Text.Length > 0)
                {
                    address = a;
                    EncodeGameGenie();
                }
            }
            TryEnableAddCheat();
        }

        private void CompareBox_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.Checked)
            {
                if (CompareBox.Text.Length > 0)
                {
                    int c = int.Parse(CompareBox.Text, NumberStyles.HexNumber);
                    if (c > 0 && c < 0x100)
                    {
                        if (ValueBox.Text.Length > 0 && AddressBox.Text.Length > 0)
                        {
                            compare = c;
                            EncodeGameGenie();
                        }
                    }
                }
                else
                {
                    compare = -1;
                    EncodeGameGenie();
                }
            }
            TryEnableAddCheat();
        }

        private void TryEnableAddCheat()
        {
            if (AddressBox.Text.Length > 0 && ValueBox.Text.Length > 0 && GameGenieCode.Text.Length > 0)
                AddCheat.Enabled = true;
            else
                AddCheat.Enabled = false;
        }

        private void ValueBox_TextChanged(object sender, EventArgs e)
        {
            if (Encoding.Checked && ValueBox.Text.Length > 0)
            {
                int v = int.Parse(ValueBox.Text, NumberStyles.HexNumber);
                if (v > 0 && v < 0x100)
                {
                    if (AddressBox.Text.Length > 0)
                    {
                        value = v;
                        EncodeGameGenie();
                    }
                }
            }

            TryEnableAddCheat();
        }

        private void EncodeGameGenie()
        {
            char[] letters = {'A','P','Z','L','G','I','T','Y','E','O','X','U','K','S','V','N'};
            if (address >= 0x8000)
                address -= 0x8000;
            GameGenieCode.Text = "";
            byte[] num = {0, 0, 0, 0, 0, 0, 0, 0};
            num[0] = (byte)((value & 7) + ((value >> 4) & 8));
            num[1] = (byte)(((value >> 4) & 7) + ((address >> 4) & 8));
            num[2] = (byte)(((address >> 4) & 7));
            num[3] = (byte)((address >> 12) + (address & 8));
            num[4] = (byte)((address & 7) + ((address >> 8) & 8));
            num[5] = (byte)(((address >> 8) & 7));

            if (compare < 0 || CompareBox.Text.Length == 0)
            {
                num[5] += (byte)(value & 8);
                for (int x = 0; x < 6; x++)
                    GameGenieCode.Text += letters[num[x]];
            }
            else
            {
                num[2] += 8;
                num[5] += (byte)(compare & 8);
                num[6] = (byte)((compare & 7) + ((compare >> 4) & 8));
                num[7] = (byte)(((compare >> 4) & 7) + (value & 8));
                for (int x = 0; x < 8; x++)
                    GameGenieCode.Text += letters[num[x]];
            }
            
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearProperties();
            GameGenieCode.Text = "";
            Encoding.Checked = false;
        }

        private void AddCheat_Click(object sender, EventArgs e)
        {
            Cheat c = new Cheat();
            c.name = GameGenieCode.Text;
            c.address = int.Parse(AddressBox.Text, NumberStyles.HexNumber);
            c.value = byte.Parse(ValueBox.Text, NumberStyles.HexNumber);
            c.Enable();
            Global.MainForm.Cheats1.AddCheat(c);
        }
    }
}
