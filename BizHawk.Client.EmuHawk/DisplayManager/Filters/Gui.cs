using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

using BizHawk.Common;
using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using BizHawk.Client.EmuHawk.FilterManager;

using BizHawk.Bizware.BizwareGL;
using BizHawk.Bizware.BizwareGL.Drivers.OpenTK;

using OpenTK;
using OpenTK.Graphics;

namespace BizHawk.Client.EmuHawk.Filters
{
	/// <summary>
	/// applies letterboxing logic to figure out how to fit the source dimensions into the target dimensions.
	/// In the future this could also apply rules like integer-only scaling, etc.
	/// </summary>
	class LetterboxingLogic
	{
		/// <summary>
		/// the location within the destination region of the output content (scaled and translated)
		/// </summary>
		public int vx, vy, vw, vh;

		/// <summary>
		/// the scale factor eventually used
		/// </summary>
		public float WidthScale, HeightScale;

		//do maths on the viewport and the native resolution and the user settings to get a display rectangle
		public LetterboxingLogic(bool maintainAspect, bool maintainInteger, int targetWidth, int targetHeight, int sourceWidth, int sourceHeight, Size textureSize, Size virtualSize)
		{
			int textureWidth = textureSize.Width;
			int textureHeight = textureSize.Height;
			int virtualWidth = virtualSize.Width;
			int virtualHeight = virtualSize.Height;

			//zero 02-jun-2014 - we passed these in, but ignored them. kind of weird..
			sourceWidth = (int)virtualWidth;
			sourceHeight = (int)virtualHeight;

			//this doesnt make sense
			if (!maintainAspect)
				maintainInteger = false;

			float widthScale = (float)targetWidth / sourceWidth;
			float heightScale = (float)targetHeight / sourceHeight;
			//zero 22-may-2014 - added this to combat problem where nes would default to having sidebars
			if (Global.Config.DispFixScaleInteger)
			{
				widthScale = (float)targetWidth / textureSize.Width;
				heightScale = (float)targetHeight / textureSize.Height;
			}
			
			if (maintainAspect)
			{
				if (widthScale > heightScale) widthScale = heightScale;
				if (heightScale > widthScale) heightScale = widthScale;
			}

			if (maintainInteger)
			{
				//don't distort the original texture
				float apparentWidth = sourceWidth * widthScale;
				float apparentHeight = sourceHeight * heightScale;
				widthScale = (apparentWidth / textureWidth);
				heightScale = (apparentHeight / textureHeight);

				//zero 27-may-2014 - no longer relevant, but this is sneaky, so leaving it as a reminder for now
				//prevent dysfunctional reduction to 0x
				//if (widthScale == 0) widthScale = 1;
				//if (heightScale == 0) heightScale = 1;
			
			REDO:

				apparentWidth = textureWidth * widthScale;
				apparentHeight = textureHeight * heightScale;

				//in case we've exaggerated the aspect ratio too far beyond the goal by our blunt integerizing, heres a chance to fix it by reducing one of the dimensions
				float goalAr = ((float)virtualWidth)/virtualHeight;
				float haveAr = apparentWidth / apparentHeight;
				if (widthScale>1)
				{
					//try truncating this scale, or reducing it by 1 if it's already truncated
					float floored = (float)Math.Floor(widthScale);
					float next;
					if (widthScale == floored)
						next = widthScale - 1;
					else next = floored;

					float tryAnotherAR = (next*textureWidth)/(heightScale*textureHeight);
					if(Math.Abs(tryAnotherAR-goalAr) < Math.Abs(haveAr-goalAr))
					{
						widthScale = next;
						goto REDO;
					}
				}
				if (heightScale > 1)
				{
					//try truncating this scale, or reducing it by 1 if it's already truncated
					float floored = (float)Math.Floor(heightScale);
					float next;
					if (heightScale == floored)
						next = heightScale - 1;
					else next = floored;

					float tryAnotherAR = (widthScale*textureWidth) / (next*textureHeight);
					if (Math.Abs(tryAnotherAR - goalAr) < Math.Abs(haveAr - goalAr))
					{
						heightScale = next;
						goto REDO;
					}
				}

				//we're not allowed to get out of here with non-integer scales, so if that's happened, back in for another loop we go
				if (Math.Floor(heightScale) != heightScale && heightScale >= 1)
				{
					heightScale = (float)Math.Floor(heightScale);
					goto REDO;
				}
				if (Math.Floor(widthScale) != widthScale && widthScale >= 1)
				{
					widthScale = (float)Math.Floor(widthScale);
					goto REDO;
				}
				

				vw = (int)(widthScale * textureWidth);
				vh = (int)(heightScale * textureHeight);
			}
			else
			{
				vw = (int)(widthScale * sourceWidth);
				vh = (int)(heightScale * sourceHeight);
			}


			vx = (targetWidth - vw) / 2;
			vy = (targetHeight - vh) / 2;
			WidthScale = widthScale;
			HeightScale = heightScale;
		}

	}

	public class FinalPresentation : BaseFilter
	{
		public enum eFilterOption
		{
			None, Bilinear, Bicubic
		}

		public eFilterOption FilterOption = eFilterOption.None;
		public RetroShaderChain BicubicFilter;

		public FinalPresentation(Size size)
		{
			this.OutputSize = size;
		}

		Size OutputSize, InputSize;
		public Size TextureSize, VirtualTextureSize;
		public int BackgroundColor;
		public GuiRenderer GuiRenderer;
		public IGL GL;
		bool nop;
		LetterboxingLogic LL;
		Size ContentSize;

		public override void Initialize()
		{
			DeclareInput();
			nop = false;
		}

		public override Size PresizeOutput(string channel, Size size)
		{
			if (FilterOption == eFilterOption.Bicubic)
			{
				size.Width = LL.vw;
				size.Height = LL.vh;
				return size;
			}
			return base.PresizeOutput(channel, size);
		}

		public override Size PresizeInput(string channel, Size size)
		{
			if (FilterOption != eFilterOption.Bicubic)
				return size;

			LL = new LetterboxingLogic(Global.Config.DispFixAspectRatio, Global.Config.DispFixScaleInteger, OutputSize.Width, OutputSize.Height, size.Width, size.Height, TextureSize, VirtualTextureSize);

			return size;
		}

		public override void SetInputFormat(string channel, SurfaceState state)
		{
			bool need = false;
			if (state.SurfaceFormat.Size != OutputSize)
				need = true;
			if (FilterOption != eFilterOption.None)
				need = true;

			if (!need)
			{
				nop = true;
				ContentSize = state.SurfaceFormat.Size;
				return;
			}

			FindInput().SurfaceDisposition = SurfaceDisposition.Texture;
			DeclareOutput(new SurfaceState(new SurfaceFormat(OutputSize), SurfaceDisposition.RenderTarget));
			InputSize = state.SurfaceFormat.Size;
			LL = new LetterboxingLogic(Global.Config.DispFixAspectRatio, Global.Config.DispFixScaleInteger, OutputSize.Width, OutputSize.Height, InputSize.Width, InputSize.Height, TextureSize, VirtualTextureSize);
			ContentSize = new Size(LL.vw,LL.vh);
		}

		public Size GetContentSize() { return ContentSize; }

		public override Vector2 UntransformPoint(string channel, Vector2 point)
		{
			if (nop)
				return point;
			point.X -= LL.vx;
			point.Y -= LL.vy;
			point.X /= LL.WidthScale;
			point.Y /= LL.HeightScale;
			return point;
		}

		public override void Run()
		{
			if (nop)
				return;

			GL.SetClearColor(Color.FromArgb(BackgroundColor));
			GL.Clear(OpenTK.Graphics.OpenGL.ClearBufferMask.ColorBufferBit);

			GuiRenderer.Begin(OutputSize.Width, OutputSize.Height);
			GuiRenderer.SetBlendState(GL.BlendNone);
			GuiRenderer.Modelview.Push();
			//GuiRenderer.Modelview.Translate(LL.vx, LL.vy);
			//GuiRenderer.Modelview.Scale(LL.WidthScale, LL.HeightScale);
			if(FilterOption != eFilterOption.None)
				InputTexture.SetFilterLinear();
			else
				InputTexture.SetFilterNearest();

			if (FilterOption == eFilterOption.Bicubic)
			{
			}


			GuiRenderer.Draw(InputTexture,LL.vx,LL.vy,LL.vw,LL.vh);

			GuiRenderer.End();
		}
	}

	public class LuaLayer : BaseFilter
	{
		public override void Initialize()
		{
			DeclareInput(SurfaceDisposition.RenderTarget);
		}
		public override void SetInputFormat(string channel, SurfaceState state)
		{
			DeclareOutput(state);
		}

		Texture2d Texture;

		public void SetTexture(Texture2d tex)
		{
			Texture = tex;
		}

		public override void Run()
		{
			var outSize = FindOutput().SurfaceFormat.Size;
			FilterProgram.GuiRenderer.Begin(outSize);
			FilterProgram.GuiRenderer.SetBlendState(FilterProgram.GL.BlendNormal);
			FilterProgram.GuiRenderer.Draw(Texture);
			FilterProgram.GuiRenderer.End();
		}
	}

	public class OSD : BaseFilter
	{
		//this class has the ability to disable its operations for higher performance when the callback is removed,
		//without having to take it out of the chain. although, its presence in the chain may slow down performance due to added resolves/renders
		//so, we should probably rebuild the chain.

		public override void Initialize()
		{
			if (RenderCallback == null) return;
			DeclareInput(SurfaceDisposition.RenderTarget);
		}
		public override void SetInputFormat(string channel, SurfaceState state)
		{
			if (RenderCallback == null) return;
			DeclareOutput(state);
		}

		public Action RenderCallback;

		public override void Run()
		{
			if (RenderCallback == null) return;
			RenderCallback();
		}
	}
}