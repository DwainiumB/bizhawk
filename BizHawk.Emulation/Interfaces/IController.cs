﻿using System.Collections.Generic;

namespace BizHawk
{
	public class ControllerDefinition
	{
		public string Name;
		public List<string> BoolButtons = new List<string>();
		public List<string> FloatControls = new List<string>();
	}

	public interface IController
	{
		ControllerDefinition Type { get; }

		//TODO - it is obnoxious for this to be here. must be removed.
		bool this[string button] { get; }
		//TODO - this can stay but it needs to be changed to go through the float
		bool IsPressed(string button);

		float GetFloat(string name);

		//TODO - why does this have a frame argument. must be removed.
		void UpdateControls(int frame);
	}
}
