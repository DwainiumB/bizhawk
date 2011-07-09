﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BizHawk.MultiClient
{
	public class InputWidget : TextBox
	{
		//TODO: when binding, make sure that the new key combo is not in one of the other bindings

		int MaxBind = 4; //Max number of bindings allowed
		int pos = 0;	 //Which mapping the widget will listen for
		private Timer timer = new Timer();
		public bool AutoTab = true;
		string[] Bindings = new string[4];
		string wasPressed = "";
		
		[DllImport("user32")]
		private static extern bool HideCaret(IntPtr hWnd);

		public InputWidget()
		{
			this.ContextMenu = new ContextMenu();
			this.timer.Tick += new System.EventHandler(this.Timer_Tick);
			InitializeBindings();
		//	this.OnGotFocus
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			HideCaret(this.Handle);
			base.OnMouseClick(e);
		}

		public InputWidget(int maxBindings)
		{
			this.ContextMenu = new ContextMenu();
			this.timer.Tick += new System.EventHandler(this.Timer_Tick);
			MaxBind = maxBindings;
			Bindings = new string[MaxBind];
			InitializeBindings();
		}

		private void InitializeBindings()
		{
			for (int x = 0; x < MaxBind; x++)
			{
				Bindings[x] = "";
			}
		}

		protected override void OnEnter(EventArgs e)
		{
			pos = 0;
			timer.Start();
			//Input.Update();

			//zero: ??? what is this all about ???
			wasPressed = Input.Instance.GetNextPressedButtonOrNull();
		}

		protected override void OnLeave(EventArgs e)
		{
			timer.Stop();
			UpdateLabel();
			base.OnLeave(e);
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			ReadKeys();
		}

		private void ReadKeys()
		{
			Input.Instance.Update();
			string TempBindingStr = Input.Instance.GetNextPressedButtonOrNull();
			if (wasPressed != "" && TempBindingStr == wasPressed) return;
			if (TempBindingStr != null)
			{
				if (TempBindingStr == "Escape")
				{
					ClearBindings();
					Increment();
					return;
				}

				if (TempBindingStr == "Alt+F4")
					return;

				if (!IsDuplicate(TempBindingStr))
				{
					Bindings[pos] = TempBindingStr;
				}
				wasPressed = TempBindingStr;
				UpdateLabel();
				Increment();
			}
		}

		public bool IsDuplicate(string binding)
		{
			for (int x = 0; x < MaxBind; x++)
			{
				if (Bindings[x] == binding)
					return true;
			}

			return false;
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F4 && e.Modifiers == Keys.Alt)
			{
				base.OnKeyUp(e);
			}
			wasPressed = "";
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F4 && e.Modifiers == Keys.Alt)
			{
				base.OnKeyDown(e);
			}
			e.Handled = true;
		}

		// Advances to the next widget or the next binding depending on the autotab setting
		public void Increment()
		{
			if (AutoTab)
				this.Parent.SelectNextControl(this, true, true, true, true);
			else
			{
				if (pos == MaxBind - 1)
					pos = 0;
				else
					pos++;
			}
		}

		public void Decrement()
		{
			if (AutoTab)
				this.Parent.SelectNextControl(this, false, true, true, true);
			else
			{
				if (pos == 0)
					pos = MaxBind - 1;
				else
					pos--;
			}
		}

		public void ClearBindings()
		{
			for (int x = 0; x < MaxBind; x++)
				Bindings[x] = "";
		}

		public void UpdateLabel()
		{
			Text = "";
			for (int x = 0; x < MaxBind; x++)
			{
				if (Bindings[x].Length > 0)
				{
					Text += Bindings[x];
					if (x < MaxBind - 1 && Bindings[x+1].Length > 0)
						Text += ", ";
				}
			}
		}

		public void SetBindings(string bindingsString)
		{
			Text = "";
			ClearBindings();
			string str = bindingsString.Trim();
			int x;
			for (int i = 0; i < MaxBind; i++)
			{
				str = str.Trim();
				x = str.IndexOf(',');
				if (x < 0)
				{
					Bindings[i] = str;
					str = "";
				}
				else
				{
					Bindings[i] = str.Substring(0, x);
					str = str.Substring(x + 1, str.Length - x - 1);
				}
			}
			UpdateLabel();
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			e.Handled = true;
		}

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case 0x0201: //WM_LBUTTONDOWN
				{
					this.Focus();
					return;
				}
				//case 0x0202://WM_LBUTTONUP
				//{
				//	return;
				//}
				case 0x0203://WM_LBUTTONDBLCLK
				{
					return;
				}
				case 0x0204://WM_RBUTTONDOWN
				{
					return;
				}
				case 0x0205://WM_RBUTTONUP
				{
					return;
				}
				case 0x0206://WM_RBUTTONDBLCLK
				{
					return;
				}
			}

			base.WndProc(ref m);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta > 0)
				Decrement();
			else
				Increment();
			base.OnMouseWheel(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			//base.OnGotFocus(e);
			HideCaret(this.Handle);
			BackColor = Color.Pink;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			BackColor = SystemColors.Window;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			return true;
		}
	}
}