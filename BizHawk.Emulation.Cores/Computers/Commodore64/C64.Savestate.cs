﻿using System.IO;

using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Computers.Commodore64
{
	sealed public partial class C64 : IEmulator
	{
		public void ClearSaveRam()
		{
		}

		public void LoadStateBinary(BinaryReader br)
		{
			SyncState(new Serializer(br));
		}

		public void LoadStateText(TextReader reader)
		{
			SyncState(new Serializer(reader));
		}

		public byte[] CloneSaveRam()
		{
			return null;
		}

		// TODO: when disk support is finished, set this flag according to if any writes to disk were done
		public bool SaveRamModified
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public void SaveStateBinary(BinaryWriter bw)
		{
			SyncState(new Serializer(bw));
		}

		public void SaveStateText(TextWriter writer)
		{
			SyncState(new Serializer(writer));
		}

		public void StoreSaveRam(byte[] data)
		{
		}

		void SyncState(Serializer ser)
		{
			ser.BeginSection("core");
			board.SyncState(ser);
			ser.EndSection();
		}
	}
}
