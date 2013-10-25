﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LuaInterface;

namespace BizHawk.MultiClient
{
	public partial class LuaWinform : Form
	{
		public List<Lua_Event> Control_Events = new List<Lua_Event>();

		public LuaWinform()
		{
			InitializeComponent();
			Closing += (o, e) => CloseThis();
		}

		private void LuaWinform_Load(object sender, EventArgs e)
		{

		}

		public void CloseThis()
		{
			GlobalWinF.MainForm.LuaConsole1.LuaImp.WindowClosed(Handle);
		}

		public void DoLuaEvent(IntPtr handle)
		{
			foreach (Lua_Event l_event in Control_Events)
			{
				if (l_event.Control == handle)
				{
					l_event.Event.Call();
				}
			}
		}

		public class Lua_Event
		{
			public LuaFunction Event;
			public IntPtr Control;

			public Lua_Event() { }
			public Lua_Event(IntPtr handle, LuaFunction lfunction)
			{
				Event = lfunction;
				Control = handle;
			}
		}
	}
}
