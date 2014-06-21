﻿using System;
using System.IO;

using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common
{
	// TODO: message callback / event handler
	// TODO: consider other event handlers, switching modes?
	public interface IMovie
	{
		#region Status

		bool IsCountingRerecords { get; set; }
		bool IsActive { get; }
		bool IsPlaying { get; }
		bool IsRecording { get; }
		bool IsFinished { get; }
		bool Changes { get; }

		#endregion

		#region Properties

		/// <summary>
		/// Gets the total number of frames that count towards the completion time of the movie
		/// Possibly (but unlikely different from InputLogLength (could be infinity, or maybe an implementation automatically discounts empty frames at the end of a movie, etc)
		/// </summary>
		double FrameCount { get; }
		
		/// <summary>
		/// Gets the Fps used to calculate the time of the movie
		/// </summary>
		double Fps { get; }

		/// <summary>
		/// Gets the time calculation based on FrameCount and Fps
		/// </summary>
		TimeSpan Time { get; }

		/// <summary>
		/// Gets the actual length of the input log, should only be used by code that iterates or needs a real length
		/// </summary>
		int InputLogLength { get; }

		IMovieHeader Header { get; }

		#endregion

		#region File Handling API

		// Filename of the movie, settable by the client
		string Filename { get; set; }

		/// <summary>
		/// Tells the movie to load the contents of Filename
		/// </summary>
		/// <returns>Return whether or not the file was successfully loaded</returns>
		bool Load();

		/// <summary>
		/// Instructs the movie to save the current contents to Filename
		/// </summary>
		void Save();
		
		/// <summary>
		/// Extracts the current input log from the user.  
		/// This is provided as the means for putting the input log into savestates,
		/// for the purpose of out of order savestate loading (known as "bullet-proof rerecording")
		/// </summary>
		/// <returns>returns a string represntation of the input log in its current state</returns>
		string GetInputLog();

		/// <summary>
		/// Compares the input log inside reader with the movie's current input to see if the reader's input belongs to the same timeline,
		/// in other words, if reader's input is completely contained in the movie's input, then it is considered in the same timeline
		/// </summary>
		/// <param name="reader">The reader containing the contents of the input log</param>
		/// <param name="errorMessage">Returns an error message, if any</param>
		/// <returns>Returns whether or not the input log in reader is in the same timeline as the movie</returns>
		bool CheckTimeLines(TextReader reader, out string errorMessage);
		
		/// <summary>
		/// Takes reader and extracts the input log, then replaces the movies input log with it
		/// </summary>
		/// <param name="reader">The reader containing the contents of the input log</param>
		/// <param name="errorMessage">Returns an error message, if any</param>
		/// <returns></returns>
		bool ExtractInputLog(TextReader reader, out string errorMessage);

		#endregion

		#region Mode Handling API

		/// <summary>
		/// Tells the movie to start recording from the beginning.
		/// This will clear SRAM, and the movie log
		/// </summary>
		void StartNewRecording();

		/// <summary>
		/// Tells the movie to start playback from the beginning
		/// This will clear SRAM
		/// </summary>
		void StartNewPlayback();

		/// <summary>
		/// Sets the movie to inactive (note that it will still be in memory)
		/// The saveChanges flag will tell the movie to save its contents to disk
		/// </summary>
		/// <param name="saveChanges">if true, will save to disk</param>
		void Stop(bool saveChanges = true);

		/// <summary>
		/// Switches to record mode
		/// Does not change the movie log or clear SRAM
		/// </summary>
		void SwitchToRecord();

		/// <summary>
		/// Switches to playback mode
		/// Does not change the movie log or clear SRAM
		/// </summary>
		void SwitchToPlay();

		#endregion

		#region Editing API

		/// <summary>
		/// Replaces the given frame's input with an empty frame
		/// </summary>
		void ClearFrame(int frame);
		
		/// <summary>
		/// Adds the given input to the movie
		/// Note: this edits the input log without the normal movie recording logic applied
		/// </summary>
		void AppendFrame(IController source);

		/// <summary>
		/// Replaces the input at the given frame with the given input
		/// Note: this edits the input log without the normal movie recording logic applied
		/// </summary>
		void PokeFrame(int frame, IController source);

		/// <summary>
		/// Records the given input into the given frame,
		/// This is subject to normal movie recording logic
		/// </summary>
		void RecordFrame(int frame, IController source);

		/// <summary>
		/// Instructs the movie to remove all input from its input log after frame,
		/// AFter truncating, frame will be the last frame of input in the movie's input log
		/// </summary>
		/// <param name="frame">The frame at which to truncate</param>
		void Truncate(int frame);

		/// <summary>
		/// Gets a single frame of input from the movie at the given frame
		/// The input will be in the same format as represented in the input log when saved as a file
		/// </summary>
		/// <param name="frame">The frame of input to be retrieved</param>
		/// <returns></returns>
		string GetInput(int frame);

		#endregion
	}
}
