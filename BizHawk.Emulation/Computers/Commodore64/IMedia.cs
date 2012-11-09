﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Computers.Commodore64
{
	public interface IMedia
	{
		void Apply();
		bool Loaded();
		bool Ready();
	}
}
