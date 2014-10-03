﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Cores.Computers.Commodore64
{
	sealed public partial class Vic
	{
		int delayC;
		int ecmPixel;
		int pixel;
		int pixelData;
		SpriteGenerator pixelOwner;
		int sprData;
		int sprPixel;
		int srC = 0;
		int srSync = 0;
		VicVideoMode videoMode;

		enum VicVideoMode : int
		{
			Mode000,
			Mode001,
			Mode010,
			Mode011,
			Mode100,
			ModeBad
		}

		private void Render()
		{
			if (hblankCheckEnableL)
			{
				if (rasterX == hblankEnd)
					hblank = false;
			}
			else if (hblankCheckEnableR)
			{
				if (rasterX == hblankStart)
					hblank = true;
			}

			renderEnabled = (!hblank && !vblank);
			for (int i = 0; i < 4; i++)
			{

				if (delayC > 0)
					delayC--;
				else
					displayC = (srC >> 12) & 0xFFF;


				if (borderCheckLEnable && (rasterX == borderL))
				{
					if (rasterLine == borderB)
						borderOnVertical = true;
					if (rasterLine == borderT && displayEnable)
						borderOnVertical = false;
					if (!borderOnVertical)
						borderOnMain = false;
				}

				switch (videoMode)
				{
					case VicVideoMode.Mode000:
						pixelData = sr & srMask2;
						pixel = (pixelData != 0) ? (displayC >> 8) : backgroundColor0;
						break;
					case VicVideoMode.Mode001:
						if ((displayC & 0x800) != 0)
						{
							// multicolor 001
							if ((srSync & srMask2) != 0)
								pixelData = sr & srMask3;

							if (pixelData == 0)
								pixel = backgroundColor0;
							else if (pixelData == srMask1)
								pixel = backgroundColor1;
							else if (pixelData == srMask2)
								pixel = backgroundColor2;
							else
								pixel = (displayC & 0x700) >> 8;
						}
						else
						{
							// standard 001
							pixelData = sr & srMask2;
							pixel = (pixelData != 0) ? (displayC >> 8) : backgroundColor0;
						}
						break;
					case VicVideoMode.Mode010:
						pixelData = sr & srMask2;
						pixel = (pixelData != 0) ? (displayC >> 4) : (displayC);
						break;
					case VicVideoMode.Mode011:
						if ((srSync & srMask2) != 0)
							pixelData = sr & srMask3;

						if (pixelData == 0)
							pixel = backgroundColor0;
						else if (pixelData == srMask1)
							pixel = (displayC >> 4);
						else if (pixelData == srMask2)
							pixel = displayC;
						else
							pixel = (displayC >> 8);
						break;
					case VicVideoMode.Mode100:
						pixelData = sr & srMask2;
						if (pixelData != 0)
						{
							pixel = displayC >> 8;
						}
						else
						{
							ecmPixel = (displayC) & 0xC0;
							if (ecmPixel == 0x00)
								pixel = backgroundColor0;
							else if (ecmPixel == 0x40)
								pixel = backgroundColor1;
							else if (ecmPixel == 0x80)
								pixel = backgroundColor2;
							else
								pixel = backgroundColor3;
						}
						break;
					default:
						pixelData = 0;
						pixel = 0;
						break;
				}
				pixel &= 0xF;
				sr <<= 1;
				srSync <<= 1;

				// render sprite
				pixelOwner = null;
				foreach (SpriteGenerator spr in sprites)
				{
					sprData = 0;
					sprPixel = pixel;
                    spr.srMask &= 0xFFFFFF;

					if (spr.x == rasterX)
                        spr.shiftEnable = spr.display && spr.srMask != 0;

					if (spr.shiftEnable) // sprite rule 6
					{
						if (spr.multicolor)
						{
							sprData = (spr.sr & srSpriteMaskMC);
                            if (spr.multicolorCrunch && spr.xCrunch && !rasterXHold)
                            {
                                spr.sr <<= 2;
                                spr.srMask <<= 2;
                            }
							spr.multicolorCrunch ^= spr.xCrunch;
						}
						else
						{
							sprData = (spr.sr & srSpriteMask);
                            if (spr.xCrunch && !rasterXHold)
                            {
                                spr.sr <<= 1;
                                spr.srMask <<= 1;
                            }
						}
						spr.xCrunch ^= spr.xExpand;

						if (sprData != 0)
						{
							// sprite-sprite collision
							if (pixelOwner == null)
							{
                                if (sprData == srSpriteMask1)
                                    sprPixel = spriteMulticolor0;
                                else if (sprData == srSpriteMask2)
                                    sprPixel = spr.color;
                                else if (sprData == srSpriteMask3)
                                    sprPixel = spriteMulticolor1;
                                pixelOwner = spr;
							}
							else
							{
								if (!borderOnVertical)
								{
									spr.collideSprite = true;
									pixelOwner.collideSprite = true;
								}
							}

							// sprite-data collision
							if (!borderOnVertical && (pixelData >= srMask2))
							{
								spr.collideData = true;
							}

                            // sprite priority logic
                            if (spr.priority)
                            {
                                pixel = (pixelData >= srMask2) ? pixel : sprPixel;
                            }
                            else
                            {
                                pixel = sprPixel;
                            }
                        }
                        if (spr.srMask == 0)
							spr.shiftEnable = false;
                        //pixel = (spr.mcbase / 3) & 0xF;
					}
				}

				if (borderCheckREnable && (rasterX == borderR))
					borderOnMain = true;

				// border doesn't work with the background buffer
				if (borderOnMain || borderOnVertical)
					pixel = borderColor;

				// plot pixel if within viewing area
				if (renderEnabled)
				{
					buf[bufOffset] = palette[pixBuffer[pixBufferIndex]];
					bufOffset++;
					if (bufOffset == bufLength)
						bufOffset = 0;
				}

				pixBuffer[pixBufferIndex] = pixel;
				pixBufferIndex++;

				if (!rasterXHold)
					rasterX++;
				bitmapColumn++;
			}

			if (pixBufferIndex >= pixBufferSize)
				pixBufferIndex = 0;
		}
	}
}
