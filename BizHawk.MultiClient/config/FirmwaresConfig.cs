﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//notes: eventually, we intend to have a "firmware acquisition interface" exposed to the emulator cores.
//it will be implemented by the multiclient, and use firmware keys to fetch the firmware content.
//however, for now, the cores are using strings from the config class. so we have the `configMember` which is 
//used by reflection to set the configuration for firmwares which were found

//TODO - we may eventually need to add a progress dialog for this. we should have one for other reasons.
//I started making one in Bizhawk.Util as QuickProgressPopup but ran out of time

//IDEA: show current path in tooltip

namespace BizHawk.MultiClient
{
	public partial class FirmwaresConfig : Form
	{

		private const int idUnsure = 0;
		private const int idMissing = 1;
		private const int idOk = 2;

		class FDR
		{
			public FDR(string hash, string systemId, string recommendedName, string descr, string configMember = null)
			{
				this.hash = hash;
				this.systemId = systemId;
				this.recommendedName = recommendedName;
				this.descr = descr;
				this.configMember = configMember;
			}
			public string hash;
			public string systemId;
			public string recommendedName;
			public string descr;
			public string configMember;

			//sort of sloppy to store this here..
			public FileInfo userPath;
		}

		FDR[] dbItems = new[] {
			new FDR("E4E41472C454F928E53EB10E0509BF7D1146ECC1", "NES", "disksys.rom", "FDS Bios", "FilenameFDSBios"),
			new FDR("973E10840DB683CF3FAF61BD443090786B3A9F04", "SNES", "sgb.sfc", "Super GameBoy Rom"),
			new FDR("A002F4EFBA42775A31185D443F3ED1790B0E949A", "SNES", "cx4.rom", "CX4 Rom"),
			new FDR("188D471FEFEA71EB53F0EE7064697FF0971B1014", "SNES", "dsp1.rom", "DSP1 Rom"),
			new FDR("78B724811F5F18D8C67669D9390397EB1A47A5E2", "SNES", "dsp1b.rom", "DSP1b Rom"),
			new FDR("198C4B1C3BFC6D69E734C5957AF3DBFA26238DFB", "SNES", "dsp2.rom", "DSP2 Rom"),
			new FDR("558DA7CB3BD3876A6CA693661FFC6C110E948CF9", "SNES", "dsp3.rom", "DSP3 Rom"),
			new FDR("AF6478AECB6F1B67177E79C82CA04C56250A8C72", "SNES", "dsp4.rom", "DSP4 Rom"),
			new FDR("6472828403DE3589433A906E2C3F3D274C0FF008", "SNES", "st010.rom", "ST010 Rom"),
			new FDR("FECBAE2CEC76C710422486BAA186FFA7CA1CF925", "SNES", "st011.rom", "ST011 Rom"),
			new FDR("91383B92745CC7CC4F15409AC5BC2C2F699A43F1", "SNES", "st018.rom", "ST018 Rom"),
			new FDR("79F5FF55DD10187C7FD7B8DAAB0B3FFBD1F56A2C", "PCECD", "[BIOS] Super CD-ROM System (Japan) (v3.0).pce", "Super CD-ROM System v3.0 (J)","FilenamePCEBios"),
			new FDR("2B8CB4F87580683EB4D760E4ED210813D667F0A2", "SAT", "Sega Saturn BIOS v1.00 (US).bin", "Sega Saturn Bios v1.00 (NTSC)"),
			new FDR("FAA8EA183A6D7BBE5D4E03BB1332519800D3FBC3", "SAT", "Sega Saturn BIOS (EUR).bin", "Sega Saturn Bios v1.00 (PAL)"),
			new FDR("DF94C5B4D47EB3CC404D88B33A8FDA237EAF4720", "SAT", "Sega Saturn BIOS v1.01 (JAP).bin", "Sega Saturn Bios v1.01 (J)", "FilenameSaturnBios"),
			new FDR("D9D134BB6B36907C615A594CC7688F7BFCEF5B43", "A78", "7800NTSCBIOS.bin", "Atari 7800 NTSC Bios", "FilenameA78NTSCBios"),
			new FDR("5A140136A16D1D83E4FF32A19409CA376A8DF874", "A78", "7800PALBIOS.bin", "Atari 7800 PAL Bios", "FilenameA78PALBios"),
			new FDR("A3AF676991391A6DD716C79022D4947206B78164", "A78", "7800highscore.bin", "Atari 7800 Highscore Bios", "FilenameA78HSCBios"),
			new FDR("45BEDC4CBDEAC66C7DF59E9E599195C778D86A92", "Coleco", "ColecoBios.bin", "Colecovision Bios", "FilenameCOLBios"),
			new FDR("300C20DF6731A33952DED8C436F7F186D25D3492", "GBA", "gbabios.rom", "GBA Bios", "FilenameGBABIOS"),
			new FDR("EF66DAD3E7B2B6A86F326765E7DFD7D1A308AD8F", "TI83", "ti83_1.rom", "TI-83 Rom"),
			new FDR("5A65B922B562CB1F57DAB51B73151283F0E20C7A", "INTV", "erom.bin", "Intellivision Executive Rom", "FilenameINTVEROM"),
			new FDR("F9608BB4AD1CFE3640D02844C7AD8E0BCD974917", "INTV", "grom.bin", "Intellivision Graphics Rom", "FilenameINTVGROM"),
			new FDR("1D503E56DF85A62FEE696E7618DC5B4E781DF1BB", "C64", "c64-kernal.bin", "C64 Kernal Rom"),
			new FDR("79015323128650C742A3694C9429AA91F355905E", "C64", "c64-basic.bin", "C64 Basic Rom"),
			new FDR("ADC7C31E18C7C7413D54802EF2F4193DA14711AA", "C64", "c64-chargen.bin", "C64 Chargen Rom")
		};

		static readonly Dictionary<string, string> systemGroupNames = new Dictionary<string, string>()
			{
				{ "NES", "NES" },
				{ "SNES", "SNES" },
				{ "PCECD", "PCE-CD" },
				{ "SAT", "Saturn" },
				{ "A78", "Atari 7800" },
				{ "Coleco", "Colecovision" },
				{ "GBA", "GBA" },
				{ "TI83", "TI-83" },
				{ "INTV", "Intellivision" },
				{ "C64", "C64" },
			};

		public FirmwaresConfig()
		{
			InitializeComponent();

			imageList1.Images.AddRange(new[] { MultiClient.Properties.Resources.RetroQuestion, MultiClient.Properties.Resources.ExclamationRed, MultiClient.Properties.Resources.GreenCheck });

			
		}

		Font fixedFont;
		private void FirmwaresConfig_Load(object sender, EventArgs e)
		{
			fixedFont = new Font(new FontFamily("Courier New"), 8);

			var groups = new Dictionary<string, ListViewGroup>();
			foreach (var fdr in dbItems)
			{
				var lvi = new ListViewItem();
				lvi.Tag = fdr;
				lvi.UseItemStyleForSubItems = false;
				lvi.ImageIndex = idUnsure;
				lvi.SubItems.Add(fdr.systemId);
				lvi.SubItems.Add("sha1:" + fdr.hash);
				lvi.SubItems.Add(fdr.recommendedName);
				lvi.SubItems.Add(fdr.descr);
				lvi.SubItems[2].Font = fixedFont;
				lvFirmwares.Items.Add(lvi);
				if (!groups.ContainsKey(fdr.systemId))
				{
					lvFirmwares.Groups.Add(fdr.systemId, systemGroupNames[fdr.systemId]);
					var lvg = lvFirmwares.Groups[lvFirmwares.Groups.Count - 1];
					groups[fdr.systemId] = lvg;
				}
				lvi.Group = groups[fdr.systemId];
			}

			lvFirmwares.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);

			DoScan();
		}

		private void FirmwaresConfig_FormClosed(object sender, FormClosedEventArgs e)
		{
			fixedFont.Dispose();
		}

		private void CloseBtn_Click(object sender, EventArgs e)
		{
			Close();
		}


		class RealFirmwareFile
		{
			public FileInfo fi;
			public string hash;
		}

		private void DoScan()
		{
			//string p = Global.Config.FirmwaresPath;
			//CheckFile(Global.Config.FilenameFDSBios, Disksys_ROM_PicBox);
			//CheckFile("cx4.rom", CX4_PicBox);

			//PCE
			//CheckFile(Global.Config.Path

			//build a list of files under the global firmwares path, and build a hash for each of them while we're at it
			var todo = new Queue<DirectoryInfo>(new[] { new DirectoryInfo(Global.Config.FirmwaresPath) });
			var files = new List<RealFirmwareFile>();
			while (todo.Count != 0)
			{
				var di = todo.Dequeue();
				foreach (var disub in di.GetDirectories()) todo.Enqueue(disub);
				foreach (var fi in di.GetFiles())
				{
					var rff = new RealFirmwareFile();
					rff.fi = fi;
					var bytes = File.ReadAllBytes(fi.FullName); //TODO - go faster by reusing buffers
					rff.hash = Util.Hash_SHA1(bytes, 0, bytes.Length);
					files.Add(rff);
				}
			}

			//now, contemplate each file and see if it matches a known firmware
			//this algorithm is slow.
			foreach (var fdr in dbItems)
				fdr.userPath = null;
			foreach (ListViewItem lvi in lvFirmwares.Items)
				lvi.ImageIndex = idUnsure;
			foreach (var f in files)
			{
				foreach (var fdr in dbItems)
				{
					if (fdr.hash == f.hash)
					{
						foreach (ListViewItem lvi in lvFirmwares.Items)
							if (lvi.Tag == fdr)
							{
								lvi.ImageIndex = idOk;
								fdr.userPath = f.fi;
							}
					}
				}
			}

			//set unfound firmwares to missing icon
			foreach (ListViewItem lvi in lvFirmwares.Items)
				if(lvi.ImageIndex == idUnsure)
					lvi.ImageIndex = idMissing;

			//set config entries
			foreach (var fdr in dbItems)
			{
				if(fdr.configMember == null) continue;
				if (fdr.userPath == null) continue;

				//ehhh... this wont be working if this file was outside of the configured firmwares directory.
				//maybe we should check that: http://stackoverflow.com/questions/5617320/given-full-path-check-if-path-is-subdirectory-of-some-other-path-or-otherwise
				string path = Path.GetFileName(fdr.userPath.FullName);

				typeof(Config).GetField(fdr.configMember).SetValue(Global.Config, path);
			}
		}

		private void tbbGroup_Click(object sender, EventArgs e)
		{
			lvFirmwares.ShowGroups = !lvFirmwares.ShowGroups;
		}

		private void lvFirmwares_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			//TBD
			//http://msdn.microsoft.com/en-us/library/ms996467.aspx#sorting_customfeatures
		}

		private void tbbOrganize_Click(object sender, EventArgs e)
		{
			if (System.Windows.Forms.MessageBox.Show(this, "This is going to move/rename files under your configured firmwares directory to match our recommended organizational scheme (which is not super great right now). Proceed?", "Firmwares Organization Confirm", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
				return;

			foreach (var fdr in dbItems)
			{
				if (fdr.userPath == null)
					continue;

				string fpTarget = Path.Combine(Global.Config.FirmwaresPath, fdr.recommendedName);
				string fpSource = fdr.userPath.FullName;
				
				try
				{
					File.Move(fpSource, fpTarget);
				}
				catch
				{
				}
			}

			//to be safe, better do this.
			DoScan();
		}

		private void tbbScan_Click(object sender, EventArgs e)
		{
			DoScan();
		}

	}
}
