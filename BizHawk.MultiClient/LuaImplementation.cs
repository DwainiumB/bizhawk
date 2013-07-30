﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using LuaInterface;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Globalization;

using BizHawk.Emulation.Consoles.Nintendo;
using BizHawk.MultiClient.tools;
using System.Text;

namespace BizHawk.MultiClient
{
	public class LuaImplementation
	{
		public LuaDocumentation docs = new LuaDocumentation();
		public bool isRunning;
		public EventWaitHandle LuaWait;
		public bool FrameAdvanceRequested;

		private Lua _lua = new Lua();
		private readonly LuaConsole Caller;
		private int CurrentMemoryDomain; //Main memory by default
		private Lua currThread;
		private LuaFunction savestate_registersavefunc;
		private LuaFunction savestate_registerloadfunc;
		private LuaFunction frame_startfunc;
		private LuaFunction frame_endfunc;
		private readonly Dictionary<Color, SolidBrush> SolidBrushes = new Dictionary<Color, SolidBrush>();
		private readonly Dictionary<Color, Pen> Pens = new Dictionary<Color, Pen>();

		public void SavestateRegisterSave(string name)
		{
			if (savestate_registersavefunc != null)
			{
				try
				{
					savestate_registersavefunc.Call(name);
				}
				catch (SystemException e)
				{
					Global.MainForm.LuaConsole1.WriteToOutputWindow(
						"error running function attached by lua function savestate.registersave" +
						"\nError message: " + e.Message);
				}
			}
		}

		public void SavestateRegisterLoad(string name)
		{
			if (savestate_registerloadfunc != null)
			{
				try
				{
					savestate_registerloadfunc.Call(name);
				}
				catch (SystemException e)
				{
					Global.MainForm.LuaConsole1.WriteToOutputWindow(
						"error running function attached by lua function savestate.registerload" +
						"\nError message: " + e.Message);
				}
			}
		}

		public void FrameRegisterBefore()
		{
			if (frame_startfunc != null)
			{
				try
				{
					frame_startfunc.Call();
				}
				catch (SystemException e)
				{
					Global.MainForm.LuaConsole1.WriteToOutputWindow(
						"error running function attached by lua function emu.registerbefore" +
						"\nError message: " + e.Message);
				}
			}
		}

		public void FrameRegisterAfter()
		{
			if (frame_endfunc != null)
			{
				try
				{
					frame_endfunc.Call();
				}
				catch (SystemException e)
				{
					Global.MainForm.LuaConsole1.WriteToOutputWindow(
						"error running function attached by lua function emu.registerafter" +
						"\nError message: " + e.Message);
				}
			}
		}

		public LuaImplementation(LuaConsole passed)
		{
			LuaWait = new AutoResetEvent(false);
			docs.Clear();
			Caller = passed.get();
			LuaRegister(_lua);
		}

		public void Close()
		{
			_lua = new Lua();
			foreach (var brush in SolidBrushes.Values) brush.Dispose();
			foreach (var brush in Pens.Values) brush.Dispose();
		}

		public void LuaRegister(Lua lua)
		{
			lua.RegisterFunction("print", this, GetType().GetMethod("print"));

			//Register libraries
			lua.NewTable("console");
			foreach (string t in ConsoleFunctions)
			{
				lua.RegisterFunction("console." + t, this,
				                     GetType().GetMethod("console_" + t));
				docs.Add("console", t, GetType().GetMethod("console_" + t));
			}

			lua.NewTable("gui");
			foreach (string t in GuiFunctions)
			{
				lua.RegisterFunction("gui." + t, this, GetType().GetMethod("gui_" + t));
				docs.Add("gui", t, GetType().GetMethod("gui_" + t));
			}

			lua.NewTable("emu");
			foreach (string t in EmuFunctions)
			{
				lua.RegisterFunction("emu." + t, this, GetType().GetMethod("emu_" + t));
				docs.Add("emu", t, GetType().GetMethod("emu_" + t));
			}

			lua.NewTable("memory");
			foreach (string t in MemoryFunctions)
			{
				lua.RegisterFunction("memory." + t, this, GetType().GetMethod("memory_" + t));
				docs.Add("memory", t, GetType().GetMethod("memory_" + t));
			}

			lua.NewTable("mainmemory");
			foreach (string t in MainMemoryFunctions)
			{
				lua.RegisterFunction("mainmemory." + t, this,
				                     GetType().GetMethod("mainmemory_" + t));
				docs.Add("mainmemory", t, GetType().GetMethod("mainmemory_" + t));
			}

			lua.NewTable("savestate");
			foreach (string t in SaveStateFunctions)
			{
				lua.RegisterFunction("savestate." + t, this,
				                     GetType().GetMethod("savestate_" + t));
				docs.Add("savestate", t, GetType().GetMethod("savestate_" + t));
			}

			lua.NewTable("movie");
			foreach (string t in MovieFunctions)
			{
				lua.RegisterFunction("movie." + t, this, GetType().GetMethod("movie_" + t));
				docs.Add("movie", t, GetType().GetMethod("movie_" + t));
			}

			lua.NewTable("input");
			foreach (string t in InputFunctions)
			{
				lua.RegisterFunction("input." + t, this, GetType().GetMethod("input_" + t));
				docs.Add("input", t, GetType().GetMethod("input_" + t));
			}

			lua.NewTable("joypad");
			foreach (string t in JoypadFunctions)
			{
				lua.RegisterFunction("joypad." + t, this, GetType().GetMethod("joypad_" + t));
				docs.Add("joypad", t, GetType().GetMethod("joypad_" + t));
			}

			lua.NewTable("client");
			foreach (string t in MultiClientFunctions)
			{
				lua.RegisterFunction("client." + t, this,
				                     GetType().GetMethod("client_" + t));
				docs.Add("client", t, GetType().GetMethod("client_" + t));
			}

			lua.NewTable("forms");
			foreach (string t in FormsFunctions)
			{
				lua.RegisterFunction("forms." + t, this, GetType().GetMethod("forms_" + t));
				docs.Add("forms", t, GetType().GetMethod("forms_" + t));
			}

			lua.NewTable("bit");
			foreach (string t in BitwiseFunctions)
			{
				lua.RegisterFunction("bit." + t, this, GetType().GetMethod("bit_" + t));
				docs.Add("bit", t, GetType().GetMethod("bit_" + t));
			}

			lua.NewTable("nes");
			foreach (string t in NESFunctions)
			{
				lua.RegisterFunction("nes." + t, this, GetType().GetMethod("nes_" + t));
				docs.Add("nes", t, GetType().GetMethod("nes_" + t));
			}

			lua.NewTable("snes");
			foreach (string t in SNESFunctions)
			{
				lua.RegisterFunction("snes." + t, this, GetType().GetMethod("snes_" + t));
				docs.Add("snes", t, GetType().GetMethod("snes_" + t));
			}

			lua.NewTable("event");
			foreach (string t in EventFunctions)
			{
				lua.RegisterFunction("event." + t, this, GetType().GetMethod("event_" + t));
				docs.Add("event", t, GetType().GetMethod("event_" + t));
			}

			docs.Sort();
		}

		public Lua SpawnCoroutine(string File)
		{
			var t = _lua.NewThread();
			//LuaRegister(t); //adelikat: Not sure why this was here but it was causing the entire luaimplmeentaiton to be duplicated each time, eventually resulting in crashes
			var main = t.LoadFile(File);
			t.Push(main); //push main function on to stack for subsequent resuming
			return t;
		}

		private int LuaInt(object lua_arg)
		{
			return Convert.ToInt32((double)lua_arg);
		}

		private uint LuaUInt(object lua_arg)
		{
			return Convert.ToUInt32((double)lua_arg);
		}


		public Color GetColor(object color)
		{
			if (color is double)
			{
				return Color.FromArgb(int.Parse(long.Parse(color.ToString()).ToString("X"), NumberStyles.HexNumber));
			}
			else
			{
				return Color.FromName(color.ToString().ToLower());
			}
		}

		public SolidBrush GetBrush(object color)
		{
			Color c = GetColor(color);
			SolidBrush b;
			if (!SolidBrushes.TryGetValue(c, out b))
			{
				b = new SolidBrush(c);
				SolidBrushes[c] = b;
			}
			return b;
		}

		public Pen GetPen(object color)
		{
			Color c = GetColor(color);
			Pen p;
			if (!Pens.TryGetValue(c, out p))
			{
				p = new Pen(c);
				Pens[c] = p;
			}
			return p;
		}


		/**
		 * LuaInterface requires the exact match of parameter count,
		 * except optional parameters. So, if you want to support
		 * variable arguments, declare them as optional and pass
		 * them to this method.
		 */

		private object[] LuaVarArgs(params object[] lua_args)
		{
			int n = lua_args.Length;
			int trim = 0;
			for (int i = n - 1; i >= 0; --i)
				if (lua_args[i] == null) ++trim;
			object[] lua_result = new object[n - trim];
			Array.Copy(lua_args, lua_result, n - trim);
			return lua_result;
		}

		public class ResumeResult
		{
			public bool WaitForFrame;
			public bool Terminated;
		}

		public ResumeResult ResumeScript(Lua script)
		{
			currThread = script;
			int execResult = script.Resume(0);
			currThread = null;
			var result = new ResumeResult();
			if (execResult == 0)
			{
				//terminated
				result.Terminated = true;
			}
			else
			{
				//yielded
				result.WaitForFrame = FrameAdvanceRequested;
			}
			FrameAdvanceRequested = false;
			return result;
		}

		public void print(string s)
		{
			Caller.AddText(s);
			Caller.AddText("\n");
		}

		/****************************************************/
		/*************library definitions********************/
		/****************************************************/

		public static string[] ConsoleFunctions = new[]
		                                          	{
		                                          		"output",
		                                          		"log",
		                                          		"clear",
		                                          		"getluafunctionslist"
		                                          	};

		public static string[] GuiFunctions = new[]
		                                      	{
		                                      		"text",
		                                      		"alert",
		                                      		"cleartext",
		                                      		"drawPixel",
		                                      		"drawLine",
													"drawBox",
		                                      		"drawRectangle",
		                                      		"drawEllipse",
		                                      		"drawPolygon",
		                                      		"drawBezier",
		                                      		"drawPie",
		                                      		"drawIcon",
		                                      		"drawImage",
													"addmessage",
													"drawText",
													"drawString"
		                                      	};

		public static string[] EmuFunctions = new[]
		                                      	{
		                                      		"frameadvance",
		                                      		"yield",
		                                      		"pause",
		                                      		"unpause",
		                                      		"togglepause",
		                                      		"ispaused",
		                                      		"speedmode",
		                                      		"framecount",
		                                      		"lagcount",
		                                      		"islagged",
		                                      		"getsystemid",
		                                      		"setrenderplanes",
		                                      		"frameskip",
		                                      		"minimizeframeskip",
		                                      		"limitframerate",
		                                      		"displayvsync",
		                                      		"enablerewind",
													"on_snoop"
		                                      	};

		public static string[] MemoryFunctions = new[]
		                                         	{
		                                         		"usememorydomain",
		                                         		"getmemorydomainlist",
		                                         		"getcurrentmemorydomain",
																								"getcurrentmemorydomainsize",
		                                         		"read_s8",
		                                         		"read_u8",
		                                         		"read_s16_le",
		                                         		"read_s24_le",
		                                         		"read_s32_le",
		                                         		"read_u16_le",
		                                         		"read_u24_le",
		                                         		"read_u32_le",
		                                         		"read_s16_be",
		                                         		"read_s24_be",
		                                         		"read_s32_be",
		                                         		"read_u16_be",
		                                         		"read_u24_be",
		                                         		"read_u32_be",
		                                         		"write_s8",
		                                         		"write_u8",
		                                         		"write_s16_le",
		                                         		"write_s24_le",
		                                         		"write_s32_le",
		                                         		"write_u16_le",
		                                         		"write_u24_le",
		                                         		"write_u32_le",
		                                         		"write_s16_be",
		                                         		"write_s24_be",
		                                         		"write_s32_be",
		                                         		"write_u16_be",
		                                         		"write_u24_be",
		                                         		"write_u32_be",
		                                         		"readbyte",
		                                         		"writebyte"
		                                         		//"registerwrite",
		                                         		//"registerread",
		                                         	};

		public static string[] MainMemoryFunctions = new[]
		                                             	{
		                                             		"read_s8",
		                                             		"read_u8",
		                                             		"read_s16_le",
		                                             		"read_s24_le",
		                                             		"read_s32_le",
		                                             		"read_u16_le",
		                                             		"read_u24_le",
		                                             		"read_u32_le",
		                                             		"read_s16_be",
		                                             		"read_s24_be",
		                                             		"read_s32_be",
		                                             		"read_u16_be",
		                                             		"read_u24_be",
		                                             		"read_u32_be",
		                                             		"write_s8",
		                                             		"write_u8",
		                                             		"write_s16_le",
		                                             		"write_s24_le",
		                                             		"write_s32_le",
		                                             		"write_u16_le",
		                                             		"write_u24_le",
		                                             		"write_u32_le",
		                                             		"write_s16_be",
		                                             		"write_s24_be",
		                                             		"write_s32_be",
		                                             		"write_u16_be",
		                                             		"write_u24_be",
		                                             		"write_u32_be",
															"readbyterange",
															"writebyterange"
		                                             	};

		public static string[] SaveStateFunctions = new[]
		                                            	{
		                                            		"saveslot",
		                                            		"loadslot",
		                                            		"save",
		                                            		"load",
		                                            		"registersave",
		                                            		"registerload"
		                                            	};

		public static string[] MovieFunctions = new[]
		                                        	{
		                                        		"mode",
		                                        		"isloaded",
		                                        		"rerecordcount",
		                                        		"length",
		                                        		"stop",
		                                        		"filename",
		                                        		"getreadonly",
		                                        		"setreadonly",
		                                        		"getrerecordcounting",
		                                        		"setrerecordcounting",
		                                        		"getinput"
		                                        	};

		public static string[] InputFunctions = new[]
		                                        	{
		                                        		"get",
		                                        		"getmouse"
		                                        	};

		public static string[] JoypadFunctions = new[]
		                                         	{
		                                         		"set",
		                                         		"get",
		                                         		"getimmediate",
														"setanalog"
		                                         	};

		public static string[] MultiClientFunctions = new[]
		                                              	{
		                                              		"getwindowsize",
															"setwindowsize",
		                                              		"openrom",
		                                              		"closerom",
		                                              		"opentoolbox",
		                                              		"openramwatch",
		                                              		"openramsearch",
		                                              		"openrampoke",
		                                              		"openhexeditor",
		                                              		"opentasstudio",
		                                              		"opencheats",
															"screenwidth",
                                                            "xpos",
															"screenheight",
                                                            "ypos",
															"screenshot",
															"screenshottoclipboard",
															"setscreenshotosd",
															"pause_av",
															"unpause_av"
		                                              	};

		public static string[] FormsFunctions = new[]
		                                        	{
		                                        		"newform",
		                                        		"destroy",
		                                        		"destroyall",
		                                        		"button",
		                                        		"label",
		                                        		"textbox",
		                                        		"setlocation",
		                                        		"setsize",
		                                        		"settext",
		                                        		"addclick",
		                                        		"clearclicks",
		                                        		"gettext",
														"setproperty",
														"getproperty",
														"openfile"
		                                        	};

		public static string[] BitwiseFunctions = new[]
		                                          	{
		                                          		"band",
		                                          		"lshift",
		                                          		"rshift",
		                                          		"rol",
		                                          		"ror",
		                                          		"bor",
		                                          		"bxor",
		                                          		"bnot"
		                                          	};

		public static string[] NESFunctions = new[]
		                                          	{
		                                          		"setscanlines",
														"gettopscanline",
														"getbottomscanline",
														"getclipleftandright",
														"setclipleftandright",
														"getdispbackground",
														"setdispbackground",
														"getdispsprites",
														"setdispsprites",
														"getallowmorethaneightsprites",
														"setallowmorethaneightsprites",
														"addgamegenie",
														"removegamegenie"
		                                          	};

		public static string[] SNESFunctions = new[]
													{
														"setlayer_bg_1",
														"setlayer_bg_2",
														"setlayer_bg_3",
														"setlayer_bg_4",
														"getlayer_bg_1",
														"getlayer_bg_2",
														"getlayer_bg_3",
														"getlayer_bg_4",

														"setlayer_obj_1",
														"setlayer_obj_2",
														"setlayer_obj_3",
														"setlayer_obj_4",
														"getlayer_obj_1",
														"getlayer_obj_2",
														"getlayer_obj_3",
														"getlayer_obj_4"
		                                          	};

		public static string[] EventFunctions = new[]
													{
														"onloadstate",
														"onsavestate",
														"onframestart",
														"onframeend",
														"onmemoryread",
														"onmemorywrite",
														"oninputpoll"
													};
		/****************************************************/
		/*************function definitions********************/
		/****************************************************/

		//----------------------------------------------------
		//Console library
		//----------------------------------------------------

		public void console_output(object lua_input)
		{
			if (lua_input == null)
			{
				Global.MainForm.LuaConsole1.WriteToOutputWindow("NULL");
			}
			else
			{
                if (lua_input is LuaTable)
                {
                    StringBuilder sb = new StringBuilder();
                    var lti = (lua_input as LuaTable);

                    List<string> Keys = new List<string>();
                    List<string> Values = new List<string>();
                    foreach (var key in lti.Keys) { Keys.Add(key.ToString()); }
                    foreach (var value in lti.Values) { Values.Add(value.ToString()); }

                    List<KeyValuePair<string, string>> KVPs = new List<KeyValuePair<string, string>>();
                    for (int i = 0; i < Keys.Count; i++)
                    {
                        if (i < Values.Count)
                        {
                            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(Keys[i], Values[i]);
                            KVPs.Add(kvp);
                        }
                    }
                    KVPs = KVPs.OrderBy(x => x.Key).ToList();
                    foreach(var kvp in KVPs)
                    {
                        sb
                            .Append("\"")
                            .Append(kvp.Key)
                            .Append("\": \"")
                            .Append(kvp.Value)
                            .Append("\"")
                            .AppendLine();
                    }

                    Global.MainForm.LuaConsole1.WriteToOutputWindow(sb.ToString());
                }
                else
                {
                    Global.MainForm.LuaConsole1.WriteToOutputWindow(lua_input.ToString());
                }
			}
		}

		public void console_log(object lua_input)
		{
			console_output(lua_input);
		}

		public void console_clear()
		{
			Global.MainForm.LuaConsole1.ClearOutputWindow();
		}

		public string console_getluafunctionslist()
		{
			string list = "";
			foreach (LuaDocumentation.LibraryFunction l in Global.MainForm.LuaConsole1.LuaImp.docs.FunctionList)
			{
				list += l.name + "\n";
			}

			return list;
		}
		//----------------------------------------------------
		//Gui library
		//----------------------------------------------------
		private void do_gui_text(object luaX, object luaY, object luaStr, bool alert, object background = null,
								 object forecolor = null, object anchor = null)
		{
			if (!alert)
			{
				if (forecolor == null)
					forecolor = "white";
				if (background == null)
					background = "black";
			}
			int dx = LuaInt(luaX);
			int dy = LuaInt(luaY);
			int a = 0;
			if (anchor != null)
			{
				int x;
				if (int.TryParse(anchor.ToString(), out x) == false)
				{
					if (anchor.ToString().ToLower() == "topleft")
						a = 0;
					else if (anchor.ToString().ToLower() == "topright")
						a = 1;
					else if (anchor.ToString().ToLower() == "bottomleft")
						a = 2;
					else if (anchor.ToString().ToLower() == "bottomright")
						a = 3;
				}
				else
				{
					a = LuaInt(anchor);
				}
			}
			else
			{
				dx -= Global.Emulator.CoreComm.ScreenLogicalOffsetX;
				dy -= Global.Emulator.CoreComm.ScreenLogicalOffsetY;
			}
			// blah hacks
			dx *= client_getwindowsize();
			dy *= client_getwindowsize();

			Global.OSD.AddGUIText(luaStr.ToString(), dx, dy, alert, GetColor(background), GetColor(forecolor),  a);
		}

		public void gui_text(object luaX, object luaY, object luaStr, object background = null, object forecolor = null,
							 object anchor = null)
		{
			do_gui_text(luaX, luaY, luaStr, false, background, forecolor, anchor);
		}

		public void gui_alert(object luaX, object luaY, object luaStr, object anchor = null)
		{
			do_gui_text(luaX, luaY, luaStr, true, null, null, anchor);
		}

		public void gui_cleartext()
		{
			Global.OSD.ClearGUIText();
		}

		public void gui_addmessage(object luaStr)
		{
			Global.OSD.AddMessage(luaStr.ToString());
		}

		public DisplaySurface luaSurface;

		/// <summary>
		/// sets the current drawing context to a new surface.
		/// you COULD pass these back to lua to use as a target in rendering jobs, instead of setting it as current here.
		/// could be more powerful.
		/// performance test may reveal that repeatedly calling GetGraphics could be slow.
		/// we may want to make a class here in LuaImplementation that wraps a DisplaySurface and a Graphics which would be created once
		/// </summary>
		public void gui_drawNew()
		{
			luaSurface = Global.DisplayManager.GetLuaSurfaceNative();
		}

		public void gui_drawNewEmu()
		{
			luaSurface = Global.DisplayManager.GetLuaEmuSurfaceEmu();
		}

		/// <summary>
		/// finishes the current drawing and submits it to the display manager (at native [host] resolution pre-osd)
		/// you would probably want some way to specify which surface to set it to, when there are other surfaces.
		/// most notably, the client output [emulated] resolution 
		/// </summary>
		public void gui_drawFinish()
		{
			Global.DisplayManager.SetLuaSurfaceNativePreOSD(luaSurface);
			luaSurface = null;
		}

		public void gui_drawFinishEmu()
		{
			Global.DisplayManager.SetLuaSurfaceEmu(luaSurface);
			luaSurface = null;
		}

		/// <summary>
		/// draws a random rectangle for testing purposes
		/// </summary>
		public void gui_drawRectangle(object X, object Y, object width, object height, object line, object background = null)
		{
			using (var g = GetGraphics())
			{
				try
				{
					int int_x = LuaInt(X);
					int int_y = LuaInt(Y);
					int int_width = LuaInt(width);
					int int_height = LuaInt(height);
					g.DrawRectangle(GetPen(line ?? "white"), int_x, int_y, int_width, int_height);
					if (background != null)
					{
						g.FillRectangle(GetBrush(background), int_x, int_y, int_width, int_height);
					}
				}
				catch (Exception)
				{
					// need to stop the script from here
					return;
				}
			}
		}

		public void gui_drawBox(object X, object Y, object X2, object Y2, object line = null, object background = null)
		{
			using (var g = GetGraphics())
			{
				try
				{
					int int_x = LuaInt(X);
					int int_y = LuaInt(Y);
					int int_width = LuaInt(X2);
					int int_height = LuaInt(Y2);

					if (int_x < int_width)
					{
						int_width = Math.Abs(int_x - int_width);
					}
					else
					{
						int_width = int_x - int_width;
						int_x -= int_width;
					}

					if (int_y < int_height)
					{
						int_height = Math.Abs(int_y - int_height);
					}
					else
					{
						int_height = int_y - int_height;
						int_y -= int_height;
					}

					g.DrawRectangle(GetPen(line ?? "white"), int_x, int_y, int_width, int_height);
					if (background != null)
					{
						g.FillRectangle(GetBrush(background), int_x, int_y, int_width, int_height);
					}
				}
				catch (Exception)
				{
					// need to stop the script from here
					return;
				}
			}
		}

		public void gui_drawString(object X, object Y, object message, object color = null, object fontsize = null, object fontfamily = null, object fontstyle = null)
		{
			gui_drawText(X, Y, message, color, fontsize, fontfamily, fontstyle);
		}

		public void gui_drawText(object X, object Y, object message, object color = null, object fontsize = null, object fontfamily = null, object fontstyle = null)
		{
			using (var g = GetGraphics())
			{
				try
				{
					int fsize = 12;
					if (fontsize != null)
					{
						fsize = LuaInt(fontsize);
					}

					FontFamily family = FontFamily.GenericMonospace;
					if (fontfamily != null)
					{
						family = new FontFamily(fontfamily.ToString());
					}

					FontStyle fstyle = FontStyle.Regular;
					if (fontstyle != null)
					{
						string tmp = fontstyle.ToString().ToLower();
						switch (tmp)
						{
							default:
							case "regular":
								break;
							case "bold":
								fstyle = FontStyle.Bold;
								break;
							case "italic":
								fstyle = FontStyle.Italic;
								break;
							case "strikethrough":
								fstyle = FontStyle.Strikeout;
								break;
							case "underline":
								fstyle = FontStyle.Underline;
								break;
						}
					}

					Font font = new Font(family, fsize, fstyle, GraphicsUnit.Pixel);
					g.DrawString(message.ToString(), font, GetBrush(color ?? "white"), LuaInt(X), LuaInt(Y));
				}
				catch (Exception)
				{
					return;
				}
			}
		}

		public void gui_drawPixel(object X, object Y, object color = null)
		{
			using (var g = GetGraphics())
			{
				float x = LuaInt(X) + 0.1F;
				try
				{
					g.DrawLine(GetPen(color ?? "white"), LuaInt(X), LuaInt(Y), x, LuaInt(Y));
				}
				catch (Exception)
				{
					return;
				}
			}
		}

		public void gui_drawLine(object x1, object y1, object x2, object y2, object color = null)
		{
			using (var g = GetGraphics())
			{
				try
				{
					g.DrawLine(GetPen(color ?? "white"), LuaInt(x1), LuaInt(y1), LuaInt(x2), LuaInt(y2));
				}
				catch (Exception)
				{
					return;
				}
			}
		}

		public void gui_drawEllipse(object X, object Y, object width, object height, object line, object background = null)
		{
			using (var g = GetGraphics())
			{
				try
				{
					g.DrawEllipse(GetPen(line ?? "white"), LuaInt(X), LuaInt(Y), LuaInt(width), LuaInt(height));
					if (background != null)
					{
						var brush = GetBrush(background);
						g.FillEllipse(brush, LuaInt(X), LuaInt(Y), LuaInt(width), LuaInt(height));
					}
				}
				catch (Exception)
				{
					// need to stop the script from here
					return;
				}
			}
		}

		public void gui_drawPolygon(LuaTable points, object line, object background = null)
		{
			//this is a test
			using (var g = GetGraphics())
			{
				try
				{
					Point[] Points = new Point[points.Values.Count];
					int i = 0;
					foreach (LuaTable point in points.Values)
					{
						Points[i] = new Point(LuaInt(point[1]), LuaInt(point[2]));
						i++;
					}

					g.DrawPolygon(GetPen(line), Points);
					if (background != null)
					{
						var brush = GetBrush(background);
						g.FillPolygon(brush, Points);
					}
				}
				catch (Exception)
				{
					return;
				}
			}
		}

		Graphics GetGraphics()
		{
			var g = luaSurface.GetGraphics();
			int tx = Global.Emulator.CoreComm.ScreenLogicalOffsetX;
			int ty = Global.Emulator.CoreComm.ScreenLogicalOffsetY;
			if (tx != 0 || ty != 0)
			{
				var transform = g.Transform;
				transform.Translate(-tx, -ty);
				g.Transform = transform;
			}
			return g;
		}

		public void gui_drawBezier(LuaTable points, object color)
		{
			using (var g = GetGraphics())
			{
				try
				{
					Point[] Points = new Point[4];
					int i = 0;
					foreach (LuaTable point in points.Values)
					{
						Points[i] = new Point(LuaInt(point[1]), LuaInt(point[2]));
						i++;
						if (i >= 4)
							break;
					}

					g.DrawBezier(GetPen(color), Points[0], Points[1], Points[2], Points[3]);
				}
				catch (Exception)
				{
					return;
				}
			}
		}

		public void gui_drawPie(object X, object Y, object width, object height, object startangle, object sweepangle,
								object line, object background = null)
		{
			using (var g = GetGraphics())
			{
				try
				{
					g.DrawPie(GetPen(line), LuaInt(X), LuaInt(Y), LuaInt(width), LuaInt(height), LuaInt(startangle), LuaInt(sweepangle));
					if (background != null)
					{
						var brush = GetBrush(background);
						g.FillPie(brush, LuaInt(X), LuaInt(Y), LuaInt(width), LuaInt(height), LuaInt(startangle), LuaInt(sweepangle));
					}
				}
				catch (Exception)
				{
					// need to stop the script from here
					return;
				}
			}
		}

		public void gui_drawIcon(object Path, object x, object y, object width = null, object height = null)
		{
			using (var g = GetGraphics())
			{
				try
				{
					Icon icon;
					if (width != null && height != null)
					{
						icon = new Icon(Path.ToString(), LuaInt(width), LuaInt(height));
					}
					else
					{
						icon = new Icon(Path.ToString());
					}

					g.DrawIcon(icon, LuaInt(x), LuaInt(y));
				}
				catch (Exception)
				{
					return;
				}
			}
		}

		public void gui_drawImage(object Path, object x, object y, object width = null, object height = null)
		{
			using (var g = GetGraphics())
			{
				try
				{
					Image img = Image.FromFile(Path.ToString());

					if (width == null || width.GetType() != typeof(int))
						width = img.Width.ToString();
					if (height == null || height.GetType() != typeof(int))
						height = img.Height.ToString();

					g.DrawImage(img, LuaInt(x), LuaInt(y), int.Parse(width.ToString()), int.Parse(height.ToString()));
				}
				catch (Exception)
				{
					return;
				}
			}
		}

		public void gui_clearGraphics()
		{
			luaSurface.Clear();
		}

		//----------------------------------------------------
		//Emu library
		//----------------------------------------------------
		public void emu_frameadvance()
		{
			FrameAdvanceRequested = true;
			currThread.Yield(0);
		}

		public void emu_yield()
		{
			currThread.Yield(0);
		}

		public void emu_pause()
		{
			Global.MainForm.PauseEmulator();
		}

		public void emu_unpause()
		{
			Global.MainForm.UnpauseEmulator();
		}

		public void emu_togglepause()
		{
			Global.MainForm.TogglePause();
		}

		public bool emu_ispaused()
		{
			return Global.MainForm.EmulatorPaused;
		}

		public int emu_framecount()
		{
			return Global.Emulator.Frame;
		}

		public int emu_lagcount()
		{
			return Global.Emulator.LagCount;
		}

		public bool emu_islagged()
		{
			return Global.Emulator.IsLagFrame;
		}

		public string emu_getsystemid()
		{
			return Global.Emulator.SystemId;
		}

		public void emu_speedmode(object percent)
		{
			try
			{
				string temp = percent.ToString();
				int speed = Convert.ToInt32(temp);
				if (speed > 0 && speed < 1000) //arbituarily capping it at 1000%
				{
					Global.MainForm.ClickSpeedItem(speed);
				}
				else
				{
					console_log("Invalid speed value");
				}
			}
			catch
			{
				console_log("Invalid speed value");
			}
		}

		public void emu_minimizeframeskip(object boolean)
		{
			string temp = boolean.ToString();
			if (!String.IsNullOrWhiteSpace(temp))
			{
				if (temp == "0" || temp.ToLower() == "false")
				{
					Global.Config.AutoMinimizeSkipping = false;
				}
				else
				{
					Global.Config.AutoMinimizeSkipping = true;
				}
				Global.MainForm.MinimizeFrameskipMessage();
			}
		}

		public void emu_limitframerate(object boolean)
		{
			string temp = boolean.ToString();
			if (!String.IsNullOrWhiteSpace(temp))
			{
				if (temp == "0" || temp.ToLower() == "false")
				{
					Global.Config.ClockThrottle = false;
				}
				else
				{
					Global.Config.ClockThrottle = true;
				}
				Global.MainForm.LimitFrameRateMessage();
			}
		}

		public void emu_displayvsync(object boolean)
		{
			string temp = boolean.ToString();
			if (!String.IsNullOrWhiteSpace(temp))
			{
				if (temp == "0" || temp.ToLower() == "false")
				{
					Global.Config.VSyncThrottle = false;
				}
				else
				{
					Global.Config.VSyncThrottle = true;
				}
				Global.MainForm.VsyncMessage();
			}
		}

		public void emu_enablerewind(object boolean)
		{
			string temp = boolean.ToString();
			if (!String.IsNullOrWhiteSpace(temp))
			{
				if (temp == "0" || temp.ToLower() == "false")
				{
					Global.MainForm.RewindActive = false;
					Global.OSD.AddMessage("Rewind suspended");
				}
				else
				{
					Global.MainForm.RewindActive = true;
					Global.OSD.AddMessage("Rewind enabled");
				}
			}
		}

		public void emu_frameskip(object num_frames)
		{
			try
			{
				string temp = num_frames.ToString();
				int frames = Convert.ToInt32(temp);
				if (frames > 0)
				{
					Global.Config.FrameSkip = frames;
					Global.MainForm.FrameSkipMessage();
				}
				else
				{
					console_log("Invalid frame skip value");
				}
			}
			catch
			{
				console_log("Invalid frame skip value");
			}
		}

		// For now, it accepts arguments up to 5.
		public void emu_setrenderplanes(
			object lua_p0, object lua_p1 = null, object lua_p2 = null,
			object lua_p3 = null, object lua_p4 = null)
		{
			emu_setrenderplanes_do(LuaVarArgs(lua_p0, lua_p1, lua_p2, lua_p3, lua_p4));
		}

		// TODO: error handling for argument count mismatch
		private void emu_setrenderplanes_do(object[] lua_p)
		{
			if (Global.Emulator is NES)
			{
				Global.CoreComm.NES_ShowOBJ = Global.Config.NESDispSprites = (bool)lua_p[0];
				Global.CoreComm.NES_ShowBG = Global.Config.NESDispBackground = (bool)lua_p[1];
			}
			else if (Global.Emulator is Emulation.Consoles.TurboGrafx.PCEngine)
			{
				Global.CoreComm.PCE_ShowOBJ1 = Global.Config.PCEDispOBJ1 = (bool)lua_p[0];
				Global.CoreComm.PCE_ShowBG1 = Global.Config.PCEDispBG1 = (bool)lua_p[1];
				if (lua_p.Length > 2)
				{
					Global.CoreComm.PCE_ShowOBJ2 = Global.Config.PCEDispOBJ2 = (bool)lua_p[2];
					Global.CoreComm.PCE_ShowBG2 = Global.Config.PCEDispBG2 = (bool)lua_p[3];
				}
			}
			else if (Global.Emulator is Emulation.Consoles.Sega.SMS)
			{
				Global.CoreComm.SMS_ShowOBJ = Global.Config.SMSDispOBJ = (bool)lua_p[0];
				Global.CoreComm.SMS_ShowBG = Global.Config.SMSDispBG = (bool)lua_p[1];
			}
		}

		public void emu_on_snoop(LuaFunction luaf)
		{
			if (luaf != null)
			{
				Global.Emulator.CoreComm.InputCallback = delegate()
				{
					try
					{
						luaf.Call();
					}
					catch (SystemException e)
					{
						Global.MainForm.LuaConsole1.WriteToOutputWindow(
							"error running function attached by lua function emu.on_snoop" +
							"\nError message: " + e.Message);
					}
				};
			}
			else
				Global.Emulator.CoreComm.InputCallback = null;
		}

		public void client_pause_av()
		{
			Global.MainForm.PauseAVI = true;
		}

		public void client_unpause_av()
		{
			Global.MainForm.PauseAVI = false;
		}

		//----------------------------------------------------
		//Memory library
		//----------------------------------------------------

		public bool memory_usememorydomain(object lua_input)
		{
			if (lua_input.GetType() != typeof(string))
				return false;

			for (int x = 0; x < Global.Emulator.MemoryDomains.Count; x++)
			{
				if (Global.Emulator.MemoryDomains[x].Name == lua_input.ToString())
				{
					CurrentMemoryDomain = x;
					return true;
				}
			}

			return false;
		}

		public string memory_getmemorydomainlist()
		{
			return Global.Emulator.MemoryDomains.Aggregate("", (current, t) => current + (t.Name + '\n'));
		}

		public string memory_getcurrentmemorydomain()
		{
			return Global.Emulator.MemoryDomains[CurrentMemoryDomain].Name;
		}

		public int memory_getcurrentmemorydomainsize()
		{
			return Global.Emulator.MemoryDomains[CurrentMemoryDomain].Size;
		}

		public uint memory_readbyte(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_U8(addr);
		}

		public void memory_writebyte(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			M_W_U8(addr, v);
		}

		public int memory_read_s8(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return (sbyte)M_R_U8(addr);
		}

		public uint memory_read_u8(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_U8(addr);
		}

		public int memory_read_s16_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_S_LE(addr, 2);
		}

		public int memory_read_s24_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_S_LE(addr, 3);
		}

		public int memory_read_s32_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_S_LE(addr, 4);
		}

		public uint memory_read_u16_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_U_LE(addr, 2);
		}

		public uint memory_read_u24_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_U_LE(addr, 3);
		}

		public uint memory_read_u32_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_U_LE(addr, 4);
		}

		public int memory_read_s16_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_S_BE(addr, 2);
		}

		public int memory_read_s24_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_S_BE(addr, 3);
		}

		public int memory_read_s32_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_S_BE(addr, 4);
		}

		public uint memory_read_u16_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_U_BE(addr, 2);
		}

		public uint memory_read_u24_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_U_BE(addr, 3);
		}

		public uint memory_read_u32_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return M_R_U_BE(addr, 4);
		}

		public void memory_write_s8(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			M_W_U8(addr, (uint)v);
		}

		public void memory_write_u8(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			M_W_U8(addr, v);
		}

		public void memory_write_s16_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			M_W_S_LE(addr, v, 2);
		}

		public void memory_write_s24_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			M_W_S_LE(addr, v, 3);
		}

		public void memory_write_s32_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			M_W_S_LE(addr, v, 4);
		}

		public void memory_write_u16_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			M_W_U_LE(addr, v, 2);
		}

		public void memory_write_u24_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			M_W_U_LE(addr, v, 3);
		}

		public void memory_write_u32_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			M_W_U_LE(addr, v, 4);
		}

		public void memory_write_s16_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			M_W_S_BE(addr, v, 2);
		}

		public void memory_write_s24_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			M_W_S_BE(addr, v, 3);
		}

		public void memory_write_s32_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			M_W_S_BE(addr, v, 4);
		}

		public void memory_write_u16_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			M_W_U_BE(addr, v, 2);
		}

		public void memory_write_u24_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			M_W_U_BE(addr, v, 3);
		}

		public void memory_write_u32_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			M_W_U_BE(addr, v, 4);
		}

		private int M_R_S_LE(int addr, int size)
		{
			return U2S(M_R_U_LE(addr, size), size);
		}

		private uint M_R_U_LE(int addr, int size)
		{
			uint v = 0;
			for (int i = 0; i < size; ++i)
				v |= M_R_U8(addr + i) << 8 * i;
			return v;
		}

		private int M_R_S_BE(int addr, int size)
		{
			return U2S(M_R_U_BE(addr, size), size);
		}

		private uint M_R_U_BE(int addr, int size)
		{
			uint v = 0;
			for (int i = 0; i < size; ++i)
				v |= M_R_U8(addr + i) << 8 * (size - 1 - i);
			return v;
		}

		private void M_W_S_LE(int addr, int v, int size)
		{
			M_W_U_LE(addr, (uint)v, size);
		}

		private void M_W_U_LE(int addr, uint v, int size)
		{
			for (int i = 0; i < size; ++i)
				M_W_U8(addr + i, (v >> (8 * i)) & 0xFF);
		}

		private void M_W_S_BE(int addr, int v, int size)
		{
			M_W_U_BE(addr, (uint)v, size);
		}

		private void M_W_U_BE(int addr, uint v, int size)
		{
			for (int i = 0; i < size; ++i)
				M_W_U8(addr + i, (v >> (8 * (size - 1 - i))) & 0xFF);
		}

		private uint M_R_U8(int addr)
		{
			return Global.Emulator.MemoryDomains[CurrentMemoryDomain].PeekByte(addr);
		}

		private void M_W_U8(int addr, uint v)
		{
			Global.Emulator.MemoryDomains[CurrentMemoryDomain].PokeByte(addr, (byte)v);
		}

		//----------------------------------------------------
		//Main Memory library
		//----------------------------------------------------

		public uint mainmemory_readbyte(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_U8(addr);
		}

		public void mainmemory_writebyte(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			MM_W_U8(addr, v);
		}

		public LuaTable mainmemory_readbyterange(object address, object length)
		{
			int l = LuaInt(length);
			int addr = LuaInt(address);
			int last_addr = l + addr;
			LuaTable table = _lua.NewTable();
			for (int i = addr; i <= last_addr; i++)
			{
				string a = String.Format("{0:X2}", i);
				byte v = Global.Emulator.MainMemory.PeekByte(i);
				string vs = String.Format("{0:X2}", (int)v);
				table[a] = vs;
			}
			return table;
		}

		public void mainmemory_writebyterange(LuaTable memoryblock)
		{
			foreach (var address in memoryblock.Keys)
			{
				int a = LuaInt(address);
				int v = LuaInt(memoryblock[address]);

				Global.Emulator.MainMemory.PokeByte(a, (byte)v);
			}
		}

		public int mainmemory_read_s8(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return (sbyte)MM_R_U8(addr);
		}

		public uint mainmemory_read_u8(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_U8(addr);
		}

		public int mainmemory_read_s16_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_S_LE(addr, 2);
		}

		public int mainmemory_read_s24_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_S_LE(addr, 3);
		}

		public int mainmemory_read_s32_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_S_LE(addr, 4);
		}

		public uint mainmemory_read_u16_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_U_LE(addr, 2);
		}

		public uint mainmemory_read_u24_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_U_LE(addr, 3);
		}

		public uint mainmemory_read_u32_le(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_U_LE(addr, 4);
		}

		public int mainmemory_read_s16_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_S_BE(addr, 2);
		}

		public int mainmemory_read_s24_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_S_BE(addr, 3);
		}

		public int mainmemory_read_s32_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_S_BE(addr, 4);
		}

		public uint mainmemory_read_u16_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_U_BE(addr, 2);
		}

		public uint mainmemory_read_u24_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_U_BE(addr, 3);
		}

		public uint mainmemory_read_u32_be(object lua_addr)
		{
			int addr = LuaInt(lua_addr);
			return MM_R_U_BE(addr, 4);
		}

		public void mainmemory_write_s8(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			MM_W_U8(addr, (uint)v);
		}

		public void mainmemory_write_u8(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			MM_W_U8(addr, v);
		}

		public void mainmemory_write_s16_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			MM_W_S_LE(addr, v, 2);
		}

		public void mainmemory_write_s24_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			MM_W_S_LE(addr, v, 3);
		}

		public void mainmemory_write_s32_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			MM_W_S_LE(addr, v, 4);
		}

		public void mainmemory_write_u16_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			MM_W_U_LE(addr, v, 2);
		}

		public void mainmemory_write_u24_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			MM_W_U_LE(addr, v, 3);
		}

		public void mainmemory_write_u32_le(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			MM_W_U_LE(addr, v, 4);
		}

		public void mainmemory_write_s16_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			MM_W_S_BE(addr, v, 2);
		}

		public void mainmemory_write_s24_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			MM_W_S_BE(addr, v, 3);
		}

		public void mainmemory_write_s32_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			int v = LuaInt(lua_v);
			MM_W_S_BE(addr, v, 4);
		}

		public void mainmemory_write_u16_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			MM_W_U_BE(addr, v, 2);
		}

		public void mainmemory_write_u24_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			MM_W_U_BE(addr, v, 3);
		}

		public void mainmemory_write_u32_be(object lua_addr, object lua_v)
		{
			int addr = LuaInt(lua_addr);
			uint v = LuaUInt(lua_v);
			MM_W_U_BE(addr, v, 4);
		}

		private int MM_R_S_LE(int addr, int size)
		{
			return U2S(MM_R_U_LE(addr, size), size);
		}

		private uint MM_R_U_LE(int addr, int size)
		{
			uint v = 0;
			for (int i = 0; i < size; ++i)
				v |= MM_R_U8(addr + i) << 8 * i;
			return v;
		}

		private int MM_R_S_BE(int addr, int size)
		{
			return U2S(MM_R_U_BE(addr, size), size);
		}

		private uint MM_R_U_BE(int addr, int size)
		{
			uint v = 0;
			for (int i = 0; i < size; ++i)
				v |= MM_R_U8(addr + i) << 8 * (size - 1 - i);
			return v;
		}

		private void MM_W_S_LE(int addr, int v, int size)
		{
			MM_W_U_LE(addr, (uint)v, size);
		}

		private void MM_W_U_LE(int addr, uint v, int size)
		{
			for (int i = 0; i < size; ++i)
				MM_W_U8(addr + i, (v >> (8 * i)) & 0xFF);
		}

		private void MM_W_S_BE(int addr, int v, int size)
		{
			MM_W_U_BE(addr, (uint)v, size);
		}

		private void MM_W_U_BE(int addr, uint v, int size)
		{
			for (int i = 0; i < size; ++i)
				MM_W_U8(addr + i, (v >> (8 * (size - 1 - i))) & 0xFF);
		}

		private uint MM_R_U8(int addr)
		{
			return Global.Emulator.MainMemory.PeekByte(addr);
		}

		private void MM_W_U8(int addr, uint v)
		{
			Global.Emulator.MainMemory.PokeByte(addr, (byte)v);
		}

		private int U2S(uint u, int size)
		{
			int s = (int)u;
			s <<= 8 * (4 - size);
			s >>= 8 * (4 - size);
			return s;
		}

		//----------------------------------------------------
		//Bitwise Operator library
		//----------------------------------------------------
		public uint bit_band(object val, object amt)
		{
			return (uint)(LuaInt(val) & LuaInt(amt));
		}

		public uint bit_lshift(object val, object amt)
		{
			return (uint)(LuaInt(val) << LuaInt(amt));
		}

		public uint bit_rshift(object val, object amt)
		{
			return (uint)(LuaInt(val) >> LuaInt(amt));
		}

		public uint bit_rol(object val, object amt)
		{
			return (uint)((LuaInt(val) << LuaInt(amt)) | (LuaInt(val) >> (32 - LuaInt(amt))));
		}

		public uint bit_ror(object val, object amt)
		{
			return (uint)((LuaInt(val) >> LuaInt(amt)) | (LuaInt(val) << (32 - LuaInt(amt))));
		}

		public uint bit_bor(object val, object amt)
		{
			return (uint)(LuaInt(val) | LuaInt(amt));
		}

		public uint bit_bxor(object val, object amt)
		{
			return (uint)(LuaInt(val) ^ LuaInt(amt));
		}

		public uint bit_bnot(object val)
		{
			return (uint)(~LuaInt(val));
		}

		//----------------------------------------------------
		//Savestate library
		//----------------------------------------------------
		public void savestate_saveslot(object lua_input)
		{
			int x;

			try //adelikat:  This crap might not be necessary, need to test for a more elegant solution
			{
				x = int.Parse(lua_input.ToString());
			}
			catch
			{
				return;
			}

			if (x < 0 || x > 9)
				return;

			Global.MainForm.SaveState("QuickSave" + x.ToString());
		}

		public void savestate_loadslot(object lua_input)
		{
			int x;

			try //adelikat:  This crap might not be necessary, need to test for a more elegant solution
			{
				x = int.Parse(lua_input.ToString());
			}
			catch
			{
				return;
			}

			if (x < 0 || x > 9)
				return;

			Global.MainForm.LoadState("QuickSave" + x.ToString(), true);
		}

		public void savestate_save(object lua_input)
		{
			if (lua_input is string)
			{
				string path = lua_input.ToString();
				Global.MainForm.SaveStateFile(path, path, true);
			}
		}

		public void savestate_load(object lua_input)
		{
			if (lua_input is string)
			{
				Global.MainForm.LoadStateFile(lua_input.ToString(), Path.GetFileName(lua_input.ToString()), true);
			}
		}

		public void savestate_registersave(LuaFunction luaf)
		{
			savestate_registersavefunc = luaf;
		}

		public void savestate_registerload(LuaFunction luaf)
		{
			savestate_registerloadfunc = luaf;
		}

		//----------------------------------------------------
		//Movie library
		//----------------------------------------------------
		public string movie_mode()
		{
			if (Global.MovieSession.Movie.IsFinished)
			{
				return "FINISHED";
			}
			else if (Global.MovieSession.Movie.IsPlaying)
			{
				return "PLAY";
			}
			else if (Global.MovieSession.Movie.IsRecording)
			{
				return "RECORD";
			}
			else
			{
				return "INACTIVE";
			}
		}

		public string movie_rerecordcount()
		{
			return Global.MovieSession.Movie.Rerecords.ToString();
		}

		public void movie_stop()
		{
			Global.MovieSession.Movie.Stop();
		}

		public bool movie_isloaded()
		{
			if (Global.MovieSession.Movie.IsActive)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public int movie_length()
		{
			if (Global.MovieSession.Movie.Frames.HasValue)
			{
				return Global.MovieSession.Movie.Frames.Value;
			}
			else
			{
				return -1;
			}
		}

		public string movie_filename()
		{
			return Global.MovieSession.Movie.Filename;
		}

		public bool movie_getreadonly()
		{
			return Global.MainForm.ReadOnly;
		}

		public void movie_setreadonly(object lua_input)
		{
			if (lua_input.ToString().ToUpper() == "TRUE" || lua_input.ToString() == "1")
				Global.MainForm.SetReadOnly(true);
			else
				Global.MainForm.SetReadOnly(false);
		}

		public LuaTable movie_getinput(object frame)
		{
			LuaTable input = _lua.NewTable();

			string s = Global.MovieSession.Movie.GetInput(LuaInt(frame));
			MovieControllerAdapter m = new MovieControllerAdapter {Type = Global.MovieSession.MovieControllerAdapter.Type};
			m.SetControllersAsMnemonic(s);
			foreach (string button in m.Type.BoolButtons)
				input[button] = m[button];

			return input;
		}

		public bool movie_getrerecordcounting()
		{
			return Global.MovieSession.Movie.IsCountingRerecords;
		}

		public void movie_setrerecordcounting(object lua_input)
		{
			if (lua_input.ToString().ToUpper() == "TRUE" || lua_input.ToString() == "1")
				Global.MovieSession.Movie.IsCountingRerecords = true;
			else
				Global.MovieSession.Movie.IsCountingRerecords = false;
		}
		//----------------------------------------------------
		//Input library
		//----------------------------------------------------
		public LuaTable input_get()
		{
			LuaTable buttons = _lua.NewTable();
			foreach (var kvp in Global.ControllerInputCoalescer.BoolButtons())
				if (kvp.Value)
					buttons[kvp.Key] = true;
			return buttons;
		}

		//----------------------------------------------------
		//Joypad library
		//----------------------------------------------------

		//Currently sends all controllers, needs to control which ones it sends
		public LuaTable joypad_get(object controller = null)
		{
			LuaTable buttons = _lua.NewTable();
            foreach (string button in Global.ControllerOutput.Source.Type.BoolButtons)
            {
                if (controller == null)
                {
                    buttons[button] = Global.ControllerOutput[button];
                }
                else if (button.Length >= 3 && button.Substring(0, 2) == "P" + LuaInt(controller).ToString())
                {
                    buttons[button.Substring(3)] = Global.ControllerOutput["P" + LuaInt(controller) + " " + button.Substring(3)];
                }
            }

            foreach (string button in Global.ControllerOutput.Source.Type.FloatControls)
            {
                if (controller == null)
                {
                    buttons[button] = Global.ControllerOutput.GetFloat(button);
                }
                else if (button.Length >= 3 && button.Substring(0, 2) == "P" + LuaInt(controller).ToString())
                {
                    buttons[button.Substring(3)] = Global.ControllerOutput.GetFloat("P" + LuaInt(controller) + " " + button.Substring(3));
                }
            }

			buttons["clear"] = null;
			buttons["getluafunctionslist"] = null;
			buttons["output"] = null;

			return buttons;
		}

		public LuaTable joypad_getimmediate()
		{
			LuaTable buttons = _lua.NewTable();
			foreach (string button in Global.ActiveController.Type.BoolButtons)
				buttons[button] = Global.ActiveController[button];
			return buttons;
		}

		public void joypad_set(LuaTable buttons, object controller = null)
		{
			try
			{
				foreach (var button in buttons.Keys)
				{
					bool invert = false;
					bool? theValue;
					string theValueStr = buttons[button].ToString();
					
					if (!String.IsNullOrWhiteSpace(theValueStr))
					{
						if (theValueStr.ToLower() == "false")
						{
							theValue = false;
						}
						else if (theValueStr.ToLower() == "true")
						{
							theValue = true;
						}
						else
						{
							invert = true;
							theValue = null;
						}
					}
					else
					{
						theValue = null;
					}
					

					if (!invert)
					{
						if (theValue == true)
						{
							if (controller == null) //Force On
							{
								Global.ClickyVirtualPadController.Click(button.ToString());
								Global.ForceOffAdaptor.SetSticky(button.ToString(), false);
							}
							else
							{
								Global.ClickyVirtualPadController.Click("P" + controller + " " + button);
								Global.ForceOffAdaptor.SetSticky("P" + controller + " " + button, false);
							}
						}
						else if (theValue == false) //Force off
						{
							if (controller == null)
							{
								Global.ForceOffAdaptor.SetSticky(button.ToString(), true);
							}
							else
							{
								Global.ForceOffAdaptor.SetSticky("P" + controller + " " + button, true);
							}
						}
						else
						{
							//Turn everything off
							if (controller == null)
							{
								Global.ForceOffAdaptor.SetSticky(button.ToString(), false);
							}
							else
							{
								Global.ForceOffAdaptor.SetSticky("P" + controller + " " + button, false);
							}
						}
					}
					else //Inverse
					{
						if (controller == null)
						{
							Global.StickyXORAdapter.SetSticky(button.ToString(), true);
							Global.ForceOffAdaptor.SetSticky(button.ToString(), false);
						}
						else
						{
							Global.StickyXORAdapter.SetSticky("P" + controller + " " + button, true);
							Global.ForceOffAdaptor.SetSticky("P" + controller + " " + button, false);
						}
					}
				}
			}
			catch { /*Eat it*/ } 
		}

		public void joypad_setanalog(LuaTable controls, object controller = null)
		{
			try
			{
				foreach (var name in controls.Keys)
				{
					string theValueStr = controls[name].ToString();

					if (!String.IsNullOrWhiteSpace(theValueStr))
					{
						try
						{
							float theValue = float.Parse(theValueStr);
							if (controller == null)
							{
								Global.StickyXORAdapter.SetFloat(name.ToString(), theValue);
							}
							else
							{
								Global.StickyXORAdapter.SetFloat("P" + controller + " " + name, theValue);
							}
						}
						catch { }
					}
				}
			}
			catch { /*Eat it*/ }
		}

		//----------------------------------------------------
		//Client library
		//----------------------------------------------------
		public void client_screenshot(object path = null)
		{
			if (path == null)
			{
				Global.MainForm.TakeScreenshot();
			}
			else
			{
				Global.MainForm.TakeScreenshot(path.ToString());
			}
		}

		public void client_screenshottoclipboard()
		{
			Global.MainForm.TakeScreenshotToClipboard();
		}

		public void client_setscreenshotosd(bool value)
		{
			Global.Config.Screenshot_CaptureOSD = value;
		}

		public int client_screenwidth()
		{
			return Global.RenderPanel.NativeSize.Width;
		}
        public int client_xpos()
        {
            return Global.MainForm.DesktopLocation.X;
        }

		public int client_screenheight()
		{
			return Global.RenderPanel.NativeSize.Height;
		}
        public int client_ypos()
        {
            return Global.MainForm.DesktopLocation.Y;
        }

		public void client_openrom(object lua_input)
		{
			Global.MainForm.LoadRom(lua_input.ToString());
		}

		public void client_closerom()
		{
			Global.MainForm.CloseROM();
		}

		public void client_opentoolbox()
		{
			Global.MainForm.LoadToolBox();
		}

		public void client_openramwatch()
		{
			Global.MainForm.LoadRamWatch(true);
		}

		public void client_openramsearch()
		{
			Global.MainForm.LoadRamSearch();
		}

		public void client_openrampoke()
		{
			Global.MainForm.LoadRamPoke();
		}

		public void client_openhexeditor()
		{
			Global.MainForm.LoadHexEditor();
		}

		public void client_opentasstudio()
		{
			Global.MainForm.LoadTAStudio();
		}

		public void client_setwindowsize(object window_size)
		{
			try
			{
				string temp = window_size.ToString();
				int size = Convert.ToInt32(temp);
				if (size == 1 || size == 2 || size == 3 || size == 4 || size == 5 || size == 10)
				{
					Global.Config.TargetZoomFactor = size;
					Global.MainForm.FrameBufferResized();
					Global.OSD.AddMessage("Window size set to " + size.ToString() + "x");
				}
				else
				{
					console_log("Invalid window size");
				}
			}
			catch
			{
				console_log("Invalid window size");
			}

		}

		public int client_getwindowsize()
		{
			return Global.Config.TargetZoomFactor;
		}

		public void client_opencheats()
		{
			Global.MainForm.LoadCheatsWindow();
		}

		//----------------------------------------------------
		//Winforms library
		//----------------------------------------------------
		public List<LuaWinform> LuaForms = new List<LuaWinform>();

		public int forms_newform(object Width = null, object Height = null, object title = null)
		{

			LuaWinform theForm = new LuaWinform();
			LuaForms.Add(theForm);
			if (Width != null && Height != null)
			{
				theForm.Size = new Size(LuaInt(Width), LuaInt(Height));
			}

			if (title != null)
			{
				theForm.Text = title.ToString();
			}

			theForm.Show();
			return (int)theForm.Handle;
		}

		public void WindowClosed(IntPtr handle)
		{
			foreach (LuaWinform form in LuaForms)
			{
				if (form.Handle == handle)
				{
					LuaForms.Remove(form);
					return;
				}
			}
		}

		public bool forms_destroy(object handle)
		{
			IntPtr ptr = new IntPtr(LuaInt(handle));
			foreach (LuaWinform form in LuaForms)
			{
				if (form.Handle == ptr)
				{
					form.Close();
					LuaForms.Remove(form);
					return true;
				}
			}
			return false;
		}

		public void forms_destroyall()
		{
			foreach (LuaWinform form in LuaForms)
			{
				form.Close();
				LuaForms.Remove(form);
			}
		}

		private LuaWinform GetForm(object form_handle)
		{
			IntPtr ptr = new IntPtr(LuaInt(form_handle));
			return LuaForms.FirstOrDefault(form => form.Handle == ptr);
		}

		private void SetLocation(Control control, object X, object Y)
		{
			try
			{
				if (X != null && Y != null)
				{
					int x = LuaInt(X);
					int y = LuaInt(Y);
					control.Location = new Point(x, y);
				}
			}
			catch
			{
				//Do nothing
			}
		}

		private void SetSize(Control control, object Width, object Height)
		{
			try
			{
				if (Width != null && Height != null)
				{
					int width = LuaInt(Width);
					int height = LuaInt(Height);
					control.Size = new Size(width, height);
				}
			}
			catch
			{
				//Do nothing
			}
		}

		private void SetText(Control control, object caption)
		{
			if (caption != null)
			{
				control.Text = caption.ToString();
			}
		}

		public int forms_button(object form_handle, object caption, LuaFunction lua_event, object X = null, object Y = null, object width = null, object height = null)
		{
			LuaWinform form = GetForm(form_handle);
			if (form == null)
			{
				return 0;
			}

			LuaButton button = new LuaButton();
			SetText(button, caption);
			form.Controls.Add(button);
			form.Control_Events.Add(new LuaWinform.Lua_Event(button.Handle, lua_event));

			if (X != null && Y != null)
				SetLocation(button, X, Y);

			if (width != null & height != null)
				SetSize(button, width, height);

			return (int)button.Handle;
		}

		public int forms_label(object form_handle, object caption, object X = null, object Y = null, object width = null, object height = null)
		{
			LuaWinform form = GetForm(form_handle);
			if (form == null)
			{
				return 0;
			}

			Label label = new Label();
			SetText(label, caption);
			form.Controls.Add(label);
			if (X != null && Y != null)
				SetLocation(label, X, Y);

			if (width != null & height != null)
				SetSize(label, width, height);

			return (int)label.Handle;
		}

		public int forms_textbox(object form_handle, object caption = null, object width = null, object height = null, object boxtype = null, object X = null, object Y = null)
		{
			LuaWinform form = GetForm(form_handle);
			if (form == null)
			{
				return 0;
			}

			LuaTextBox textbox = new LuaTextBox();
			SetText(textbox, caption);

			if (X != null && Y != null)
				SetLocation(textbox, X, Y);

			if (width != null & height != null)
				SetSize(textbox, width, height);

			if (boxtype != null)
			{
				switch (boxtype.ToString().ToUpper())
				{
					case "HEX":
					case "HEXADECIMAL":
						textbox.SetType(BoxType.HEX);
						break;
					case "UNSIGNED":
					case "UINT":
						textbox.SetType(BoxType.UNSIGNED);
						break;
					case "NUMBER":
					case "NUM":
					case "SIGNED":
					case "INT":
						textbox.SetType(BoxType.SIGNED);
						break;
				}
			}
			form.Controls.Add(textbox);
			return (int)textbox.Handle;
		}

		public void forms_setlocation(object handle, object X, object Y)
		{
			IntPtr ptr = new IntPtr(LuaInt(handle));
			foreach (LuaWinform form in LuaForms)
			{
				if (form.Handle == ptr)
				{
					SetLocation(form, X, Y);
				}
				else
				{
					foreach (Control control in form.Controls)
					{
						if (control.Handle == ptr)
						{
							SetLocation(control, X, Y);
						}
					}
				}
			}
		}

		public void forms_setsize(object handle, object Width, object Height)
		{
			IntPtr ptr = new IntPtr(LuaInt(handle));
			foreach (LuaWinform form in LuaForms)
			{
				if (form.Handle == ptr)
				{
					SetSize(form, Width, Height);
				}
				else
				{
					foreach (Control control in form.Controls)
					{
						if (control.Handle == ptr)
						{
							SetSize(control, Width, Height);
						}
					}
				}
			}
		}

		public void forms_settext(object handle, object caption)
		{
			IntPtr ptr = new IntPtr(LuaInt(handle));
			foreach (LuaWinform form in LuaForms)
			{
				if (form.Handle == ptr)
				{
					SetText(form, caption);
				}
				else
				{
					foreach (Control control in form.Controls)
					{
						if (control.Handle == ptr)
						{
							SetText(control, caption);
						}
					}
				}
			}
		}

		public void forms_setproperty(object handle, object property, object value)
		{
			IntPtr ptr = new IntPtr(LuaInt(handle));
			foreach (LuaWinform form in LuaForms)
			{
				if (form.Handle == ptr)
				{
					form.GetType().GetProperty(property.ToString()).SetValue(form, Convert.ChangeType(value, form.GetType().GetProperty(property.ToString()).PropertyType), null);
				}
				else
				{
					foreach (Control control in form.Controls)
					{
						if (control.Handle == ptr)
						{
							control.GetType().GetProperty(property.ToString()).SetValue(control, Convert.ChangeType(value, form.GetType().GetProperty(property.ToString()).PropertyType), null);
						}
					}
				}
			}
		}

		public void forms_addclick(object handle, LuaFunction lua_event)
		{
			IntPtr ptr = new IntPtr(LuaInt(handle));
			foreach (LuaWinform form in LuaForms)
			{
				foreach (Control control in form.Controls)
				{
					if (control.Handle == ptr)
					{
						form.Control_Events.Add(new LuaWinform.Lua_Event(control.Handle, lua_event));
					}
				}
			}
		}

		public void forms_clearclicks(object handle)
		{
			IntPtr ptr = new IntPtr(LuaInt(handle));
			foreach (LuaWinform form in LuaForms)
			{
				foreach (Control control in form.Controls)
				{
					if (control.Handle == ptr)
					{
						List<LuaWinform.Lua_Event> lua_events = form.Control_Events.Where(x => x.Control == ptr).ToList();
						foreach (LuaWinform.Lua_Event levent in lua_events)
						{
							form.Control_Events.Remove(levent);
						}
					}
				}
			}
		}

		public string forms_gettext(object handle)
		{
			try
			{
				IntPtr ptr = new IntPtr(LuaInt(handle));
				foreach (LuaWinform form in LuaForms)
				{
					if (form.Handle == ptr)
					{
						return form.Text;
					}
					else
					{
						foreach (Control control in form.Controls)
						{
							if (control.Handle == ptr)
							{
								return control.Text;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				console_output(ex.Message);
			}

			return "";
		}

		public string forms_getproperty(object handle, object property)
		{
			try
			{
				IntPtr ptr = new IntPtr(LuaInt(handle));
				foreach (LuaWinform form in LuaForms)
				{
					if (form.Handle == ptr)
					{
						return form.GetType().GetProperty(property.ToString()).GetValue(form, null).ToString();
					}
					else
					{
						foreach (Control control in form.Controls)
						{
							if (control.Handle == ptr)
							{
								return control.GetType().GetProperty(property.ToString()).GetValue(control, null).ToString();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				console_output(ex.Message);
			}

			return "";
		}

		// filterext format ex: "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
		public string forms_openfile(string FileName = null, string InitialDirectory = null, string Filter = "All files (*.*)|*.*")
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			if (InitialDirectory != null)
			{
				openFileDialog1.InitialDirectory = InitialDirectory; 
			}
			if (FileName != null)
			{
				openFileDialog1.FileName = FileName;
			}
			 if (Filter != null)
			{
				openFileDialog1.AddExtension = true;
				openFileDialog1.Filter = Filter;
			}
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
				return openFileDialog1.FileName;
			else
				return "";
		}

		public LuaTable input_getmouse()
		{
			LuaTable buttons = _lua.NewTable();
			Point p = Global.RenderPanel.ScreenToScreen(Control.MousePosition);
			buttons["X"] = p.X;
			buttons["Y"] = p.Y;
			buttons[MouseButtons.Left.ToString()] = (Control.MouseButtons & MouseButtons.Left) != 0;
			buttons[MouseButtons.Middle.ToString()] = (Control.MouseButtons & MouseButtons.Middle) != 0;
			buttons[MouseButtons.Right.ToString()] = (Control.MouseButtons & MouseButtons.Right) != 0;
			buttons[MouseButtons.XButton1.ToString()] = (Control.MouseButtons & MouseButtons.XButton1) != 0;
			buttons[MouseButtons.XButton2.ToString()] = (Control.MouseButtons & MouseButtons.XButton2) != 0;
			return buttons;
		}

		//----------------------------------------------------
		//NES library
		//----------------------------------------------------

		public void nes_setscanlines(object top, object bottom, bool pal = false)
		{

			int first = LuaInt(top);
			int last = LuaInt(bottom);
			if (first > 127)
			{
				first = 127;
			}
			else if (first < 0)
			{
				first = 0;
			}

			if (last > 239)
			{
				last = 239;
			}
			else if (last < 128)
			{
				last = 128;
			}

			if (pal)
			{
				Global.Config.PAL_NESTopLine = first;
				Global.Config.PAL_NESBottomLine = last;
			}
			else
			{
				Global.Config.NTSC_NESTopLine = first;
				Global.Config.NTSC_NESBottomLine = last;
			}

			if (Global.Emulator is NES)
			{
				if (pal)
				{
					(Global.Emulator as NES).PAL_FirstDrawLine = first;
					(Global.Emulator as NES).PAL_LastDrawLine = last;
				}
				else
				{
					(Global.Emulator as NES).NTSC_FirstDrawLine = first;
					(Global.Emulator as NES).NTSC_LastDrawLine = last;
				}
			}
		}

		public int nes_gettopscanline(bool pal = false)
		{
			if (pal)
			{
				return Global.Config.PAL_NESTopLine;
			}
			else
			{
				return Global.Config.NTSC_NESTopLine;
			}
		}

		public int nes_getbottomscanline(bool pal = false)
		{
			if (pal)
			{
				return Global.Config.PAL_NESBottomLine;
			}
			else
			{
				return Global.Config.NTSC_NESBottomLine;
			}
		}

		public bool nes_getclipleftandright()
		{
			return Global.Config.NESClipLeftAndRight;
		}

		public void nes_setclipleftandright(bool leftandright)
		{
			Global.Config.NESClipLeftAndRight = leftandright;
			if (Global.Emulator is NES)
			{
				(Global.Emulator as NES).SetClipLeftAndRight(leftandright);
			}
		}

		public bool nes_getdispbackground()
		{
			return Global.Config.NESDispBackground;
		}

		public void nes_setdispbackground(bool show)
		{
			Global.Config.NESDispBackground = show;
			Global.MainForm.SyncCoreCommInputSignals();
		}

		public bool nes_getdispsprites()
		{
			return Global.Config.NESDispSprites;
		}

		public void nes_setdispsprites(bool show)
		{
			Global.Config.NESDispSprites = show;
			Global.MainForm.SyncCoreCommInputSignals();
		}

		public bool nes_getallowmorethaneightsprites()
		{
			return Global.Config.NESAllowMoreThanEightSprites;
		}

		public void nes_setallowmorethaneightsprites(bool allow)
		{
			Global.Config.NESAllowMoreThanEightSprites = allow;
			if (Global.Emulator is NES)
			{
				(Global.Emulator as NES).CoreComm.NES_UnlimitedSprites = allow;
			}
		}

		public void nes_addgamegenie(string code)
		{
			if (Global.Emulator is NES)
			{
				NESGameGenie gg = new NESGameGenie();
				gg.DecodeGameGenieCode(code);
				if (gg.Address > 0 && gg.Value > 0)
				{
					Cheat c = new Cheat
						{
							name = code,
							domain = Global.Emulator.MemoryDomains[1],
							address = gg.Address,
							value = (byte) gg.Value
						};
					if (gg.Compare != -1)
					{
						c.compare = (byte)gg.Compare;
					}
					c.Enable();
					Global.MainForm.Cheats1.AddCheat(c);
				}
			}
		}

		public void nes_removegamegenie(string code)
		{
			if (Global.Emulator is NES)
			{
				NESGameGenie gg = new NESGameGenie();
				gg.DecodeGameGenieCode(code);
				if (gg.Address > 0 && gg.Value > 0)
				{
					Cheat c = new Cheat
						{
							name = code,
							domain = Global.Emulator.MemoryDomains[1],
							address = gg.Address,
							value = (byte) gg.Value
						};
					if (gg.Compare != -1)
					{
						c.compare = (byte)gg.Compare;
					}
					Global.CheatList.RemoveCheat(Global.Emulator.MemoryDomains[1], c.address);
				}
			}
		}

		//----------------------------------------------------
		//SNES library
		//----------------------------------------------------
		public bool snes_getlayer_bg_1()
		{
			return Global.Config.SNES_ShowBG1_1;
		}

		public bool snes_getlayer_bg_2()
		{
			return Global.Config.SNES_ShowBG2_1;
		}

		public bool snes_getlayer_bg_3()
		{
			return Global.Config.SNES_ShowBG3_1;
		}

		public bool snes_getlayer_bg_4()
		{
			return Global.Config.SNES_ShowBG4_1;
		}

		public bool snes_getlayer_obj_1()
		{
			return Global.Config.SNES_ShowOBJ1;
		}

		public bool snes_getlayer_obj_2()
		{
			return Global.Config.SNES_ShowOBJ2;
		}

		public bool snes_getlayer_obj_3()
		{
			return Global.Config.SNES_ShowOBJ3;
		}

		public bool snes_getlayer_obj_4()
		{
			return Global.Config.SNES_ShowOBJ4;
		}


		public void snes_setlayer_bg_1(bool value)
		{
			Global.MainForm.SNES_ToggleBG1(value);
		}

		public void snes_setlayer_bg_2(bool value)
		{
			Global.MainForm.SNES_ToggleBG2(value);
		}

		public void snes_setlayer_bg_3(bool value)
		{
			Global.MainForm.SNES_ToggleBG3(value);
		}

		public void snes_setlayer_bg_4(bool value)
		{
			Global.MainForm.SNES_ToggleBG4(value);
		}

		public void snes_setlayer_obj_1(bool value)
		{
			Global.MainForm.SNES_ToggleOBJ1(value);
		}

		public void snes_setlayer_obj_2(bool value)
		{
			Global.MainForm.SNES_ToggleOBJ2(value);
		}

		public void snes_setlayer_obj_3(bool value)
		{
			Global.MainForm.SNES_ToggleOBJ3(value);
		}

		public void snes_setlayer_obj_4(bool value)
		{
			Global.MainForm.SNES_ToggleOBJ4(value);
		}
		//----------------------------------------------------
		//Events library
		//----------------------------------------------------

		public void event_onsavestate(LuaFunction luaf)
		{
			savestate_registersave(luaf);
		}

		public void event_onloadstate(LuaFunction luaf)
		{
			savestate_registerload(luaf);
		}

		public void event_onframestart(LuaFunction luaf)
		{
			frame_startfunc = luaf;
		}

		public void event_onframeend(LuaFunction luaf)
		{
			frame_endfunc = luaf;
		}

		public void event_oninputpoll(LuaFunction luaf)
		{
			emu_on_snoop(luaf);
		}

		public void event_onmemoryread(LuaFunction luaf, object address = null)
		{
			//TODO: allow a list of addresses
			if (luaf != null)
			{
				int? _addr;
				if (address == null)
				{
					_addr = null;
				}
				else
				{
					_addr = LuaInt(address);
				}

				Global.Emulator.CoreComm.MemoryCallbackSystem.ReadAddr = _addr;
				Global.Emulator.CoreComm.MemoryCallbackSystem.SetReadCallback(delegate(uint addr)
				{
					try
					{
						luaf.Call(addr);
					}
					catch (SystemException e)
					{
						Global.MainForm.LuaConsole1.WriteToOutputWindow(
							"error running function attached by lua function event.onmemoryread" +
							"\nError message: " + e.Message);
					}
				});

			}
			else
			{
				Global.Emulator.CoreComm.MemoryCallbackSystem.SetReadCallback(null);
			}
		}


		public void event_onmemorywrite(LuaFunction luaf, object address = null)
		{
			//TODO: allow a list of addresses
			if (luaf != null)
			{
				int? _addr;
				if (address == null)
				{
					_addr = null;
				}
				else
				{
					_addr = LuaInt(address);
				}

				Global.Emulator.CoreComm.MemoryCallbackSystem.WriteAddr = _addr;
				Global.Emulator.CoreComm.MemoryCallbackSystem.SetWriteCallback(delegate(uint addr)
				{
					try
					{
						luaf.Call(addr);
					}
					catch (SystemException e)
					{
						Global.MainForm.LuaConsole1.WriteToOutputWindow(
							"error running function attached by lua function event.onmemoryread" +
							"\nError message: " + e.Message);
					}
				});
			}
			else
			{
				Global.Emulator.CoreComm.MemoryCallbackSystem.SetWriteCallback(null);
			}
		}
	}
}
