﻿using System.Text;
namespace BizHawk
{
	public class CoreInputComm
	{
		public int NES_BackdropColor;
		public bool NES_UnlimitedSprites;
		public bool NES_ShowBG, NES_ShowOBJ;
		public bool PCE_ShowBG1, PCE_ShowOBJ1, PCE_ShowBG2, PCE_ShowOBJ2;
		public bool SMS_ShowBG, SMS_ShowOBJ;

		public string SNES_FirmwaresPath;
		public bool SNES_ShowBG1_0, SNES_ShowBG2_0, SNES_ShowBG3_0, SNES_ShowBG4_0;
		public bool SNES_ShowBG1_1, SNES_ShowBG2_1, SNES_ShowBG3_1, SNES_ShowBG4_1;
		public bool SNES_ShowOBJ_0, SNES_ShowOBJ_1, SNES_ShowOBJ_2, SNES_ShowOBJ_3;

		/// <summary>
		/// if this is set, then the cpu should dump trace info to CpuTraceStream
		/// </summary>
		public TraceBuffer Tracer = new TraceBuffer();
	}

	public class CoreOutputComm
	{
		public double VsyncRate
		{
			get
			{
				return VsyncNum / (double)VsyncDen;
			}
		}
		public int VsyncNum = 60;
		public int VsyncDen = 1;
		public string RomStatusAnnotation;
		public string RomStatusDetails;

		public int ScreenLogicalOffsetX, ScreenLogicalOffsetY;

		public bool CpuTraceAvailable = false;
	}

	public class TraceBuffer
	{
		public string TakeContents()
		{
			string s = buffer.ToString();
			buffer.Clear();
			return s;
		}

		public string Contents
		{
			get
			{
				return buffer.ToString();
			}
		}

		public void Put(string content)
		{
			if (logging)
			{
				buffer.Append(content);
				buffer.Append('\n');
			}
		}

		public TraceBuffer()
		{
			buffer = new StringBuilder();
		}

		public bool Enabled
		{
			get
			{
				return logging;
			}

			set
			{
				logging = value;
			}
		}

		private StringBuilder buffer;
		private bool logging = false;
	}
}
