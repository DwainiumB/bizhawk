﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace BizHawk.Emulation.Common
{
	public static class FirmwareDatabase
	{
		static FirmwareDatabase()
		{
			//FDS has two OK variants  (http://tcrf.net/Family_Computer_Disk_System)
			var fds_nintendo = File("57FE1BDEE955BB48D357E463CCBF129496930B62", "disksys-nintendo.rom", "Bios (Nintendo)");
			var fds_twinfc = File("E4E41472C454F928E53EB10E0509BF7D1146ECC1", "disksys-nintendo.rom", "Bios (TwinFC)");
			Firmware("NES", "Bios_FDS", "Bios");
			Option("NES", "Bios_FDS", fds_nintendo);
			Option("NES", "Bios_FDS", fds_twinfc);

			FirmwareAndOption("973E10840DB683CF3FAF61BD443090786B3A9F04", "SNES", "Rom_SGB", "sgb.sfc", "Super GameBoy Rom"); //World (Rev B) ?
			FirmwareAndOption("A002F4EFBA42775A31185D443F3ED1790B0E949A", "SNES", "CX4", "cx4.rom", "CX4 Rom");
			FirmwareAndOption("188D471FEFEA71EB53F0EE7064697FF0971B1014", "SNES", "DSP1", "dsp1.rom", "DSP1 Rom");
			FirmwareAndOption("78B724811F5F18D8C67669D9390397EB1A47A5E2", "SNES", "DSP1b", "dsp1b.rom", "DSP1b Rom");
			FirmwareAndOption("198C4B1C3BFC6D69E734C5957AF3DBFA26238DFB", "SNES", "DSP2", "dsp2.rom", "DSP2 Rom");
			FirmwareAndOption("558DA7CB3BD3876A6CA693661FFC6C110E948CF9", "SNES", "DSP3", "dsp3.rom", "DSP3 Rom");
			FirmwareAndOption("AF6478AECB6F1B67177E79C82CA04C56250A8C72", "SNES", "DSP4", "dsp4.rom", "DSP4 Rom");
			FirmwareAndOption("6472828403DE3589433A906E2C3F3D274C0FF008", "SNES", "ST010", "st010.rom", "ST010 Rom");
			FirmwareAndOption("FECBAE2CEC76C710422486BAA186FFA7CA1CF925", "SNES", "ST011", "st011.rom", "ST011 Rom");
			FirmwareAndOption("91383B92745CC7CC4F15409AC5BC2C2F699A43F1", "SNES", "ST018", "st018.rom", "ST018 Rom");
			FirmwareAndOption("79F5FF55DD10187C7FD7B8DAAB0B3FFBD1F56A2C", "PCECD", "Bios", "pcecd-3.0-(J).pce", "Super CD Bios (J)");
			FirmwareAndOption("D9D134BB6B36907C615A594CC7688F7BFCEF5B43", "A78", "Bios_NTSC", "7800NTSCBIOS.bin", "NTSC Bios");
			FirmwareAndOption("5A140136A16D1D83E4FF32A19409CA376A8DF874", "A78", "Bios_PAL", "7800PALBIOS.bin", "PAL Bios");
			FirmwareAndOption("A3AF676991391A6DD716C79022D4947206B78164", "A78", "Bios_HSC", "7800highscore.bin", "Highscore Bios");
			FirmwareAndOption("45BEDC4CBDEAC66C7DF59E9E599195C778D86A92", "Coleco", "Bios", "ColecoBios.bin", "Bios");
			FirmwareAndOption("300C20DF6731A33952DED8C436F7F186D25D3492", "GBA", "Bios", "gbabios.rom", "Bios");
			//FirmwareAndOption("24F67BDEA115A2C847C8813A262502EE1607B7DF", "NDS", "Bios_Arm7", "biosnds7.rom", "ARM7 Bios");
			//FirmwareAndOption("BFAAC75F101C135E32E2AAF541DE6B1BE4C8C62D", "NDS", "Bios_Arm9", "biosnds9.rom", "ARM9 Bios");
			FirmwareAndOption("5A65B922B562CB1F57DAB51B73151283F0E20C7A", "INTV", "EROM", "erom.bin", "Executive Rom");
			FirmwareAndOption("F9608BB4AD1CFE3640D02844C7AD8E0BCD974917", "INTV", "GROM", "grom.bin", "Graphics Rom");
			FirmwareAndOption("1D503E56DF85A62FEE696E7618DC5B4E781DF1BB", "C64", "Kernal", "c64-kernal.bin", "Kernal Rom");
			FirmwareAndOption("79015323128650C742A3694C9429AA91F355905E", "C64", "Basic", "c64-basic.bin", "Basic Rom");
			FirmwareAndOption("ADC7C31E18C7C7413D54802EF2F4193DA14711AA", "C64", "Chargen", "c64-chargen.bin", "Chargen Rom");

			//for saturn, we think any bios region can pretty much run any iso
			//so, we're going to lay this out carefully so that we choose things in a sensible order, but prefer the correct region
			var ss_100_j = File("2B8CB4F87580683EB4D760E4ED210813D667F0A2", "saturn-1.00-(J).bin", "Bios v1.00 (J)");
			var ss_100_ue = File("FAA8EA183A6D7BBE5D4E03BB1332519800D3FBC3", "saturn-1.00-(U+E).bin", "Bios v1.00 (U+E)");
			var ss_100a_ue = File("3BB41FEB82838AB9A35601AC666DE5AACFD17A58", "saturn-1.00a-(U+E).bin", "Bios v1.00a (U+E)");
			var ss_101_j = File("DF94C5B4D47EB3CC404D88B33A8FDA237EAF4720", "saturn-1.01-(J).bin", "Bios v1.01 (J)");
			Firmware("SAT", "J", "Bios (J)");
			Option("SAT", "J", ss_100_j);
			Option("SAT", "J", ss_101_j);
			Option("SAT", "J", ss_100_ue);
			Option("SAT", "J", ss_100a_ue);
			Firmware("SAT", "U", "Bios (U)");
			Option("SAT", "U", ss_100_ue);
			Option("SAT", "U", ss_100a_ue);
			Option("SAT", "U", ss_100_j);
			Option("SAT", "U", ss_101_j);
			Firmware("SAT", "E", "Bios (E)");
			Option("SAT", "E", ss_100_ue);
			Option("SAT", "E", ss_100a_ue);
			Option("SAT", "E", ss_100_j);
			Option("SAT", "E", ss_101_j);

			var ti83_102 = File("CE08F6A808701FC6672230A790167EE485157561", "ti83_102.rom", "TI-83 Rom v1.02");
			var ti83_103 = File("8399E384804D8D29866CAA4C8763D7A61946A467", "ti83_103.rom", "TI-83 Rom v1.03");
			var ti83_104 = File("33877FF637DC5F4C5388799FD7E2159B48E72893", "ti83_104.rom", "TI-83 Rom v1.04");
			var ti83_106 = File("3D65C2A1B771CE8E5E5A0476EC1AA9C9CDC0E833", "ti83_106.rom", "TI-83 Rom v1.06");
			var ti83_107 = File("EF66DAD3E7B2B6A86F326765E7DFD7D1A308AD8F", "ti83_107.rom", "TI-83 Rom v1.07"); //formerly the 1.?? recommended one
			var ti83_108 = File("9C74F0B61655E9E160E92164DB472AD7EE02B0F8", "ti83_108.rom", "TI-83 Rom v1.08");
			var ti83p_103 = File("37EAEEB9FB5C18FB494E322B75070E80CC4D858E", "ti83p_103b.rom", "TI-83 Plus Rom v1.03");
			var ti83p_112 = File("6615DF5554076B6B81BD128BF847D2FF046E556B", "ti83p_112.rom", "TI-83 Plus Rom v1.12");

			Firmware("TI83", "Rom", "TI-83 Rom");
			Option("TI83", "Rom", ti83_102);
			Option("TI83", "Rom", ti83_103);
			Option("TI83", "Rom", ti83_104);
			Option("TI83", "Rom", ti83_106);
			Option("TI83", "Rom", ti83_107);
			Option("TI83", "Rom", ti83_108);
			Option("TI83", "Rom", ti83p_103);
			Option("TI83", "Rom", ti83p_112);

			// mega cd
			var eu_mcd1_9210 = File("f891e0ea651e2232af0c5c4cb46a0cae2ee8f356", "eu_mcd1_9210.bin", "Mega CD EU (9210)");
			var eu_mcd2_9303 = File("7063192ae9f6b696c5b81bc8f0a9fe6f0c400e58", "eu_mcd2_9303.bin", "Mega CD EU (9303)");
			var eu_mcd2_9306 = File("523b3125fb0ac094e16aa072bc6ccdca22e520e5", "eu_mcd2_9306.bin", "Mega CD EU (9310)");
			var jp_mcd1_9111 = File("4846f448160059a7da0215a5df12ca160f26dd69", "jp_mcd1_9111.bin", "Mega CD JP (9111)");
			var jp_mcd1_9112 = File("e4193c6ae44c3cea002707d2a88f1fbcced664de", "jp_mcd1_9112.bin", "Mega CD JP (9112)");
			var us_scd1_9210 = File("f4f315adcef9b8feb0364c21ab7f0eaf5457f3ed", "us_scd1_9210.bin", "Sega CD US (9210)");
			var us_scd2_9303 = File("bd3ee0c8ab732468748bf98953603ce772612704", "us_scd2_9303.bin", "Sega CD US (9303)");

			Firmware("GEN", "CD_BIOS_EU", "Mega CD Bios (Europe)");
			Firmware("GEN", "CD_BIOS_JP", "Mega CD Bios (Japan)");
			Firmware("GEN", "CD_BIOS_US", "Sega CD Bios (USA)");
			Option("GEN", "CD_BIOS_EU", eu_mcd1_9210);
			Option("GEN", "CD_BIOS_EU", eu_mcd2_9303);
			Option("GEN", "CD_BIOS_EU", eu_mcd2_9306);
			Option("GEN", "CD_BIOS_JP", jp_mcd1_9111);
			Option("GEN", "CD_BIOS_JP", jp_mcd1_9112);
			Option("GEN", "CD_BIOS_US", us_scd1_9210);
			Option("GEN", "CD_BIOS_US", us_scd2_9303);	

			// SMS
			var sms_us_13 = File("C315672807D8DDB8D91443729405C766DD95CAE7", "sms_us_1.3.sms", "SMS BIOS 1.3 (USA, Europe)");
			var sms_jp_21 = File("A8C1B39A2E41137835EDA6A5DE6D46DD9FADBAF2", "sms_jp_2.1.sms", "SMS BIOS 2.1 (Japan)");
			var sms_us_1b = File("29091FF60EF4C22B1EE17AA21E0E75BAC6B36474", "sms_us_1.0b.sms", "SMS BIOS 1.0 (USA) (Proto)");
			var sms_m404  = File("4A06C8E66261611DCE0305217C42138B71331701", "sms_m404.sms", "SMS BIOS (USA) (M404) (Proto)");

			Firmware("SMS", "Export", "SMS Bios (USA/Export)");
			Firmware("SMS", "Japan", "SMS Bios (Japan)");
			Option("SMS", "Export", sms_us_13);
			Option("SMS", "Export", sms_us_1b);
			Option("SMS", "Export", sms_m404);
			Option("SMS", "Japan", sms_jp_21);
		}

		//adds a defined firmware ID to the database
		static void Firmware(string systemId, string id, string descr)
		{
			var fr = new FirmwareRecord
				{
					systemId = systemId,
					firmwareId = id,
					descr = descr
				};

			FirmwareRecords.Add(fr);
		}

		//adds an acceptable option for a firmware ID to the database
		static void Option(string hash, string systemId, string id)
		{
			var fo = new FirmwareOption
				{
					systemId = systemId,
					firmwareId = id,
					hash = hash
				};

			FirmwareOptions.Add(fo);
		}

		//adds an acceptable option for a firmware ID to the database
		static void Option(string systemId, string id, FirmwareFile ff)
		{
			Option(ff.hash, systemId, id);
		}

		//defines a firmware file
		static FirmwareFile File(string hash, string recommendedName, string descr)
		{
			string hashfix = hash.ToUpperInvariant();

			var ff = new FirmwareFile
				{
					hash = hashfix,
					recommendedName = recommendedName,
					descr = descr
				};
			FirmwareFiles.Add(ff);
			FirmwareFilesByHash[hashfix] = ff;
			return ff;
		}

		//adds a defined firmware ID and one file and option
		static void FirmwareAndOption(string hash, string systemId, string id, string name, string descr)
		{
			Firmware(systemId, id, descr);
			File(hash, name, descr);
			Option(hash, systemId, id);
		}


		public static List<FirmwareRecord> FirmwareRecords = new List<FirmwareRecord>();
		public static List<FirmwareOption> FirmwareOptions = new List<FirmwareOption>();
		public static List<FirmwareFile> FirmwareFiles = new List<FirmwareFile>();

		public static Dictionary<string, FirmwareFile> FirmwareFilesByHash = new Dictionary<string, FirmwareFile>();

		public class FirmwareFile
		{
			public string hash;
			public string recommendedName;
			public string descr;
		}

		public class FirmwareRecord
		{
			public string systemId;
			public string firmwareId;
			public string descr;
			public string ConfigKey { get { return string.Format("{0}+{1}", systemId, firmwareId); } }
		}

		public class FirmwareOption
		{
			public string systemId;
			public string firmwareId;
			public string hash;
			public string ConfigKey { get { return string.Format("{0}+{1}", systemId, firmwareId); } }
		}


		public static FirmwareRecord LookupFirmwareRecord(string sysId, string firmwareId)
		{
			var found =
				(from fr in FirmwareRecords
				 where fr.firmwareId == firmwareId
				 && fr.systemId == sysId
				 select fr);

			try
			{
				return found.First();
			}
			catch (InvalidOperationException)
			{
				// list is empty;
				return null;
			}
		}

	} //static class FirmwareDatabase
}