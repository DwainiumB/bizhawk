﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BizHawk.Common;

namespace BizHawk.Emulation.Computers.Commodore64.Experimental.Chips.Internals
{
	public class Cassette
	{
		public Func<bool> InputData;
		public Func<bool> InputMotor;

		virtual public bool Data { get { return true; } }
		public bool OutputData() { return Data; }
		public bool OutputSense() { return Sense; }
		virtual public bool Sense { get { return true; } }
		virtual public void SyncState(Serializer ser) { SaveState.SyncObject(ser, this); }
	}
}
