﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

using BizHawk.Client.Common;
using BizHawk.Emulation.Consoles.Sega;

#pragma warning disable 675 //TOOD: fix the potential problem this is masking

namespace BizHawk.Client.EmuHawk
{
	public partial class GenGameGenie : Form, IToolForm
	{
		bool Processing = false;
		private readonly Dictionary<char, int> GameGenieTable = new Dictionary<char, int>();

		public bool AskSave() { return true; }
		public bool UpdateBefore { get { return false; } }
		public void Restart()
		{
			if (!(Global.Emulator is Genesis))
			{
				Close();
			}
		}
		public void UpdateValues()
		{
			if (!(Global.Emulator is Genesis))
			{
				Close();
			}
		}

		public GenGameGenie()
		{
			InitializeComponent();
			Closing += (o, e) => SaveConfigSettings();

			GameGenieTable.Add('A', 0);   
			GameGenieTable.Add('B', 1);   
			GameGenieTable.Add('C', 2);   
			GameGenieTable.Add('D', 3);   
			GameGenieTable.Add('E', 4);   
			GameGenieTable.Add('F', 5);   
			GameGenieTable.Add('G', 6);   
			GameGenieTable.Add('H', 7);   
			GameGenieTable.Add('J', 8);   
			GameGenieTable.Add('K', 9);   
			GameGenieTable.Add('L', 10);  
			GameGenieTable.Add('M', 11);  
			GameGenieTable.Add('N', 12);  
			GameGenieTable.Add('P', 13);  
			GameGenieTable.Add('R', 14);  
			GameGenieTable.Add('S', 15);  
			GameGenieTable.Add('T', 16);  
			GameGenieTable.Add('V', 17);  
			GameGenieTable.Add('W', 18);  
			GameGenieTable.Add('X', 19);  
			GameGenieTable.Add('Y', 20);  
			GameGenieTable.Add('Z', 21);  
			GameGenieTable.Add('0', 22);  
			GameGenieTable.Add('1', 23);  
			GameGenieTable.Add('2', 24);  
			GameGenieTable.Add('3', 25);  
			GameGenieTable.Add('4', 26);  
			GameGenieTable.Add('5', 27);  
			GameGenieTable.Add('6', 28);  
			GameGenieTable.Add('7', 29);  
			GameGenieTable.Add('8', 30);  
			GameGenieTable.Add('9', 31);  
		}


		// code is code to be converted, val is pointer to value, add is pointer to address
		private void GenGGDecode(string code, ref int val, ref int add)
		{
			long hexcode = 0;
			long decoded = 0;
			int y = 0;

			//convert code to a long binary string
			for (int x = 0; x < code.Length; x++)
			{
				hexcode <<= 5;
				GameGenieTable.TryGetValue(code[x], out y);
				hexcode |= y;
			}

			decoded = ((hexcode & 0xFF00000000) >> 32);
			decoded |= (hexcode & 0x00FF000000);
			decoded |= ((hexcode & 0x0000FF0000) << 16 );
			decoded |= ((hexcode & 0x00000000700) << 5);
			decoded |= ((hexcode & 0x000000F800) >> 3);
			decoded |= ((hexcode & 0x00000000FF) << 16);

			val = (int)(decoded & 0x000000FFFF);
			add = (int)((decoded & 0xFFFFFF0000) >> 16);

		}

		private string GenGGEncode(int val, int add)
		{
			long encoded = 0;
			string code = null;

			encoded = ((long)(val & 0x00FF) << 32);
			encoded |= ((val & 0xE000) >> 5);
			encoded |= ((val & 0x1F00) << 3);
			encoded |= (add & 0xFF0000);
			encoded |= ((add & 0x00FF00) << 16);
			encoded |= (add & 0x0000FF);

			char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			for (int x = 0; x < 8; x++)
			{
				int chr = 0;
				chr = (int)(encoded & 0x1F);
				code += letters[chr];
				encoded >>= 5; 
			}
			//reverse string, as its build backward
			char[] array = code.ToCharArray();
            Array.Reverse(array);
   			return (new string(array));
		}

		private void GGCodeMaskBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			//ignore I O Q U
			if ((e.KeyChar == 73) || (e.KeyChar == 79) || (e.KeyChar == 81) || (e.KeyChar == 85) ||
					(e.KeyChar == 105) || (e.KeyChar == 111) || (e.KeyChar == 113) || (e.KeyChar == 117))
			{
				e.KeyChar = '\n' ;
			}
		}

		private void GGCodeMaskBox_TextChanged(object sender, EventArgs e)
		{
			if (Processing == false)
			{
				Processing = true;
				//remove Invalid I O Q P if pasted
				GGCodeMaskBox.Text = GGCodeMaskBox.Text.Replace("I", string.Empty);
				GGCodeMaskBox.Text = GGCodeMaskBox.Text.Replace("O", string.Empty);
				GGCodeMaskBox.Text = GGCodeMaskBox.Text.Replace("Q", string.Empty);
				GGCodeMaskBox.Text = GGCodeMaskBox.Text.Replace("U", string.Empty);
				

				if (GGCodeMaskBox.Text.Length > 0)
				{
					int val = 0;
					int add = 0;
					GenGGDecode(GGCodeMaskBox.Text, ref val, ref add);
					AddressBox.Text = String.Format("{0:X6}", add);
					ValueBox.Text = String.Format("{0:X4}", val);
					addcheatbt.Enabled = true;
				}
				else
				{
					AddressBox.Text = "";
					ValueBox.Text = "";
					addcheatbt.Enabled = false;
				}
				Processing = false;
			}
		}

		private void ClearBT_Click(object sender, EventArgs e)
		{
			AddressBox.Text = "";
			ValueBox.Text = "";
			GGCodeMaskBox.Text = "";
			addcheatbt.Enabled = false;
		}

		private void AddressBox_TextChanged(object sender, EventArgs e)
		{
			//remove invalid character when pasted
			if (Processing == false)
			{
				Processing = true;
				if (Regex.IsMatch(AddressBox.Text, @"[^a-fA-F0-9]"))
				{
					string temp = Regex.Replace(AddressBox.Text, @"[^a-fA-F0-9]", string.Empty);
					AddressBox.Text = temp;
				}
				if ((AddressBox.Text.Length > 0) || (ValueBox.Text.Length > 0))
				{
					int val = 0;
					int add = 0;
					if (ValueBox.Text.Length > 0)
						val = int.Parse(ValueBox.Text, NumberStyles.HexNumber);
					if (AddressBox.Text.Length > 0)
						add = int.Parse(AddressBox.Text, NumberStyles.HexNumber);
					GGCodeMaskBox.Text = GenGGEncode(val, add);
					addcheatbt.Enabled = true;
				}
				else
				{
					GGCodeMaskBox.Text = "";
					addcheatbt.Enabled = false;
				}
				Processing = false;
			}
		}

		private void ValueBox_TextChanged(object sender, EventArgs e)
		{
			if (Processing == false)
			{
				Processing = true;
				//remove invalid character when pasted
				if (Regex.IsMatch(ValueBox.Text, @"[^a-fA-F0-9]"))
				{
					string temp = Regex.Replace(ValueBox.Text, @"[^a-fA-F0-9]", string.Empty);
					ValueBox.Text = temp;
				}
				if ((AddressBox.Text.Length > 0) || (ValueBox.Text.Length > 0))
				{
					int val = 0;
					int add = 0;
					if (ValueBox.Text.Length > 0)
						val = int.Parse(ValueBox.Text, NumberStyles.HexNumber);
					if (AddressBox.Text.Length > 0)
						add = int.Parse(AddressBox.Text, NumberStyles.HexNumber);
					GGCodeMaskBox.Text = GenGGEncode(val, add);
					addcheatbt.Enabled = true;

				}
				else
				{
					GGCodeMaskBox.Text = "";
					addcheatbt.Enabled = false;
				}
				Processing = false;
			}
		}

		private void addcheatbt_Click(object sender, EventArgs e)
		{
			if (Global.Emulator is Genesis)
			{
				string NAME;
				int ADDRESS = 0;
				int VALUE = 0;
				int romDataDomainIndex = 0;


				if (!String.IsNullOrWhiteSpace(cheatname.Text))
				{
					NAME = cheatname.Text;
				}
				else
				{
					Processing = true;
					GGCodeMaskBox.TextMaskFormat = MaskFormat.IncludeLiterals;
					NAME = GGCodeMaskBox.Text;
					GGCodeMaskBox.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
					Processing = false;
				}

				if (!String.IsNullOrWhiteSpace(AddressBox.Text))
				{
					ADDRESS = int.Parse(AddressBox.Text, NumberStyles.HexNumber);
				}

				if (!String.IsNullOrWhiteSpace(ValueBox.Text))
				{
					VALUE = ValueBox.ToRawInt();
				}

				for (int i = 0; i < Global.Emulator.MemoryDomains.Count; i++)
				{
					if (Global.Emulator.MemoryDomains[i].ToString() == "Rom Data")
					{
						romDataDomainIndex = i;
					}
				}

				Watch watch = Watch.GenerateWatch(
					Global.Emulator.MemoryDomains[romDataDomainIndex],
					ADDRESS,
					Watch.WatchSize.Word,
					Watch.DisplayType.Hex,
					NAME,
					bigEndian: true
				);

				Global.CheatList.Add(new Cheat(
					watch,
					VALUE,
					compare: null,
					enabled: true
				));

				ToolHelpers.UpdateCheatRelatedTools();
			}

		}

		private void autoloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.GENGGAutoload ^= true;
		}

		private void saveWindowPositionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.GENGGSaveWindowPosition ^= true;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void GENGameGenie_Load(object sender, EventArgs e)
		{

			if (Global.Config.GENGGSaveWindowPosition && Global.Config.GENGGWndx >= 0 && Global.Config.GENGGWndy >= 0)
			{
				Location = new Point(Global.Config.GENGGWndx, Global.Config.GENGGWndy);
			}
		}

		private void SaveConfigSettings()
		{
			Global.Config.GENGGWndx = Location.X;
			Global.Config.GENGGWndy = Location.Y;
		}

	}
}

