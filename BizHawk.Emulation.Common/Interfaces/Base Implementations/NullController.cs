﻿namespace BizHawk.Emulation.Common
{
	public class NullController : IController
	{
		public ControllerDefinition Type { get { return null; } }
		public bool this[string button] { get { return false; } }
		public bool IsPressed(string button) { return false; }
		public float GetFloat(string name) { return 0f; }
		public void UnpressButton(string button) { }
		public void ForceButton(string button) { }

		public void SetSticky(string button, bool sticky) { }
		public bool IsSticky(string button) { return false; }
		private static readonly NullController nullController = new NullController();
		public static NullController GetNullController() { return nullController; }
	}
}