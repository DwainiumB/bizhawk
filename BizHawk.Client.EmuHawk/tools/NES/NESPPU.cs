﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using BizHawk.Client.Common;
using BizHawk.Emulation.Cores.Nintendo.NES;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class NesPPU : Form, IToolForm
	{
		// TODO:
		// If 8/16 sprite mode, mouse over should put 32x64 version of prite
		// Speedups
		// Smarter refreshing?  only refresh when things of changed, perhaps peek at the ppu to when the pattern table has changed, or sprites have moved
		// Maybe 48 individual bitmaps for sprites is faster than the overhead of redrawing all that transparent space
		private readonly byte[] _ppuBus = Enumerable.Repeat((byte)0, 0x2000).ToArray();
		private readonly byte[] _ppuBusprev = Enumerable.Repeat((byte)0, 0x2000).ToArray();
		private readonly byte[] _palRam = Enumerable.Repeat((byte)0, 0x20).ToArray();
		private readonly byte[] _palRamPrev = Enumerable.Repeat((byte)0, 0x20).ToArray();
		private readonly NES.PPU.DebugCallback _callback = new NES.PPU.DebugCallback();

		private Bitmap _zoomBoxDefaultImage = new Bitmap(64, 64);
		private NES _nes;
		private bool _forceChange;

		public NesPPU()
		{
			InitializeComponent();
			Closing += (o, e) =>
				{
					Global.Config.NesPPUSettings.Wndx = Location.X;
					Global.Config.NesPPUSettings.Wndy = Location.Y;
					Global.Config.NESPPURefreshRate = RefreshRate.Value;
				};
			TopMost = Global.Config.NesPPUSettings.TopMost;
			_callback.Callback = () => Generate();
			CalculateFormSize();
		}

		private void NesPPU_Load(object sender, EventArgs e)
		{
			LoadConfigSettings();
			_nes = Global.Emulator as NES;
			ClearDetails();
			RefreshRate.Value = Global.Config.NESPPURefreshRate;
			Generate(true);
			CHRROMViewReload();
		}

		#region Public API

		public bool AskSave() { return true; }
		public bool UpdateBefore { get { return true; } }

		public void UpdateValues()
		{
			if (Global.Emulator is NES)
			{
				_nes.ppu.PPUViewCallback = _callback;
			}
			else
			{
				Close();
			}
		}

		public void Restart()
		{
			if (Global.Emulator is NES)
			{
				_nes = Global.Emulator as NES;
				Generate(true);
				CHRROMViewReload();
			}
			else
			{
				Close();
			}
		}

		#endregion

		private void LoadConfigSettings()
		{
			if (Global.Config.NesPPUSettings.UseWindowPosition)
			{
				Location = Global.Config.NesPPUSettings.WindowPosition;
			}
		}

		private byte GetBit(int address, int bit)
		{
			return (byte)((_ppuBus[address] >> (7 - bit)) & 1);
		}
		
		private bool CheckChange()
		{
			var changed = false;
			for (var i = 0; i < 0x20; i++)
			{
				_palRamPrev[i] = _palRam[i];
				_palRam[i] = _nes.ppu.PALRAM[i];
				if (_palRam[i] != _palRamPrev[i])
				{
					changed = true;
				}
			}

			for (var i = 0; i < 0x2000; i++)
			{
				_ppuBusprev[i] = _ppuBus[i];
				_ppuBus[i] = _nes.ppu.ppubus_peek(i);
				if (_ppuBus[i] != _ppuBusprev[i])
				{
					changed = true;
				}
			}

			if (_forceChange)
			{
				return true;
			}

			return changed;
		}

		private unsafe void DrawPatternView(PatternViewer dest, byte[] src)
		{
			int b0;
			int b1;
			byte value;
			int cvalue;

			var bmpdata = dest.pattern.LockBits(
				new Rectangle(new Point(0, 0), dest.pattern.Size),
				ImageLockMode.WriteOnly,
				PixelFormat.Format32bppArgb);

			var framebuf = (int*)bmpdata.Scan0.ToPointer();
			for (int z = 0; z < 2; z++)
			{
				int pal;
				pal = z == 0 ? PatternView.Pal0 : PatternView.Pal1; // change?

				for (int i = 0; i < 16; i++)
				{
					for (int j = 0; j < 16; j++)
					{
						for (int x = 0; x < 8; x++)
						{
							for (int y = 0; y < 8; y++)
							{
								int address = (z << 12) + (i << 8) + (j << 4) + y;
								b0 = (byte)((src[address] >> (7 - x)) & 1);
								b1 = (byte)((src[address + 8] >> (7 - x)) & 1);

								value = (byte)(b0 + (b1 << 1));
								cvalue = _nes.LookupColor(_nes.ppu.PALRAM[value + (pal << 2)]);
								int adr = (x + (j << 3)) + (y + (i << 3)) * (bmpdata.Stride >> 2);
								framebuf[adr + (z << 7)] = cvalue;
							}
						}
					}
				}
			}

			dest.pattern.UnlockBits(bmpdata);
			dest.Refresh();
		}

		private unsafe void Generate(bool now = false)
		{
			if (!IsHandleCreated || IsDisposed)
			{
				return;
			}

			if (Global.Emulator.Frame % RefreshRate.Value == 0 || now)
			{
				int b0;
				int b1;
				byte value;
				int cvalue;

				if (CheckChange())
				{
					_forceChange = false;

					// Pattern Viewer
					for (var i = 0; i < 16; i++)
					{
						PaletteView.BgPalettesPrev[i].Value = PaletteView.BgPalettes[i].Value;
						PaletteView.SpritePalettesPrev[i].Value = PaletteView.SpritePalettes[i].Value;
						PaletteView.BgPalettes[i].Value = _nes.LookupColor(_nes.ppu.PALRAM[PaletteView.BgPalettes[i].Address]);
						PaletteView.SpritePalettes[i].Value = _nes.LookupColor(_nes.ppu.PALRAM[PaletteView.SpritePalettes[i].Address]);
					}

					if (PaletteView.HasChanged())
					{
						PaletteView.Refresh();
					}

					DrawPatternView(PatternView, _ppuBus);
				}

				var bmpdata2 = SpriteView.sprites.LockBits(new Rectangle(new Point(0, 0), SpriteView.sprites.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				var framebuf2 = (int*)bmpdata2.Scan0.ToPointer();

				int pt_add = _nes.ppu.reg_2000.obj_pattern_hi ? 0x1000 : 0;
				bool is8x16 = _nes.ppu.reg_2000.obj_size_16;


				// Sprite Viewer
				for (int n = 0; n < 4; n++)
				{
					for (int r = 0; r < 16; r++)
					{
						int BaseAddr = (r << 2) +  (n << 6);
						int TileNum = _nes.ppu.OAM[BaseAddr + 1];
						int patternAddr;

						if (is8x16)
						{
							patternAddr = (TileNum >> 1) * 0x20;
							patternAddr += 0x1000 * (TileNum & 1);
						}
						else
						{
							patternAddr = TileNum * 0x10;
							patternAddr += pt_add;
						}

						int Attributes = _nes.ppu.OAM[BaseAddr + 2];
						int Palette = Attributes & 0x03;

						for (int x = 0; x < 8; x++)
						{
							for (int y = 0; y < 8; y++)
							{
								int address = patternAddr + y;
								b0 = (byte)((_ppuBus[address] >> (7 - x)) & 1);
								b1 = (byte)((_ppuBus[address + 8] >> (7 - x)) & 1);
								value = (byte)(b0 + (b1 << 1));
								cvalue = _nes.LookupColor(_nes.ppu.PALRAM[16 + value + (Palette << 2)]);

								int adr = (x + (r * 16)) + (y + (n * 24)) * (bmpdata2.Stride >> 2);
								framebuf2[adr] = cvalue;
							}

							if (is8x16)
							{
								patternAddr += 0x10;
								for (int y = 0; y < 8; y++)
								{
									int address = patternAddr + y;
									b0 = (byte)((_ppuBus[address] >> (7 - x)) & 1);
									b1 = (byte)((_ppuBus[address + 8] >> (7 - x)) & 1);
									value = (byte)(b0 + (b1 << 1));
									cvalue = _nes.LookupColor(_nes.ppu.PALRAM[16 + value + (Palette << 2)]);

									int adr = (x + (r << 4)) + ((y+8) + (n * 24)) * (bmpdata2.Stride >> 2);
									framebuf2[adr] = cvalue;
								}

								patternAddr -= 0x10;
							}
						}
					}
				}

				SpriteView.sprites.UnlockBits(bmpdata2);
				SpriteView.Refresh();

				HandleSpriteViewMouseMove(SpriteView.PointToClient(MousePosition));
				HandlePaletteViewMouseMove(PaletteView.PointToClient(MousePosition));
			}
		}

		private void ClearDetails()
		{
			DetailsBox.Text = "Details";
			AddressLabel.Text = string.Empty;
			ValueLabel.Text = string.Empty;
			Value2Label.Text = string.Empty;
			Value3Label.Text = string.Empty;
			Value4Label.Text = string.Empty;
			Value5Label.Text = string.Empty;
			ZoomBox.Image = _zoomBoxDefaultImage;
		}

		private void UpdatePaletteSelection()
		{
			_forceChange = true;
			Table0PaletteLabel.Text = "Palette: " + PatternView.Pal0;
			Table1PaletteLabel.Text = "Palette: " + PatternView.Pal1;
		}

		private static Bitmap Section(Image srcBitmap, Rectangle section, bool is8x16)
		{
			// Create the new bitmap and associated graphics object
			var bmp = new Bitmap(64, 64);
			var g = Graphics.FromImage(bmp);

			// Draw the specified section of the source bitmap to the new one
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

			var rect = is8x16 ? new Rectangle(0, 0, 32, 64) : new Rectangle(0, 0, 64, 64);
			g.DrawImage(srcBitmap, rect, section, GraphicsUnit.Pixel);
			g.Dispose();

			return bmp;
		}

		private void HandleDefaultImage()
		{
			if (ModifierKeys == Keys.Shift)
			{
				_zoomBoxDefaultImage = ZoomBox.Image as Bitmap;
			}
		}

		private void RefreshFloatingWindowControl()
		{
			Owner = Global.Config.NesPPUSettings.FloatingWindow ? null : GlobalWin.MainForm;
		}

		#region Events

		#region Menu and Context Menu

		#region File

		private void SavePaletteScreenshotMenuItem_Click(object sender, EventArgs e)
		{
			PaletteView.Screenshot();
		}

		private void SavePatternScreenshotMenuItem_Click(object sender, EventArgs e)
		{
			PatternView.Screenshot();
		}

		private void SaveSpriteScreenshotMenuItem_Click(object sender, EventArgs e)
		{
			SpriteView.Screenshot();
		}

		private void CopyPaletteToClipboardMenuItem_Click(object sender, EventArgs e)
		{
			PaletteView.ScreenshotToClipboard();
		}

		private void CopyPatternToClipboardMenuItem_Click(object sender, EventArgs e)
		{
			PatternView.ScreenshotToClipboard();
		}

		private void CopySpriteToClipboardMenuItem_Click(object sender, EventArgs e)
		{
			SpriteView.ScreenshotToClipboard();
		}

		private void ExitMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion

		#region Pattern

		private void Table0PaletteSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			Table0P0MenuItem.Checked = PatternView.Pal0 == 0;
			Table0P1MenuItem.Checked = PatternView.Pal0 == 1;
			Table0P2MenuItem.Checked = PatternView.Pal0 == 2;
			Table0P3MenuItem.Checked = PatternView.Pal0 == 3;
			Table0P4MenuItem.Checked = PatternView.Pal0 == 4;
			Table0P5MenuItem.Checked = PatternView.Pal0 == 5;
			Table0P6MenuItem.Checked = PatternView.Pal0 == 6;
			Table0P7MenuItem.Checked = PatternView.Pal0 == 7;
		}

		private void Table1PaletteSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			Table1P0MenuItem.Checked = PatternView.Pal1 == 0;
			Table1P1MenuItem.Checked = PatternView.Pal1 == 1;
			Table1P2MenuItem.Checked = PatternView.Pal1 == 2;
			Table1P3MenuItem.Checked = PatternView.Pal1 == 3;
			Table1P4MenuItem.Checked = PatternView.Pal1 == 4;
			Table1P5MenuItem.Checked = PatternView.Pal1 == 5;
			Table1P6MenuItem.Checked = PatternView.Pal1 == 6;
			Table1P7MenuItem.Checked = PatternView.Pal1 == 7;
		}

		private void Palette_Click(object sender, EventArgs e)
		{
			if (sender == Table0P0MenuItem)
			{
				PatternView.Pal0 = 0;
			}
			else if (sender == Table0P1MenuItem)
			{
				PatternView.Pal0 = 1;
			}
			else if (sender == Table0P2MenuItem)
			{
				PatternView.Pal0 = 2;
			}
			else if (sender == Table0P3MenuItem)
			{
				PatternView.Pal0 = 3;
			}
			else if (sender == Table0P4MenuItem)
			{
				PatternView.Pal0 = 4;
			}
			else if (sender == Table0P5MenuItem)
			{
				PatternView.Pal0 = 5;
			}
			else if (sender == Table0P6MenuItem)
			{
				PatternView.Pal0 = 6;
			}
			else if (sender == Table0P7MenuItem)
			{
				PatternView.Pal0 = 7;
			}
			else if (sender == Table1P0MenuItem)
			{
				PatternView.Pal1 = 0;
			}
			else if (sender == Table1P1MenuItem)
			{
				PatternView.Pal1 = 1;
			}
			else if (sender == Table1P2MenuItem)
			{
				PatternView.Pal1 = 2;
			}
			else if (sender == Table1P3MenuItem)
			{
				PatternView.Pal1 = 3;
			}
			else if (sender == Table1P4MenuItem)
			{
				PatternView.Pal1 = 4;
			}
			else if (sender == Table1P5MenuItem)
			{
				PatternView.Pal1 = 5;
			}
			else if (sender == Table1P6MenuItem)
			{
				PatternView.Pal1 = 6;
			}
			else if (sender == Table1P7MenuItem)
			{
				PatternView.Pal1 = 7;
			}

			UpdatePaletteSelection();
		}

		#endregion

		#region Settings

		private void SettingsSubMenu_DropDownOpened(object sender, EventArgs e)
		{
			AutoLoadMenuItem.Checked = Global.Config.AutoLoadNESPPU;
			SaveWindowPositionMenuItem.Checked = Global.Config.NesPPUSettings.SaveWindowPosition;
			AlwaysOnTopMenuItem.Checked = Global.Config.NesPPUSettings.TopMost;
			FloatingWindowMenuItem.Checked = Global.Config.NesPPUSettings.FloatingWindow;
			cHRROMTileViewerToolStripMenuItem.Checked = Global.Config.NESPPUChrRomView;
		}

		private void AutoloadMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.AutoLoadNESPPU ^= true;
		}

		private void SaveWindowPositionMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.NesPPUSettings.SaveWindowPosition ^= true;
		}

		private void AlwaysOnTopMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.NesPPUSettings.TopMost ^= true;
			TopMost = Global.Config.NesPPUSettings.TopMost;
		}

		private void FloatingWindowMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.NesPPUSettings.FloatingWindow ^= true;
			RefreshFloatingWindowControl();
		}

		#endregion

		#region Context Menus

		private void PaletteRefreshMenuItem_Click(object sender, EventArgs e)
		{
			PaletteView.Refresh();
		}

		private void PatternRefreshMenuItem_Click(object sender, EventArgs e)
		{
			PatternView.Refresh();
		}

		private void SpriteRefreshMenuItem_Click(object sender, EventArgs e)
		{
			SpriteView.Refresh();
		}

		#endregion

		#endregion

		#region Dialog and Controls

		private void NesPPU_MouseClick(object sender, MouseEventArgs e)
		{
			ZoomBox.Image = new Bitmap(64, 64);
		}

		private void NesPPU_KeyDown(object sender, KeyEventArgs e)
		{
			if (ModifierKeys.HasFlag(Keys.Control) && e.KeyCode == Keys.C)
			{
				// find the control under the mouse
				var m = Cursor.Position;
				Control top = this;
				Control found;
				do
				{
					found = top.GetChildAtPoint(top.PointToClient(m));
					top = found;
				}
				while (found != null && found.HasChildren);

				if (found != null)
				{
					var meth = found.GetType().GetMethod("ScreenshotToClipboard", Type.EmptyTypes);
					if (meth != null)
					{
						meth.Invoke(found, null);
					}
					else if (found is PictureBox)
					{
						Clipboard.SetImage((found as PictureBox).Image);
					}
					else
					{
						return;
					}

					toolStripStatusLabel1.Text = found.Text + " copied to clipboard.";

					Messagetimer.Stop();
					Messagetimer.Start();
				}
			}
		}

		private void MessageTimer_Tick(object sender, EventArgs e)
		{
			Messagetimer.Stop();
			toolStripStatusLabel1.Text = "Use CTRL+C to copy the pane under the mouse to the clipboard.";
		}

		protected override void OnShown(EventArgs e)
		{
			RefreshFloatingWindowControl();
			base.OnShown(e);
		}

		private void PaletteView_MouseClick(object sender, MouseEventArgs e)
		{
			HandleDefaultImage();
		}

		private void SpriteView_MouseClick(object sender, MouseEventArgs e)
		{
			HandleDefaultImage();
		}

		private void SpriteView_MouseEnter(object sender, EventArgs e)
		{
			DetailsBox.Text = "Details - Sprites";
		}

		private void SpriteView_MouseLeave(object sender, EventArgs e)
		{
			ClearDetails();
		}
		
		private void SpriteView_MouseMove(object sender, MouseEventArgs e)
		{
			HandleSpriteViewMouseMove(e.Location);
		}

		private void HandleSpriteViewMouseMove(Point e)
		{
			if (e.X < SpriteView.ClientRectangle.Left) return;
			if (e.Y < SpriteView.ClientRectangle.Top) return;
			if (e.X >= SpriteView.ClientRectangle.Right) return;
			if (e.Y >= SpriteView.ClientRectangle.Bottom) return;

			bool is8x16 = _nes.ppu.reg_2000.obj_size_16;
			var spriteNumber = ((e.Y / 24) * 16) + (e.X / 16);
			int x = _nes.ppu.OAM[(spriteNumber * 4) + 3];
			int y = _nes.ppu.OAM[spriteNumber * 4];
			var color = _nes.ppu.OAM[(spriteNumber * 4) + 2] & 0x03;
			var attributes = _nes.ppu.OAM[(spriteNumber * 4) + 2];

			var flags = "Flags: ";
			int h = GetBit(attributes, 6);
			int v = GetBit(attributes, 7);
			int priority = GetBit(attributes, 5);
			if (h > 0)
			{
				flags += "H ";
			}

			if (v > 0)
			{
				flags += "V ";
			}

			if (priority > 0)
			{
				flags += "Behind";
			}
			else
			{
				flags += "Front";
			}

			int tile = _nes.ppu.OAM[spriteNumber * 4 + 1];
			if (is8x16)
			{
				if ((tile & 1) != 0)
					tile += 256;
				tile &= ~1;
			}

			AddressLabel.Text = "Number: " + string.Format("{0:X2}", spriteNumber);
			ValueLabel.Text = "X: " + string.Format("{0:X2}", x);
			Value2Label.Text = "Y: " + string.Format("{0:X2}", y);
			Value3Label.Text = "Tile: " + string.Format("{0:X2}", tile);
			Value4Label.Text = "Color: " + color;
			Value5Label.Text = flags;

			if (is8x16)
			{
				ZoomBox.Image = Section(
					SpriteView.sprites, new Rectangle(new Point((e.X / 8) * 8, (e.Y / 24) * 24), new Size(8, 16)), true);
			}
			else
			{
				ZoomBox.Image = Section(
					SpriteView.sprites, new Rectangle(new Point((e.X / 8) * 8, (e.Y / 8) * 8), new Size(8, 8)), false);
			}
		}

		private void PaletteView_MouseLeave(object sender, EventArgs e)
		{
			ClearDetails();
		}

		private void PaletteView_MouseEnter(object sender, EventArgs e)
		{
			DetailsBox.Text = "Details - Palettes";
		}

		private void PaletteView_MouseMove(object sender, MouseEventArgs e)
		{
			HandlePaletteViewMouseMove(e.Location);
		}

		private void HandlePaletteViewMouseMove(Point e)
		{
			if (e.X < PaletteView.ClientRectangle.Left) return;
			if (e.Y < PaletteView.ClientRectangle.Top) return;
			if (e.X >= PaletteView.ClientRectangle.Right) return;
			if (e.Y >= PaletteView.ClientRectangle.Bottom) return;

			int baseAddr = 0x3F00;
			if (e.Y > 16)
			{
				baseAddr += 16;
			}

			int column = e.X / 16;
			int addr = column + baseAddr;
			AddressLabel.Text = "Address: 0x" + string.Format("{0:X4}", addr);
			int val;

			var bmp = new Bitmap(64, 64);
			var g = Graphics.FromImage(bmp);

			if (baseAddr == 0x3F00)
			{
				val = _nes.ppu.PALRAM[PaletteView.BgPalettes[column].Address];
				ValueLabel.Text = "ID: BG" + (column / 4);
				g.FillRectangle(new SolidBrush(PaletteView.BgPalettes[column].Color), 0, 0, 64, 64);
			}
			else
			{
				val = _nes.ppu.PALRAM[PaletteView.SpritePalettes[column].Address];
				ValueLabel.Text = "ID: SPR" + (column / 4);
				g.FillRectangle(new SolidBrush(PaletteView.SpritePalettes[column].Color), 0, 0, 64, 64);
			}

			g.Dispose();

			Value3Label.Text = "Color: 0x" + string.Format("{0:X2}", val);
			Value4Label.Text = "Offset: " + (addr & 0x03);
			ZoomBox.Image = bmp;
		}

		private void PatternView_Click(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (e.X < PatternView.Width / 2)
				{
					PatternView.Pal0++;
					if (PatternView.Pal0 > 7)
					{
						PatternView.Pal0 = 0;
					}
				}
				else
				{
					PatternView.Pal1++;
					if (PatternView.Pal1 > 7)
					{
						PatternView.Pal1 = 0;
					}
				}

				UpdatePaletteSelection();
			}

			HandleDefaultImage();
		}

		private void PatternView_MouseEnter(object sender, EventArgs e)
		{
			DetailsBox.Text = "Details - Patterns";
		}

		private void PatternView_MouseLeave(object sender, EventArgs e)
		{
			ClearDetails();
		}

		private void PatternView_MouseMove(object sender, MouseEventArgs e)
		{
			int table = 0;
			int address;
			int tile;
			if (e.X > PatternView.Width / 2)
			{
				table = 1;
			}

			if (table == 0)
			{
				tile = (e.X - 1) / 8;
				address = tile * 16;
			}
			else
			{
				tile = (e.X - 128) / 8;
				address = 0x1000 + (tile * 16);
			}

			address += (e.Y / 8) * 256;
			tile += (e.Y / 8) * 16;
			var usage = "Usage: ";

			if ((_nes.ppu.reg_2000.Value & 0x10) << 4 == ((address >> 4) & 0x100))
			{
				usage = "BG";
			}
			else if (((_nes.ppu.reg_2000.Value & 0x08) << 5) == ((address >> 4) & 0x100))
			{
				usage = "SPR";
			}

			if ((_nes.ppu.reg_2000.Value & 0x20) > 0)
			{
				usage += " (SPR16)";
			}

			AddressLabel.Text = "Address: " + string.Format("{0:X4}", address);
			ValueLabel.Text = "Table " + table;
			Value3Label.Text = "Tile " + string.Format("{0:X2}", tile);
			Value4Label.Text = usage;

			ZoomBox.Image = Section(PatternView.pattern, new Rectangle(new Point((e.X / 8) * 8, (e.Y / 8) * 8), new Size(8, 8)), false);
		}

		private void ScanlineTextbox_TextChanged(object sender, EventArgs e)
		{
			int temp;
			if (int.TryParse(txtScanline.Text, out temp))
			{
				_callback.Scanline = temp;
			}
		}

		private void NesPPU_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_nes != null && _nes.ppu.PPUViewCallback == _callback)
			{
				_nes.ppu.PPUViewCallback = null;
			}
		}

		#endregion

		MemoryDomain CHRROM;
		byte[] chrromcache = new byte[8192];

		private void cHRROMTileViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Global.Config.NESPPUChrRomView ^= true;
			CalculateFormSize();
		}

		private void CalculateFormSize()
		{
			Width = Global.Config.NESPPUChrRomView ? 861 : 580;
		}

		private void CHRROMViewReload()
		{
			CHRROM = _nes.MemoryDomains["CHR VROM"];
			if (CHRROM == null)
			{
				numericUpDownCHRROMBank.Enabled = false;
				Array.Clear(chrromcache, 0, 8192);
			}
			else
			{
				numericUpDownCHRROMBank.Enabled = true;
				numericUpDownCHRROMBank.Minimum = 0;
				numericUpDownCHRROMBank.Maximum = CHRROM.Size / 8192 - 1;
				numericUpDownCHRROMBank.Value = Math.Min(numericUpDownCHRROMBank.Value, numericUpDownCHRROMBank.Maximum);
			}
			CHRROMViewRefresh();
		}

		private void CHRROMViewRefresh()
		{
			if (CHRROM != null)
			{
				int offs = 8192 * (int)numericUpDownCHRROMBank.Value;
				for (int i = 0; i < 8192; i++)
					chrromcache[i] = CHRROM.PeekByte(offs + i);

				DrawPatternView(CHRROMView, chrromcache);
			}
		}

		#endregion

		private void numericUpDownCHRROMBank_ValueChanged(object sender, EventArgs e)
		{
			CHRROMViewRefresh();
		}
	}
}
