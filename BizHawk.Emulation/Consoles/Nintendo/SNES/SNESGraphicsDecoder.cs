﻿//http://wiki.superfamicom.org/snes/show/Backgrounds
//http://board.zsnes.com/phpBB3/viewtopic.php?f=10&t=13029&start=75 yoshis island offset per tile demos. and other demos of advanced modes
//but we wont worry about offset per tile modes here.

using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace BizHawk.Emulation.Consoles.Nintendo.SNES
{

	public unsafe class SNESGraphicsDecoder
	{
		public struct Dimensions
		{
			public Dimensions(int w, int h) { Width = w; Height = h; }
			public int Width, Height;
			public override string ToString()
			{
				return string.Format("{0}x{1}", Width, Height);
			}
		}

		public enum ScreenSize
		{
			AAAA_32x32 = 0, ABAB_64x32 = 1, AABB_32x64 = 2, ABCD_64x64 = 3
		}

		public static Dimensions SizeInTilesForBGSize(ScreenSize size)
		{
			var ret = SizeInBlocksForBGSize(size);
			ret.Width *= 32;
			ret.Height *= 32;
			return ret;
		}

		public static Dimensions SizeInBlocksForBGSize(ScreenSize size)
		{
			switch (size)
			{
				case ScreenSize.AAAA_32x32: return new Dimensions(1, 1);
				case ScreenSize.ABAB_64x32: return new Dimensions(2, 1);
				case ScreenSize.AABB_32x64: return new Dimensions(1, 2);
				case ScreenSize.ABCD_64x64: return new Dimensions(2, 2);
				default: throw new Exception();
			}
		}

		public class BGInfo
		{
			/// <summary>
			/// Is the layer even enabled?
			/// </summary>
			public bool Enabled { get { return Bpp != 0; } }

			/// <summary>
			/// screen and tiledata register values
			/// </summary>
			public int SCADDR, TDADDR;

			/// <summary>
			/// SCSIZE register
			/// </summary>
			public int SCSIZE;

			/// <summary>
			/// the address of the screen data
			/// </summary>
			public int ScreenAddr { get { return SCADDR << 9; } }

			/// <summary>
			/// the address of the tile data
			/// </summary>
			public int TiledataAddr { get { return TDADDR << 13; } }

			/// <summary>
			/// Screen size (shape, really.)
			/// </summary>
			public ScreenSize ScreenSize { get { return (ScreenSize)SCSIZE; } }

			/// <summary>
			/// the BPP of the BG, as derived from the current mode
			/// </summary>
			public int Bpp;

			/// <summary>
			/// value of the tilesize register; 1 implies 16x16 tiles
			/// </summary>
			public int TILESIZE;

			/// <summary>
			/// TileSize; 8 or 16
			/// </summary>
			public int TileSize { get { return TILESIZE == 1 ? 16 : 8; } }

			/// <summary>
			/// The size of the layer, in tiles
			/// </summary>
			public Dimensions ScreenSizeInTiles { get { return SizeInTilesForBGSize(ScreenSize); } }

			/// <summary>
			/// The size of the layer, in pixels. This has factored in the selection of 8x8 or 16x16 tiles
			/// </summary>
			public Dimensions ScreenSizeInPixels
			{
				get
				{
					return new Dimensions(ScreenSizeInTiles.Width * TileSize, ScreenSizeInTiles.Height * TileSize);
				}
			}
		}

		public class BGInfos
		{
			BGInfo[] bgs = new BGInfo[4] { new BGInfo(), new BGInfo(), new BGInfo(), new BGInfo() };
			public BGInfo BG1 { get { return bgs[0]; } }
			public BGInfo BG2 { get { return bgs[1]; } }
			public BGInfo BG3 { get { return bgs[2]; } }
			public BGInfo BG4 { get { return bgs[3]; } }
			public BGInfo this[int index] { get { return bgs[index - 1]; } }
		}

		public class ModeInfo
		{
			/// <summary>
			/// the mode number, i.e. Mode 7
			/// </summary>
			public int MODE;
		}

		public class ScreenInfo
		{
			public BGInfos BG = new BGInfos();

			public ModeInfo Mode = new ModeInfo();
		}

		static int[,] ModeBpps = new[,] {
				{2,2,2,2},
				{4,4,2,0},
				{4,4,0,0},
				{8,4,0,0},
				{8,2,0,0},
				{4,2,0,0},
				{4,0,0,0},
				{8,0,0,0},
				{8,7,0,0}
			};


		public ScreenInfo ScanScreenInfo()
		{
			var si = new ScreenInfo();

			si.Mode.MODE = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG_MODE);
			si.BG.BG1.Bpp = ModeBpps[si.Mode.MODE, 0];
			si.BG.BG2.Bpp = ModeBpps[si.Mode.MODE, 1];
			si.BG.BG3.Bpp = ModeBpps[si.Mode.MODE, 2];
			si.BG.BG4.Bpp = ModeBpps[si.Mode.MODE, 3];

			si.BG.BG1.TILESIZE = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG1_TILESIZE);
			si.BG.BG2.TILESIZE = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG2_TILESIZE);
			si.BG.BG3.TILESIZE = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG3_TILESIZE);
			si.BG.BG4.TILESIZE = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG4_TILESIZE);

			si.BG.BG1.SCSIZE = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG1_SCSIZE);
			si.BG.BG2.SCSIZE = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG2_SCSIZE);
			si.BG.BG3.SCSIZE = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG3_SCSIZE);
			si.BG.BG4.SCSIZE = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG4_SCSIZE);
			si.BG.BG1.SCADDR = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG1_SCADDR);
			si.BG.BG2.SCADDR = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG2_SCADDR);
			si.BG.BG3.SCADDR = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG3_SCADDR);
			si.BG.BG4.SCADDR = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG4_SCADDR);
			si.BG.BG1.TDADDR = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG1_TDADDR);
			si.BG.BG2.TDADDR = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG2_TDADDR);
			si.BG.BG3.TDADDR = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG3_TDADDR);
			si.BG.BG4.TDADDR = LibsnesDll.snes_peek_logical_register(LibsnesDll.SNES_REG.BG4_TDADDR);

			return si;
		}

		//the same basic color table that libsnes uses to convert from snes 555 to rgba32
		public static int[] colortable = new int[16 * 32768];
		static SNESGraphicsDecoder()
		{
			for (int l = 0; l < 16; l++)
			{
				for (int r = 0; r < 32; r++)
				{
					for (int g = 0; g < 32; g++)
					{
						for (int b = 0; b < 32; b++)
						{
							//zero 04-sep-2012 - go ahead and turn this into a pixel format we'll want
							double luma = (double)l / 15.0;
							int ar = (int)(luma * r + 0.5);
							int ag = (int)(luma * g + 0.5);
							int ab = (int)(luma * b + 0.5);
							ar = ar * 255 / 31;
							ag = ag * 255 / 31;
							ab = ab * 255 / 31;
							int color = (ab << 16) + (ag << 8) + (ar << 0) | unchecked((int)0xFF000000);
							colortable[(l << 15) + (r << 10) + (g << 5) + (b << 0)] = color;
						}
					}
				}
			}
		}

		byte* vram;
		public ushort* cgram, vram16;
		public SNESGraphicsDecoder()
		{
			IntPtr block = LibsnesDll.snes_get_memory_data(LibsnesDll.SNES_MEMORY.VRAM);
			vram = (byte*)block.ToPointer();
			vram16 = (ushort*)vram;
			block = LibsnesDll.snes_get_memory_data(LibsnesDll.SNES_MEMORY.CGRAM);
			cgram = (ushort*)block.ToPointer();
		}

		public struct TileEntry
		{
			public ushort tilenum;
			public byte palette;
			public TileEntryFlags flags;
		}

		public enum TileEntryFlags : byte
		{
			Priority = 1, Horz = 2, Vert = 4,
		}

		/// <summary>
		/// decodes a BG. youll still need to paletteize and colorize it.
		/// someone else has to take care of calculating the starting color from the mode and layer number.
		/// </summary>
		public void DecodeBG(int* screen, int stride, TileEntry[] map, int tiledataBaseAddr, ScreenSize size, int bpp, int tilesize, int paletteStart)
		{
			int ncolors = 1 << bpp;

			int[] tileBuf = new int[16*16];
			var dims = SizeInTilesForBGSize(size);
			int count8x8 = tilesize / 8;
			int tileSizeBytes = 8 * bpp;
			int baseTileNum = tiledataBaseAddr / tileSizeBytes;
			int[] tileCache = _tileCache[bpp];

			int screenWidth = dims.Width * count8x8 * 8;

			for (int mty = 0; mty < dims.Height; mty++)
			{
				for (int mtx = 0; mtx < dims.Width; mtx++)
				{
					for (int tx = 0; tx < count8x8; tx++)
					{
						for (int ty = 0; ty < count8x8; ty++)
						{
							int mapIndex = (mty * count8x8 + ty) * dims.Width + mtx * count8x8 + tx;
							var te = map[mapIndex];
							int tileNum = te.tilenum + tx + ty * 16 + baseTileNum;
							int srcOfs = tileNum * 64;
							for (int i = 0, y = 0; y < 8; y++)
							{
								for (int x = 0; x < 8; x++, i++)
								{
									int px = x;
									int py = y;
									if (te.flags.HasFlag(TileEntryFlags.Horz)) px = 7 - x;
									if (te.flags.HasFlag(TileEntryFlags.Vert)) py = 7 - y;
									int dstX = (mtx * count8x8 + tx) * 8 + px;
									int dstY = (mty * count8x8 + ty) * 8 + py;
									int dstOfs = dstY * stride + dstX;
									int color = tileCache[srcOfs++];
									color += te.palette * ncolors;
									color += paletteStart;
									screen[dstOfs] = color;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// fetches a tilemap. this is simple; apparently only the screen size (shape) is a factor
		/// </summary>
		public TileEntry[] FetchTilemap(int addr, ScreenSize size)
		{
			var blockDims = SizeInBlocksForBGSize(size);
			int blocksw = blockDims.Width;
			int blocksh = blockDims.Height;
			int width = blockDims.Width * 32;
			int height = blockDims.Height * 32;
			TileEntry[] buf = new TileEntry[width*height];

			for (int by = 0; by < blocksh; by++)
			{
				for (int bx = 0; bx < blocksw; bx++)
				{
					for (int y = 0; y < 32; y++)
					{
						for (int x = 0; x < 32; x++)
						{
							int idx = (by * 32 + y) * width + bx * 32 + x;
							ushort entry = *(ushort*)(vram + addr);
							buf[idx].tilenum = (ushort)(entry & 0x3FF);
							buf[idx].palette = (byte)((entry >> 10) & 7);
							buf[idx].flags = (TileEntryFlags)((entry >> 13) & 7);
							addr += 2;
						}
					}
				}
			}

			return buf;
		}

		//TODO - paletteize and colorize could be in one step, for more speed
		public void Paletteize(int* buf, int offset, int startcolor, int numpixels)
		{
			for (int i = 0; i < numpixels; i++)
			{
				buf[offset + i] = cgram[startcolor + buf[offset + i]] & 0x7FFF; //unfortunate that we have to mask this here.. maybe do it in a more optimal spot when we port it to c++
			}
		}
		public void Colorize(int* buf, int offset, int numpixels)
		{
			for (int i = 0; i < numpixels; i++)
			{
				buf[offset + i] = colortable[491520 + buf[offset + i]];
			}
		}

		int[][] _tileCache = new int[9][];

		/// <summary>
		/// Caches all tiles at the 2bpp, 4bpp, and 8bpp decoded states
		/// </summary>
		public void CacheTiles()
		{
			//generate 2bpp tiles
			int numtiles = 8192;
			int[] tiles = new int[8 * 8 * numtiles];
			_tileCache[2] = tiles;
			for (int i = 0; i < numtiles; i++)
			{
				Decode8x8x2bpp(tiles, i * 64, 16 * i, 8);
			}

			//merge 2bpp tiles into 4bpp and 8bpp
			CacheTiles_Merge(2);
			CacheTiles_Merge(4);
		}

		/// <summary>
		/// merges one type of tiles with another to create the higher-order bitdepth.
		/// TODO - templateize this when we change it to c++
		/// </summary>
		void CacheTiles_Merge(int fromBpp)
		{
			int toBpp = fromBpp * 2;
			int shift = fromBpp;
			int numtiles = 8192 / toBpp;
			int[] tilesDst = new int[8 * 8 * numtiles];
			_tileCache[toBpp] = tilesDst;
			int[] tilesSrc = _tileCache[2];

			for (int i = 0; i < numtiles; i++)
			{
				int srcAddr = i * 128;
				int dstAddr = i * 64;
				for (int p = 0; p < 64; p++)
				{
					int tileA = tilesSrc[srcAddr + p];
					int tileB = tilesSrc[srcAddr + p + 64];
					tilesDst[dstAddr + p] = tileA | (tileB << shift);
				}
			}
		}

		/// <summary>
		/// decodes an 8x8 tile to a linear framebuffer type thing. fundamental unit of tile decoding.
		/// </summary>
		public void Decode8x8x2bpp(int[] buf, int offset, int addr, int stride=8)
		{
			for (int y = 0; y < 8; y++)
			{
				byte val = vram[addr + 1];
				for (int x = 0; x < 8; x++) buf[offset + y * stride + x] = val >> (7 - x) & 1;
				val = vram[addr + 0];
				for (int x = 0; x < 8; x++) buf[offset + y * stride + x] = (buf[offset + y * stride + x] << 1) | (val >> (7 - x) & 1);
				addr += 2;
			}
		}

		public static Dimensions GetDimensionsForTileScreen(int bpp)
		{
			if (bpp == 2) return new Dimensions(512, 512);
			if (bpp == 4) return new Dimensions(512, 256);
			if (bpp == 8) return new Dimensions(256, 256);
			throw new ApplicationException("weird");
		}

		/// <summary>
		/// renders the tiles to a screen with the predefined size
		/// we might need 16x16 unscrambling and some other perks here eventually.
		/// provide a start color to use as the basis for the palette
		/// </summary>
		public void RenderTilesToScreen(int* screen, int stride, int bpp, int startcolor)
		{
			var dims = GetDimensionsForTileScreen(bpp);
			int tilesw = dims.Width / 8;
			int numTiles = 8192 / bpp;
			int[] tilebuf = _tileCache[bpp];
			for (int i = 0; i < numTiles; i++)
			{
				int ty = i / tilesw;
				int tx = i % tilesw;
				int dstOfs = (ty*8) * stride + tx*8;
				int srcOfs = i * 64;
				for (int y = 0,p=0; y < 8; y++)
					for (int x = 0; x < 8; x++,p++)
					{
						screen[dstOfs+y*stride+x] = tilebuf[srcOfs + p];
					}
			}

			int numPixels = numTiles * 8 * 8;
			Paletteize(screen, 0, startcolor, numPixels);
			Colorize(screen, 0, numPixels);
		}

	
	
	} //class SNESGraphicsDecoder
} //namespace


//GraphicsDecoder dec = new GraphicsDecoder();
//int[] tilebuf = new int[8 * 8];
//int[] screen = new int[64*64*8*8];
//for (int i = 0; i < 64 * 64; i++)
//{
//  dec.Decode8x8x2bpp(tilebuf, 0, 16 * i);
//  int ty = i / 64;
//  int tx = i % 64;
//  ty *= 8;
//  tx *= 8;
//  for(int y=0;y<8;y++)
//    for (int x = 0; x < 8; x++)
//    {
//      screen[(ty + y) * 512 + tx + x] = tilebuf[y * 8 + x];
//    }
//}
//dec.Paletteize2bpp(screen, 0, 0, 64 * 64 * 8 * 8);
//dec.Colorize(screen, 0, 64 * 64 * 8 * 8);
//MemoryStream ms = new MemoryStream();
//foreach (int i in screen)
//{
//  ms.WriteByte((byte)(i & 0xFF));
//  ms.WriteByte((byte)((i >> 8) & 0xFF));
//  ms.WriteByte((byte)((i >> 16) & 0xFF));
//}
//File.WriteAllBytes("c:\\dump\\file" + ctr, ms.ToArray());
//ctr++;


		//public void Decode8x8x4bpp(int[] buf, int offset, int addr, int stride=8)
		//{
		//  for (int y = 0; y < 8; y++)
		//  {
		//    byte val = vram[addr + 17];
		//    for (int x = 0; x < 8; x++) buf[offset + y * stride + x] = val >> (7 - x) & 1;
		//    val = vram[addr + 16];
		//    for (int x = 0; x < 8; x++) buf[offset + y * stride + x] = (buf[offset + y * stride + x] << 1) | (val >> (7 - x) & 1);
		//    val = vram[addr + 1];
		//    for (int x = 0; x < 8; x++) buf[offset + y * stride + x] = (buf[offset + y * stride + x] << 1) | (val >> (7 - x) & 1);
		//    val = vram[addr + 0];
		//    for (int x = 0; x < 8; x++) buf[offset + y * stride + x] = (buf[offset + y * stride + x] << 1) | (val >> (7 - x) & 1);
		//    addr += 2;
		//  }
		//}

		//public void Decode8x8x8bpp(int[] buf, int offset, int addr, int stride=8)
		//{
		//  for (int y = 0; y < 8; y++)
		//  {
		//    byte val = vram[addr + 49];
		//    for (int x = 0; x < 8; x++) buf[offset + y * 8 + x] = (val >> (7 - x) & 1);
		//    val = vram[addr + 48];
		//    for (int x = 0; x < 8; x++) buf[offset + y * 8 + x] = (buf[offset + y * 8 + x] << 1) | (val >> (7 - x) & 1);
		//    val = vram[addr + 33];
		//    for (int x = 0; x < 8; x++) buf[offset + y * 8 + x] = (buf[offset + y * 8 + x] << 1) | (val >> (7 - x) & 1);
		//    val = vram[addr + 32];
		//    for (int x = 0; x < 8; x++) buf[offset + y * 8 + x] = (buf[offset + y * 8 + x] << 1) | (val >> (7 - x) & 1);
		//    val = vram[addr + 17];
		//    for (int x = 0; x < 8; x++) buf[offset + y * 8 + x] = (buf[offset + y * 8 + x] << 1) | (val >> (7 - x) & 1);
		//    val = vram[addr + 16];
		//    for (int x = 0; x < 8; x++) buf[offset + y * 8 + x] = (buf[offset + y * 8 + x] << 1) | (val >> (7 - x) & 1);
		//    val = vram[addr + 1];
		//    for (int x = 0; x < 8; x++) buf[offset + y * 8 + x] = (buf[offset + y * 8 + x] << 1) | (val >> (7 - x) & 1);
		//    val = vram[addr + 0];
		//    for (int x = 0; x < 8; x++) buf[offset + y * 8 + x] = val >> (7 - x) & 1;
		//    addr += 2;
		//  }
		//}


	///// <summary>
	//  /// decodes all the tiles in vram as if they were 2bpp tiles to a 64x64 tile (512x512 pixel) screen
	//  /// </summary>
	//  public void DecodeTiles2bpp(int* screen, int stride, int startcolor)
	//  {
	//    //cant handle this with our speed optimized routines
	//    Debug.Assert(stride == 512);
			
	//    int[] tilebuf = new int[8 * 8];
	//    for (int i = 0; i < 64 * 64; i++)
	//    {
	//      Decode8x8x2bpp(tilebuf, 0, 16 * i);
	//      int ty = i / 64;
	//      int tx = i % 64;
	//      ty *= 8;
	//      tx *= 8;
	//      for (int y = 0; y < 8; y++)
	//        for (int x = 0; x < 8; x++)
	//        {
	//          screen[(ty + y) * stride + tx + x] = tilebuf[y * 8 + x];
	//        }
	//    }

	//    Paletteize(screen, 0, startcolor, 64 * 64 * 8 * 8);
	//    Colorize(screen, 0, 64 * 64 * 8 * 8);
	//  }

	//  /// <summary>
	//  /// decodes all the tiles in vram as if they were 4bpp tiles to a 64x32 tile (512x256 pixel) screen
	//  /// </summary>
	//  public void DecodeTiles4bpp(int* screen, int stride, int startcolor)
	//  {
	//    ////cant handle this with our speed optimized routines
	//    //Debug.Assert(stride == 512);

	//    //for (int i = 0; i < 64 * 32; i++)
	//    //{
	//    //  int ty = i / 64;
	//    //  int tx = i % 64;
	//    //  ty *= 8;
	//    //  tx *= 8;
	//    //  for (int y = 0; y < 8; y++)
	//    //    for (int x = 0; x < 8; x++)
	//    //    {
	//    //      screen[(ty + y) * stride + tx + x] = tilebuf[y * 8 + x];
	//    //    }
	//    //}

	//    //Paletteize(screen, 0, startcolor, 64 * 32 * 8 * 8);
	//    //Colorize(screen, 0, 64 * 32 * 8 * 8);
	//  }

	//  /// <summary>
	//  /// decodes all the tiles in vram as if they were 4bpp tiles to a 32x32 tile (256x256 pixel) screen
	//  /// </summary>
	//  public void DecodeTiles8bpp(int* screen, int stride, int startcolor)
	//  {
	//    ////cant handle this with our speed optimized routines
	//    //Debug.Assert(stride == 256);

	//    //int[] tilebuf = new int[8 * 8];
	//    //for (int i = 0; i < 32 * 32; i++)
	//    //{
	//    //  Decode8x8x8bpp(tilebuf, 0, 64 * i);
	//    //  int ty = i / 32;
	//    //  int tx = i % 32;
	//    //  ty *= 8;
	//    //  tx *= 8;
	//    //  for (int y = 0; y < 8; y++)
	//    //    for (int x = 0; x < 8; x++)
	//    //    {
	//    //      screen[(ty + y) * stride + tx + x] = tilebuf[y * 8 + x];
	//    //    }
	//    //}

	//    //Paletteize(screen, 0, startcolor, 32 * 32 * 8 * 8);
	//    //Colorize(screen, 0, 32 * 32 * 8 * 8);
	//  }