﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Computers.Commodore64.Experimental.Chips.Internals
{
    sealed public partial class Vic
    {
        // inputs
        public Action ClockPhi0;
        public Func<int, int> ReadColorRam;
        public Func<int, int> ReadRam;

        // outputs
        public bool AEC { get { return true; } }
        public bool BA { get { return true; } }
        public bool IRQ { get { return true; } }
        public bool OutputAEC() { return true; }
        public bool OutputBA() { return true; }
        public bool OutputIRQ() { return true; }

        // exposed internal data
        public int Address { get { return address; } }
        public int CyclesPerFrame { get { return rasterCount * rasterWidth / 8; } }
        public int CyclesPerSecond { get { return frequency; } }
        public int Data { get { return data; } }
        public int DataPhi1 { get { return phi1Data; } }
    }
}
