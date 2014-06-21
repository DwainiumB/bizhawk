﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common
{
	public class NesMnemonicGenerator : IMnemonicPorts
	{
		private bool _isFds;
		private bool _isFourscore;

		public NesMnemonicGenerator(IController source, bool fds = false, bool isFourscore = false)
		{
			Source = source;
			_isFds = fds;
			_isFourscore = isFourscore;

			_nesConsoleControls.Source = source;
			_fdsConsoleControls.Source = source;
			_controllerPorts.ForEach(x => x.Source = source);
		}

		public bool FourScoreEnabled
		{
			get { return _isFourscore; } 
			set { _isFourscore = value; } 
		}

		public bool IsFDS
		{
			get { return _isFds; }
			set { _isFds = value; }
		}

		#region IMnemonicPorts Implementation

		public string EmptyMnemonic
		{
			get
			{
				var blah = AvailableGenerators.Select(x => x.EmptyMnemonicString);
				return "|" + string.Join("|", blah) + "|";
			}
		}

		public int Count
		{
			get { return _isFourscore ? 4 : 2; }
		}

		public IController Source { get; set; }

		// This is probably not necessary, but let's see how things go
		public IEnumerable<IMnemonicGenerator> AvailableGenerators
		{
			get
			{
				yield return ConsoleControls;

				for (var i = 0; i < Count; i++)
				{
					yield return _controllerPorts[i];
				}
			}
		}

		public IMnemonicGenerator ConsoleControls
		{
			get { return _isFds ? _fdsConsoleControls : _nesConsoleControls; }
		}

		public IMnemonicGenerator this[int portNum]
		{
			get
			{
				if (portNum < Count)
				{
					return _controllerPorts[portNum];
				}
				else
				{
					throw new ArgumentOutOfRangeException("portNum");
				}
			}

			set
			{
				if (portNum < Count)
				{
					// Eventually this will support zappers and FDS controllers, Arkanoid paddle, etc
					if (value is BooleanControllerMnemonicGenerator)
					{
						_controllerPorts[portNum] = value;
					}
					else
					{
						throw new InvalidOperationException("Invalid Mnemonic Generator for the given port");
					}
				}
				else
				{
					throw new ArgumentOutOfRangeException("portNum");
				}
			}
		}

		#region TODO: nothing specific to this object here, this could be done in a base class

		public Dictionary<string, bool> ParseMnemonicString(string mnemonicStr)
		{
			var segments = mnemonicStr.Split('|');
			var kvps = new List<KeyValuePair<string, bool>>();
			var generators = AvailableGenerators.ToList();
			for (var i = 0; i < mnemonicStr.Length; i++)
			{
				kvps.AddRange(generators[i].ParseMnemonicSegment(segments[i]));
			}

			return kvps.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		public Dictionary<string, bool> GetBoolButtons()
		{
			return AvailableGenerators
				.Where(g => g.IsFloat)
				.SelectMany(mc => mc.AvailableMnemonics)
				.ToDictionary(kvp => kvp.Key, kvp => Source.IsPressed(kvp.Key));
		}

		public Dictionary<string, float> GetFloatButtons()
		{
			return AvailableGenerators
				.Where(g => !g.IsFloat)
				.SelectMany(mc => mc.AvailableMnemonics)
				.ToDictionary(kvp => kvp.Key, kvp => Source.GetFloat(kvp.Key));
		}

		// TODO: refactor me!
		public Dictionary<string, char> AvailableMnemonics
		{
			get
			{
				var dictionary = new Dictionary<string, char>();
				foreach (var generator in AvailableGenerators)
				{
					foreach (var mnemonic in generator.AvailableMnemonics)
					{
						dictionary.Add(mnemonic.Key, mnemonic.Value);
					}
				}

				return dictionary;
			}
		}
		#endregion

		#endregion

		#region Privates

		private static readonly Dictionary<string, char> _basicController = new Dictionary<string, char>
		{
			{ "Up", 'U' },
			{ "Down", 'D' },
			{ "Left", 'L' },
			{ "Right", 'R' },
			{ "Select", 's' },
			{ "Start", 'S' },
			{ "B", 'B' },
			{ "A", 'A' }
		};

		private readonly BooleanControllerMnemonicGenerator _nesConsoleControls = new BooleanControllerMnemonicGenerator(
			"Console",
			new Dictionary<string, char>
			{
				{ "Reset", 'r' },
				{ "Power", 'P' },
			}
		)
			{
				ControllerPrefix = string.Empty
			};

		private readonly BooleanControllerMnemonicGenerator _fdsConsoleControls = new BooleanControllerMnemonicGenerator(
			"Console",
			new Dictionary<string, char>
			{
				{ "Reset", 'r' },
				{ "Power", 'P' },
				{ "FDS Eject", 'E' },
				{ "FDS Insert 0", '0' },
				{ "FDS Insert 1", '1' },
			}
		)
		{
			ControllerPrefix = string.Empty
		};

		private readonly List<IMnemonicGenerator> _controllerPorts =
			new List<IMnemonicGenerator>
			{
				new BooleanControllerMnemonicGenerator("Player 1", _basicController)
				{
					ControllerPrefix = "P1"
				},
				new BooleanControllerMnemonicGenerator("Player 2", _basicController)
				{
					ControllerPrefix = "P2"
				},
				new BooleanControllerMnemonicGenerator("Player 3", _basicController)
				{
					ControllerPrefix = "P3"
				},
				new BooleanControllerMnemonicGenerator("Player 4", _basicController)
				{
					ControllerPrefix = "P4"
				}
			};

		#endregion
	}
}
