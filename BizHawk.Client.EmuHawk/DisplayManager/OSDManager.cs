using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using BizHawk.Client.Common;

using BizHawk.Bizware.BizwareGL;

namespace BizHawk.Client.EmuHawk
{
	/// <summary>
	/// This is an old abstracted rendering class that the OSD system is using to get its work done.
	/// We should probably just use a GuiRenderer (it was designed to do that) although wrapping it with
	/// more information for OSDRendering could be helpful I suppose
	/// </summary>
	public interface IBlitter
	{

		IBlitterFont GetFontType(string fontType);
		void DrawString(string s, IBlitterFont font, Color color, float x, float y);
		SizeF MeasureString(string s, IBlitterFont font);
		Rectangle ClipBounds { get; set; }
	}

	class UIMessage
	{
		public string Message;
		public DateTime ExpireAt;
	}

	class UIDisplay
	{
		public string Message;
		public int X;
		public int Y;
		public int Anchor;
		public Color ForeColor;
		public Color BackGround;
	}

	public class OSDManager
	{
		public string FPS { get; set; }
		public string MT { get; set; }
		public IBlitterFont MessageFont;

		public void Dispose()
		{

		}

		public void Begin(IBlitter blitter)
		{
			MessageFont = blitter.GetFontType("MessageFont");
		}

		public System.Drawing.Color FixedMessagesColor { get { return System.Drawing.Color.FromArgb(Global.Config.MessagesColor); } }
		public System.Drawing.Color FixedAlertMessageColor { get { return System.Drawing.Color.FromArgb(Global.Config.AlertMessageColor); } }

		public OSDManager()
		{
		}

		private float GetX(IBlitter g, int x, int anchor, string message)
		{
			var size = g.MeasureString(message, MessageFont);

			switch (anchor)
			{
				default:
				case 0: //Top Left
				case 2: //Bottom Left
					return x;
				case 1: //Top Right
				case 3: //Bottom Right
					return g.ClipBounds.Width - x - size.Width;
			}
		}

		private float GetY(IBlitter g, int y, int anchor, string message)
		{
			var size = g.MeasureString(message, MessageFont);

			switch (anchor)
			{
				default:
				case 0: //Top Left
				case 1: //Top Right
					return y;
				case 2: //Bottom Left
				case 3: //Bottom Right
					return g.ClipBounds.Height - y - size.Height;
			}
		}

		private string MakeFrameCounter()
		{
			if (Global.MovieSession.Movie.IsFinished)
			{
				var sb = new StringBuilder();
				sb
					.Append(Global.Emulator.Frame)
					.Append('/')
					.Append(Global.MovieSession.Movie.FrameCount)
					.Append(" (Finished)");
				return sb.ToString();
			}

			if (Global.MovieSession.Movie.IsPlaying)
			{
				var sb = new StringBuilder();
				sb
					.Append(Global.Emulator.Frame)
					.Append('/')
					.Append(Global.MovieSession.Movie.FrameCount);

				return sb.ToString();
			}
			
			if (Global.MovieSession.Movie.IsRecording)
			{
				return Global.Emulator.Frame.ToString();
			}
			
			return Global.Emulator.Frame.ToString();
		}

		private List<UIMessage> messages = new List<UIMessage>(5);
		private List<UIDisplay> GUITextList = new List<UIDisplay>();

		public void AddMessage(string message)
		{
			GlobalWin.DisplayManager.NeedsToPaint = true;
			messages.Add(new UIMessage { Message = message, ExpireAt = DateTime.Now + TimeSpan.FromSeconds(2) });
		}

		public void AddGUIText(string message, int x, int y, Color backGround, Color foreColor, int anchor)
		{
			GlobalWin.DisplayManager.NeedsToPaint = true;
			GUITextList.Add(new UIDisplay
			{
				Message = message,
				X = x,
				Y = y,
				BackGround = backGround,
				ForeColor = foreColor,
				Anchor = anchor
			});
		}

		public void ClearGUIText()
		{
			GlobalWin.DisplayManager.NeedsToPaint = true;
			GUITextList.Clear();
		}

		public void DrawMessages(IBlitter g)
		{
			if (!Global.ClientControls["MaxTurbo"])
			{
				messages.RemoveAll(m => DateTime.Now > m.ExpireAt);
				int line = 1;
				if (Global.Config.StackOSDMessages)
				{
					for (int i = messages.Count - 1; i >= 0; i--, line++)
					{
						float x = GetX(g, Global.Config.DispMessagex, Global.Config.DispMessageanchor, messages[i].Message);
						float y = GetY(g, Global.Config.DispMessagey, Global.Config.DispMessageanchor, messages[i].Message);
						if (Global.Config.DispMessageanchor < 2)
						{
							y += ((line - 1) * 18);
						}
						else
						{
							y -= ((line - 1) * 18);
						}

						g.DrawString(messages[i].Message, MessageFont, Color.Black, x + 2, y + 2);
						g.DrawString(messages[i].Message, MessageFont, FixedMessagesColor, x, y);
					}
				}
				else
				{
					if (messages.Any())
					{
						int i = messages.Count - 1;

						float x = GetX(g, Global.Config.DispMessagex, Global.Config.DispMessageanchor, messages[i].Message);
						float y = GetY(g, Global.Config.DispMessagey, Global.Config.DispMessageanchor, messages[i].Message);
						if (Global.Config.DispMessageanchor < 2)
						{
							y += ((line - 1) * 18);
						}
						else
						{
							y -= ((line - 1) * 18);
						}

						g.DrawString(messages[i].Message, MessageFont, Color.Black, x + 2, y + 2);
						g.DrawString(messages[i].Message, MessageFont, FixedMessagesColor, x, y);
					}
				}

				foreach (var text in GUITextList)
				{
					try
					{
						float posx = GetX(g, text.X, text.Anchor, text.Message);
						float posy = GetY(g, text.Y, text.Anchor, text.Message);

						g.DrawString(text.Message, MessageFont, text.BackGround, posx + 2, posy + 2);
						g.DrawString(text.Message, MessageFont, text.ForeColor, posx, posy);
					}
					catch (Exception)
					{
						return;
					}
				}
			}
		}

		public string InputStrMovie()
		{
			var sb = new StringBuilder(Global.GetOutputControllersAsMnemonic());
			sb.Replace(".", " ").Replace("|", "").Replace(" 000, 000", "         ");
			return sb.ToString();
		}

		public string InputStrImmediate()
		{
			var mg = new MnemonicsGenerator();
			mg.SetSource(Global.AutofireStickyXORAdapter);

			var sb = new StringBuilder(mg.GetControllersAsMnemonic());
			sb.Replace(".", " ").Replace("|", "").Replace(" 000, 000", "         ");
			return sb.ToString();
		}

		public string InputPrevious()
		{
			if (Global.MovieSession.Movie.IsActive)
			{
				var sb = new StringBuilder(Global.MovieSession.Movie.GetInput(Global.Emulator.Frame - 1));
				sb.Replace(".", " ").Replace("|", "").Replace(" 000, 000", "         ");
				return sb.ToString();
			}

			return string.Empty;
		}

		public string InputStrOrAll()
		{
			var m = new MovieControllerAdapter { Type = Global.MovieSession.MovieControllerAdapter.Type };

			if (Global.MovieSession.Movie.IsActive)
			{
				m.SetControllersAsMnemonic(
					Global.MovieSession.Movie.GetInput(Global.Emulator.Frame - 1));
			}

			var orAdaptor = new ORAdapter()
			{
				Source = Global.AutofireStickyXORAdapter,
				SourceOr = m
			};

			var mg = new MnemonicsGenerator();
			mg.SetSource(orAdaptor);

			var sb = new StringBuilder(mg.GetControllersAsMnemonic());
			sb.Replace(".", " ").Replace("|", "").Replace(" 000, 000", "         ");
			return sb.ToString();
		}

		public string InputStrSticky()
		{
			var stickyOr = new StickyOrAdapter
			{
				Source = Global.StickyXORAdapter,
				SourceStickyOr = Global.AutofireStickyXORAdapter
			};

			var mg = new MnemonicsGenerator();
			mg.SetSource(stickyOr);
			var sb = new StringBuilder(mg.GetControllersAsMnemonic());
			sb.Replace(".", " ").Replace("|", "").Replace(" 000, 000", "         ");
			return sb.ToString();
		}

		public string MakeIntersectImmediatePrevious()
		{
			if (Global.MovieSession.Movie.IsActive)
			{
				var m = new MovieControllerAdapter { Type = Global.MovieSession.MovieControllerAdapter.Type };
				m.SetControllersAsMnemonic(
					Global.MovieSession.Movie.GetInput(Global.Emulator.Frame - 1));

				var andAdaptor = new AndAdapter
				{
					Source = Global.AutofireStickyXORAdapter,
					SourceAnd = m
				};

				var mg = new MnemonicsGenerator();
				mg.SetSource(andAdaptor);

				var sb = new StringBuilder(mg.GetControllersAsMnemonic());
				sb.Replace(".", " ").Replace("|", "").Replace(" 000, 000", "         ");
				return sb.ToString();
			}

			// TODO: track previous input even when not movie recording
			return string.Empty;
		}

		public string MakeRerecordCount()
		{
			if (Global.MovieSession.Movie.IsActive)
			{
				return "Rerecord Count: " + Global.MovieSession.Movie.Header.Rerecords;
			}
			
			return string.Empty;
		}

		private void DrawOsdMessage(IBlitter g, string message, Color color, float x, float y)
		{
			g.DrawString(message, MessageFont, Color.Black, x + 1, y + 1);
			g.DrawString(message, MessageFont, color, x, y);
		}

		/// <summary>
		/// Display all screen info objects like fps, frame counter, lag counter, and input display
		/// </summary>
		public void DrawScreenInfo(IBlitter g)
		{
			if (Global.Config.DisplayFrameCounter)
			{
				string message = MakeFrameCounter();
				float x = GetX(g, Global.Config.DispFrameCx, Global.Config.DispFrameanchor, message);
				float y = GetY(g, Global.Config.DispFrameCy, Global.Config.DispFrameanchor, message);

				DrawOsdMessage(g, message, Color.FromArgb(Global.Config.MessagesColor), x, y);

				if (Global.Emulator.IsLagFrame)
				{
					DrawOsdMessage(g, Global.Emulator.Frame.ToString(), FixedAlertMessageColor, x, y);
				}
			}

			if (Global.Config.DisplayInput)
			{
				if (Global.MovieSession.Movie.IsPlaying && !Global.MovieSession.Movie.IsFinished)
				{
					var input = InputStrMovie();
					var x = GetX(g, Global.Config.DispInpx, Global.Config.DispInpanchor, input);
					var y = GetY(g, Global.Config.DispInpy, Global.Config.DispInpanchor, input);
					Color c = Color.FromArgb(Global.Config.MovieInput);
					g.DrawString(input, MessageFont, Color.Black, x + 1, y + 1);
					g.DrawString(input, MessageFont, c, x, y);
				}

				else // TODO: message config -- allow setting of "previous", "mixed", and "auto"
				{
					var bgStr = InputStrOrAll();
					var x = GetX(g, Global.Config.DispInpx, Global.Config.DispInpanchor, bgStr);
					var y = GetY(g, Global.Config.DispInpy, Global.Config.DispInpanchor, bgStr);
					g.DrawString(bgStr, MessageFont, Color.Black, x + 1, y + 1);

					

					var previousStr = InputPrevious();
					var pColor = Color.Orange;
					g.DrawString(previousStr, MessageFont, pColor, x, y);


					var immediate = InputStrImmediate();
					Color immediateColor = Color.FromArgb(Global.Config.MessagesColor);
					g.DrawString(immediate, MessageFont, immediateColor, x, y);

					var immediateOverlay = MakeIntersectImmediatePrevious();
					var oColor = Color.PeachPuff;
					g.DrawString(immediateOverlay, MessageFont, oColor, x, y);

					var autoString = InputStrSticky();
					g.DrawString(autoString, MessageFont, Color.Pink, x, y);
				}
			}

			if (Global.MovieSession.MultiTrack.IsActive)
			{
				float x = GetX(g, Global.Config.DispMultix, Global.Config.DispMultianchor, MT);
				float y = GetY(g, Global.Config.DispMultiy, Global.Config.DispMultianchor, MT);

				DrawOsdMessage(g, MT, FixedMessagesColor, x, y);
			}

			if (Global.Config.DisplayFPS && FPS != null)
			{
				float x = GetX(g, Global.Config.DispFPSx, Global.Config.DispFPSanchor, FPS);
				float y = GetY(g, Global.Config.DispFPSy, Global.Config.DispFPSanchor, FPS);

				DrawOsdMessage(g, FPS, FixedMessagesColor, x, y);
			}

			if (Global.Config.DisplayLagCounter)
			{
				var counter = Global.Emulator.LagCount.ToString();
				var x = GetX(g, Global.Config.DispLagx, Global.Config.DispLaganchor, counter);
				var y = GetY(g, Global.Config.DispLagy, Global.Config.DispLaganchor, counter);

				DrawOsdMessage(g, counter, FixedAlertMessageColor, x, y);
			}

			if (Global.Config.DisplayRerecordCount)
			{
				string rerec = MakeRerecordCount();
				float x = GetX(g, Global.Config.DispRecx, Global.Config.DispRecanchor, rerec);
				float y = GetY(g, Global.Config.DispRecy, Global.Config.DispRecanchor, rerec);

				DrawOsdMessage(g, rerec, FixedMessagesColor, x, y);
			}

			if (Global.ClientControls["Autohold"] || Global.ClientControls["Autofire"])
			{
				var disp = new StringBuilder("Held: ");

				foreach (string sticky in Global.StickyXORAdapter.CurrentStickies)
				{
					disp.Append(sticky).Append(' ');
				}

				foreach (string autoSticky in Global.AutofireStickyXORAdapter.CurrentStickies)
				{
					disp
						.Append("Auto-")
						.Append(autoSticky)
						.Append(' ');
				}

				var message = disp.ToString();

				g.DrawString(
					message,
					MessageFont,
					Color.White,
					GetX(g, Global.Config.DispAutoholdx, Global.Config.DispAutoholdanchor, message),
					GetY(g, Global.Config.DispAutoholdy, Global.Config.DispAutoholdanchor, message));
			}

			if (Global.MovieSession.Movie.IsActive && Global.Config.DisplaySubtitles)
			{
				var subList = Global.MovieSession.Movie.Header.Subtitles.GetSubtitles(Global.Emulator.Frame);

				foreach (var sub in subList)
				{
					DrawOsdMessage(g, sub.Message, Color.FromArgb((int)sub.Color), sub.X, sub.Y);
				}
			}
		}
	}

}