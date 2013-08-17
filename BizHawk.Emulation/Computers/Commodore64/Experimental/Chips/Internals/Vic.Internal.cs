﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Computers.Commodore64.Experimental.Chips.Internals
{
    public partial class Vic
    {
        int cachedADDR;
        bool cachedAEC;
        bool cachedBA;
        bool cachedCAS;
        int cachedDATA;
        bool cachedIRQ;
        bool cachedRAS;

        class Sprite
        {
            public int Color;
            public bool DataCollision;
            public bool Enabled;
            public bool ExpandX;
            public bool ExpandY;
            public bool Multicolor;
            public bool Priority;
            public bool SpriteCollision;
            public int X;
            public int Y;
        }

        bool ba;
        int[] backgroundColor;
        bool bitmapMode;
        int borderColor;
        bool cas;
        int characterBitmap;
        bool columnSelect;
        int data;
        bool dataCollisionInterrupt;
        bool displayEnable;
        bool extraColorMode;
        byte interruptEnableRegister;
        bool irq;
        bool lightPenInterrupt;
        int lightPenX;
        int lightPenY;
        bool multiColorMode;
        bool rasterInterrupt;
        int rasterX;
        int rasterY;
        bool reset;
        bool rowSelect;
        bool spriteCollisionInterrupt;
        int[] spriteMultiColor;
        Sprite[] sprites;
        int videoMemory;
        int xScroll;
        int yScroll;

        bool badLineCondition;
        bool badLineEnable;
        bool idleState;
        int pixelTimer;
        int rowCounter;
        int videoCounter;
        int videoCounterBase;
        int videoMatrixLineIndex;

        public void Clock()
        {
            if (pixelTimer == 0)
            {
                pixelTimer = 8;
                badLineEnable |= (rasterY == 0x30 && displayEnable);
                badLineCondition = (
                    badLineEnable &&
                    rasterY >= 0x030 &&
                    rasterY <= 0x0F7 &&
                    (rasterY & 0x007) == yScroll
                    );
                if (!idleState && badLineCondition)
                    idleState = true;
            }
            pixelTimer--;

        }
    }
}
