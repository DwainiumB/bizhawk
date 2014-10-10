﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.MOS
{
	sealed public partial class Vic
	{
		const int BORDER_LEFT_38 = 0x01F;
		const int BORDER_LEFT_40 = 0x018;
		const int BORDER_RIGHT_38 = 0x14F;
		const int BORDER_RIGHT_40 = 0x158;

		// The special actions taken by the Vic are in the same order and interval on all chips, just different offsets.
		static private int[] TimingBuilder_Cycle14Act = new int[]
        {
			pipelineUpdateVc, 0,
			pipelineChkSprCrunch, 0,
			pipelineUpdateMcBase, 0,
        };
		static private int[] TimingBuilder_Cycle55Act = new int[]
        {
			pipelineChkSprDma, 0,
			pipelineChkSprDma | pipelineChkSprExp, 0,
			0, 0,
			pipelineChkSprDisp, pipelineUpdateRc
        };

		// This builds a table of special actions to take on each half-cycle. Cycle14 is the X-raster position where
		// pre-display operations happen, and Cycle55 is the X-raster position where post-display operations happen.
		static public int[] TimingBuilder_Act(int[] timing, int cycle14, int cycle55, int hblankStart, int hblankEnd)
		{
			List<int> result = new List<int>();

			int length = timing.Length;
			for (int i = 0; i < length; i++)
			{
				while (i < result.Count)
					i++;
				if (timing[i] == cycle14)
					result.AddRange(TimingBuilder_Cycle14Act);
				else if (timing[i] == cycle55)
					result.AddRange(TimingBuilder_Cycle55Act);
				else
					result.Add(0);
			}
			for (int i = 0; i < length; i++)
			{
				// pipeline raster X delay
				if (timing[(i + 1) % length] == timing[i])
					result[i] |= pipelineHoldX;

				// pipeline border checks
				if (timing[i] == (BORDER_LEFT_40 & 0xFFC))
					result[i] |= pipelineChkBrdL1;
				if (timing[i] == (BORDER_LEFT_38 & 0xFFC))
					result[i] |= pipelineChkBrdL0;
				if (timing[i] == (BORDER_RIGHT_38 & 0xFFC))
					result[i] |= pipelineChkBrdR0;
				if (timing[i] == (BORDER_RIGHT_40 & 0xFFC))
					result[i] |= pipelineChkBrdR1;
				if (timing[i] == (hblankStart & 0xFFC))
					result[i] |= pipelineHBlankR;
				if (timing[i] == (hblankEnd & 0xFFC))
					result[i] |= pipelineHBlankL;
			}

			return result.ToArray();
		}

		// This builds a table of how the BA pin is supposed to act on each half-cycle.
		static public int[] TimingBuilder_BA(int[] fetch)
		{
			int baRestart = 7;
			int start = 0;
			int length = fetch.Length;
			int[] result = new int[length];
			int[] spriteBA = new int[8];
			int charBA = 0;

			while (true)
			{
				if (fetch[start] == 0)
					break;
				start++;
			}

			while (true)
			{
				if (fetch[start] == 0x200)
					break;
				start--;
			}

			if (start < 0)
				start += length;
			int offset = start;

			while (true)
			{
				int ba = 0x0888;

				if (fetch[offset] == 0x200)
					charBA = baRestart;
				else if ((fetch[offset] & 0xFF00) == 0x0000)
					spriteBA[fetch[offset] & 0x007] = baRestart;

				for (int i = 0; i < 8; i++)
				{
					if (spriteBA[i] > 0)
					{
						ba <<= 4;
						ba |= i;
						spriteBA[i]--;
					}
				}
				ba &= 0x0FFF;

				if (charBA > 0)
				{
					ba = 0x1000;
					charBA--;
				}

				result[offset] = ba;

				offset--;
				if (offset < 0)
					offset += length;

				if (offset == start)
					break;
			}

			for (int i = 0; i < length; i += 2)
			{
				result[i] = result[i + 1];
			}

			return result;
		}

		// This builds a table of the fetch operations to take on each half-cycle.
		static public int[] TimingBuilder_Fetch(int[] timing, int sprite)
		{
			int length = timing.Length;
			int[] result = new int[length];
			int offset;
			int index = -1;
			int refreshCounter = 0;
			bool spriteActive = false;
			int spriteIndex = 0;
			int spritePhase = 0;
			int charCounter = 0;

			for (int i = 0; i < length; i++)
			{
				result[i++] = 0x500;
				result[i] = 0x100;
			}

			while (true)
			{
				index++;
				if (index >= length)
					index -= length;
				offset = timing[index];

				if (charCounter > 0)
				{
					result[index] = (charCounter & 1) == 0 ? 0x200 : 0x300;
					charCounter--;
					if (charCounter == 0)
						break;
				}

				if (refreshCounter > 0)
				{
					result[index] = (refreshCounter & 1) == 0 ? 0x500 : 0x100;
					refreshCounter--;
					if (refreshCounter == 0)
						charCounter = 80;
				}

				if (offset == sprite)
				{
					spriteActive = true;
				}

				if (spriteActive)
				{
					result[index] = (spriteIndex | (spritePhase << 4));
					spritePhase++;
					if (spritePhase == 4)
					{
						spritePhase = 0;
						spriteIndex++;
						if (spriteIndex == 8)
						{
							spriteActive = false;
							refreshCounter = 9;
						}
					}
				}
			}

			return result.ToArray();
		}

		// This uses the vBlank values to determine the height of the visible screen.
		static public int TimingBuilder_ScreenHeight(int vblankStart, int vblankEnd, int lines)
		{
			int offset = vblankEnd;
			int result = 0;
			while (true)
			{
				if (offset >= lines)
					offset -= lines;
				if (offset == vblankStart)
					return result;
				offset++;
				result++;
			}
		}

		// This uses the hBlank values to determine the width of the visible screen.
		static public int TimingBuilder_ScreenWidth(int[] timing, int hblankStart, int hblankEnd)
		{
			int length = timing.Length;
			int result = 0;
			int offset = 0;

			while (timing[offset] != hblankEnd) { offset = (offset + 1) % length; }
			while (timing[offset] != hblankStart) { offset = (offset + 1) % length; result++; }

			return (result * 4);
		}

		// This builds the table of X-raster positions. Start marks the position where the
		// Y-raster is incremented. Width is the position where the X-raster is reset to zero. Count
		// is the width of a rasterline in pixels. DelayOffset is the X-raster position where lag begins
		// (specifically on an NTSC 6567R8) and DelayAmount is the number of positions to lag.
		static public int[] TimingBuilder_XRaster(int start, int width, int count, int delayOffset, int delayAmount)
		{
			List<int> result = new List<int>();
			int rasterX = start;
			bool delayed = false;
			count >>= 2;
			delayAmount >>= 2;

			for (int i = 0; i < count; i++)
			{
				result.Add(rasterX);

				if (!delayed)
				{
					rasterX += 4;
					if (rasterX >= width)
						rasterX -= width;
				}
				else
				{
					delayAmount--;
					if (delayAmount <= 0)
						delayed = false;
					continue;
				}

				if (rasterX == delayOffset && delayAmount > 0)
					delayed = true;
			}

			return result.ToArray();
		}
	}
}
