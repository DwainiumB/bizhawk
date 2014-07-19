﻿using System;
using System.Collections.Generic;

using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common
{
	public partial class Bk2Movie : IMovie
	{
		private bool _makeBackup = true;

		public Bk2Movie(string filename)
			: this()
		{
			Rerecords = 0;
			Filename = filename;
		}

		public Bk2Movie()
		{
			Subtitles = new SubtitleList();
			Comments = new List<string>();

			Filename = string.Empty;
			IsCountingRerecords = true;
			_mode = Moviemode.Inactive;
			_makeBackup = true;

			Header[HeaderKeys.MOVIEVERSION] = "BizHawk v2.0.0";
		}

		public string Filename { get; set; }

		public virtual string PreferredExtension { get { return Extension; } }
		public const string Extension = "bk2";

		public bool Changes { get; protected set; }
		public bool IsCountingRerecords { get; set; }

		public ILogEntryGenerator LogGeneratorInstance()
		{
			return new Bk2LogEntryGenerator(_logKey);
		}

		public double FrameCount
		{
			get
			{
				if (LoopOffset.HasValue)
				{
					return double.PositiveInfinity;
				}

				return _log.Count;
			}
		}

		public int InputLogLength
		{
			get { return _log.Count; }
		}

		#region Log Editing

		public void AppendFrame(IController source)
		{
			var lg = LogGeneratorInstance();
			lg.SetSource(source);
			_log.Add(lg.GenerateLogEntry());
			Changes = true;
		}

		public virtual void RecordFrame(int frame, IController source)
		{
			if (Global.Config.VBAStyleMovieLoadState)
			{
				if (Global.Emulator.Frame < _log.Count)
				{
					Truncate(Global.Emulator.Frame);
				}
			}

			var lg = LogGeneratorInstance();
			lg.SetSource(source);
			SetFrameAt(frame, lg.GenerateLogEntry());

			Changes = true;
		}

		public virtual void Truncate(int frame)
		{
			if (frame < _log.Count)
			{
				_log.RemoveRange(frame, _log.Count - frame);
				Changes = true;
			}
		}

		public IController GetInputState(int frame)
		{
			if (frame < FrameCount && frame >= 0)
			{

				int getframe;

				if (LoopOffset.HasValue)
				{
					if (frame < _log.Count)
					{
						getframe = frame;
					}
					else
					{
						getframe = ((frame - LoopOffset.Value) % (_log.Count - LoopOffset.Value)) + LoopOffset.Value;
					}
				}
				else
				{
					getframe = frame;
				}

				var adapter = new Bk2ControllerAdapter
				{
					Type = Global.MovieSession.MovieControllerAdapter.Type
				};

				adapter.SetControllersAsMnemonic(_log[getframe]);
				return adapter;
			}

			return null;
		}

		public virtual void PokeFrame(int frame, IController source)
		{
			var lg = LogGeneratorInstance();
			lg.SetSource(source);

			Changes = true;
			SetFrameAt(frame, lg.GenerateLogEntry());
		}

		public virtual void ClearFrame(int frame)
		{
			var lg = LogGeneratorInstance();
			lg.SetSource(Global.MovieSession.MovieControllerInstance());
			SetFrameAt(frame, lg.EmptyEntry);
			Changes = true;
		}

		#endregion

		private void SetFrameAt(int frameNum, string frame)
		{
			if (_log.Count > frameNum)
			{
				_log[frameNum] = frame;
			}
			else
			{
				_log.Add(frame);
			}
		}
	}
}
