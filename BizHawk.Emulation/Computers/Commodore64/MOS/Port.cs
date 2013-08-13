﻿using System;

namespace BizHawk.Emulation.Computers.Commodore64.MOS
{
    public class LatchedPort
    {
        public byte Direction;
        public byte Latch;

        public LatchedPort()
        {
            Direction = 0x00;
            Latch = 0x00;
        }

        // data works like this in these types of systems:
        // 
        //  directionA  directionB  result
        //  0           0           1
        //  1           0           latchA
        //  0           1           latchB
        //  1           1           latchA && latchB
        //
        // however because this uses transistor logic, there are cases where wired-ands
        // cause the pull-up resistors not to be enough to keep the bus bit set to 1 when
        // both the direction and latch are 1 (the keyboard and joystick port 2 can do this.)
        // the class does not handle this case as it must be handled differently in every occurrence.

        public byte ReadInput(byte bus)
        {
            return (byte)((Latch & Direction) | ((Direction ^ 0xFF) & bus));
        }

        public byte ReadOutput()
        {
            return (byte)((Latch & Direction) | (Direction ^ 0xFF));
        }
    }
}
