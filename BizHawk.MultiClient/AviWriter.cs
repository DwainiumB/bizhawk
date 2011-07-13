﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using BizHawk;

//some helpful p/invoke from http://www.codeproject.com/KB/audio-video/Motion_Detection.aspx?msg=1142967

namespace BizHawk.MultiClient
{
	class AviWriter : IDisposable
	{
		CodecToken currVideoCodecToken = null;
		AviWriterSegment currSegment;
		IEnumerator<string> nameProvider;

		bool IsOpen { get { return nameProvider != null; } }

		public void Dispose()
		{
			if (currSegment != null)
				currSegment.Dispose();
		}

		/// <summary>
		/// sets the codec token to be used for video compression
		/// </summary>
		public void SetVideoCodecToken(CodecToken token)
		{
			currVideoCodecToken = token;
		}

		public static IEnumerator<string> CreateBasicNameProvider(string template)
		{
			string dir = Path.GetDirectoryName(template);
			string baseName = Path.GetFileNameWithoutExtension(template);
			string ext = Path.GetExtension(template);
			yield return template;
			int counter = 1;
			for (; ; )
			{
				yield return Path.Combine(dir, baseName) + "_" + counter + ext;
				counter++;
			}
		}

		/// <summary>
		/// opens an avi file for recording with names based on the supplied template.
		/// set a video codec token first.
		/// </summary>
		public void OpenFile(string baseName) { OpenFile(CreateBasicNameProvider(baseName)); }

		/// <summary>
		/// opens an avi file for recording with the supplied enumerator used to name files.
		/// set a video codec token first.
		/// </summary>
		/// <param name="nameProvider"></param>
		public void OpenFile(IEnumerator<string> nameProvider)
		{
			this.nameProvider = nameProvider;
			if (currVideoCodecToken == null)
				throw new InvalidOperationException("Tried to start recording an AVI with no video codec token set");
		}

		public void CloseFile()
		{
			if (currSegment != null)
				currSegment.Dispose();
			currSegment = null;
		}

		public void AddFrame(IVideoProvider source)
		{
			SetVideoParameters(source.BufferWidth, source.BufferHeight);
			ConsiderLengthSegment();
			if (currSegment == null) Segment();
			currSegment.AddFrame(source);
		}

		public void AddSamples(short[] samples)
		{
			ConsiderLengthSegment();
			if (currSegment == null) Segment();
			currSegment.AddSamples(samples);
		}

		void ConsiderLengthSegment()
		{
			if(currSegment == null) return;
			long len = currSegment.GetLengthApproximation();
			const long segment_length_limit = 2 * 1000 * 1000 * 1000; //2GB
			//const long segment_length_limit = 10 * 1000 * 1000; //for testing
			if (len > segment_length_limit) Segment();
		}

		void StartRecording()
		{
			//i guess theres nothing to do here
		}

		void Segment()
		{
			if (!IsOpen) return;

			if (currSegment == null)
				StartRecording();
			else
				currSegment.Dispose();
			currSegment = new AviWriterSegment();
			nameProvider.MoveNext();
			currSegment.OpenFile(nameProvider.Current, parameters, currVideoCodecToken);
			currSegment.OpenStreams();
		}

		/// <summary>
		/// Acquires a video codec configuration from the user. you may save it for future use, but you must dispose of it when youre done with it.
		/// returns null if the user canceled the dialog
		/// </summary>
		public static CodecToken AcquireVideoCodecToken(IntPtr hwnd, CodecToken lastToken)
		{
			var temp_params = new Parameters();
			temp_params.height = 256;
			temp_params.width = 256;
			temp_params.fps = 60;
			temp_params.fps_scale = 1;
			temp_params.a_bits = 16;
			temp_params.a_samplerate = 44100;
			temp_params.a_channels = 2;
			var temp = new AviWriterSegment();
			string tempfile = Path.GetTempFileName();
			File.Delete(tempfile);
			tempfile = Path.ChangeExtension(tempfile, "avi");
			temp.OpenFile(tempfile, temp_params, lastToken);
			CodecToken token = temp.AcquireVideoCodecToken(hwnd);
			temp.CloseFile();
			File.Delete(tempfile);
			return token;
		}

		class Parameters
		{
			public int width, height;
			public void PopulateBITMAPINFOHEADER(ref Win32.BITMAPINFOHEADER bmih)
			{
				bmih.Init();
				bmih.biPlanes = 1;
				bmih.biBitCount = 24;
				bmih.biHeight = height;
				bmih.biWidth = width;
				bmih.biSizeImage = (uint)(3 * width * height);
			}

			public bool has_audio;
			public int a_samplerate, a_channels, a_bits;
			public void PopulateWAVEFORMATEX(ref Win32.WAVEFORMATEX wfex)
			{
				int bytes = 0;
				if (a_bits == 16) bytes = 2;
				else if (a_bits == 8) bytes = 1;
				else throw new InvalidOperationException("only 8/16 bits audio are supported by AviWriter and you chose: " + a_bits);
				if (a_channels == 1) { }
				else if (a_channels == 2) { }
				else throw new InvalidOperationException("only 1/2 channels audio are supported by AviWriter and you chose: " + a_channels);

				wfex.Init();
				wfex.nBlockAlign = (ushort)(bytes * a_channels);
				wfex.nChannels = (ushort)a_channels;
				wfex.wBitsPerSample = (ushort)a_bits;
				wfex.wFormatTag = Win32.WAVE_FORMAT_PCM;
				wfex.nSamplesPerSec = (uint)a_samplerate;
				wfex.nAvgBytesPerSec = (uint)(wfex.nBlockAlign * a_samplerate);
			}

			public int fps, fps_scale;
		}
		Parameters parameters = new Parameters();


		/// <summary>
		/// set basic movie timing parameters for the avi file. must be set before the file is opened.
		/// </summary>
		public void SetMovieParameters(int fps, int fps_scale)
		{
			bool change = false;
			
			change |= fps != parameters.fps;
			parameters.fps = fps;

			change |= parameters.fps_scale != fps_scale;
			parameters.fps_scale = fps_scale;

			if (change) Segment();
		}

		/// <summary>
		/// set basic video parameters for the avi file. must be set before the file is opened.
		/// </summary>
		public void SetVideoParameters(int width, int height)
		{
			bool change = false;

			change |= parameters.width != width;
			parameters.width = width;

			change |= parameters.height != height;
			parameters.height = height;

			if (change) Segment();
		}

		/// <summary>
		/// set basic audio parameters for the avi file. must be set before the file isopened.
		/// </summary>
		public void SetAudioParameters(int sampleRate, int channels, int bits)
		{
			bool change = false;

			change |= parameters.a_samplerate != sampleRate;
			parameters.a_samplerate = sampleRate;

			change |= parameters.a_channels != channels;
			parameters.a_channels = channels;

			change |= parameters.a_bits != bits;
			parameters.a_bits = bits;

			change |= parameters.has_audio != true;
			parameters.has_audio = true;

			if (change) Segment();
		}

		public class CodecToken : IDisposable
		{
			~CodecToken()
			{
				Dispose();
			}
			public static CodecToken TakePossession(Win32.AVICOMPRESSOPTIONS comprOptions)
			{
				CodecToken ret = new CodecToken();
				ret.allocated = true;
				ret.comprOptions = comprOptions;
				ret.codec = Win32.decode_mmioFOURCC(comprOptions.fccHandler);
				return ret;
			}
			private CodecToken() { }
			public Win32.AVICOMPRESSOPTIONS comprOptions;
			public string codec;
			bool allocated = false;
			public void Dispose()
			{
				if (!allocated) return;

				IntPtr[] infPtrs = new IntPtr[1];
				IntPtr mem;

				// alloc unmanaged memory 
				mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.AVICOMPRESSOPTIONS)));
				infPtrs[0] = mem;

				// copy from managed structure to unmanaged memory 
				Marshal.StructureToPtr(comprOptions, mem, false);

				Win32.AVISaveOptionsFree(1, infPtrs);
				Marshal.FreeHGlobal(mem);

				codec = null;
				comprOptions = new Win32.AVICOMPRESSOPTIONS();
				allocated = false;
			}
		}

		unsafe class AviWriterSegment : IDisposable
		{
			static AviWriterSegment()
			{
				Win32.AVIFileInit();
			}

			public AviWriterSegment()
			{
			}

			public void Dispose()
			{
				CloseFile();
			}

			CodecToken currVideoCodecToken = null;
			bool IsOpen;
			IntPtr pAviFile, pAviRawVideoStream, pAviRawAudioStream, pAviCompressedVideoStream;
			IntPtr pGlobalBuf;
			int pGlobalBuf_size;

			/// <summary>
			/// there is just ony global buf. this gets it and makes sure its big enough. don't get all re-entrant on it!
			/// </summary>
			IntPtr GetStaticGlobalBuf(int amount)
			{
				if (amount > pGlobalBuf_size)
				{
					if (pGlobalBuf != IntPtr.Zero)
						Marshal.FreeHGlobal(pGlobalBuf);
					pGlobalBuf_size = amount;
					pGlobalBuf = Marshal.AllocHGlobal(pGlobalBuf_size);
				}
				return pGlobalBuf;
			}


			class OutputStatus
			{
				public int video_frames;
				public int video_bytes;
				public int audio_bytes;
				public int audio_samples;
				public int audio_buffered_shorts;
				public const int AUDIO_SEGMENT_SIZE = 44100 * 2;
				public short[] BufferedShorts = new short[AUDIO_SEGMENT_SIZE];
			}
			OutputStatus outStatus;

			public long GetLengthApproximation() { return outStatus.video_bytes + outStatus.audio_bytes; }

			static int AVISaveOptions(IntPtr stream, ref Win32.AVICOMPRESSOPTIONS opts, IntPtr owner)
			{
				IntPtr mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32.AVICOMPRESSOPTIONS)));

				Marshal.StructureToPtr(opts, mem, false);

				IntPtr[] streams = new[] { stream };
				IntPtr[] infPtrs = new[] { mem };

				int ret = Win32.AVISaveOptions(owner, 0, 1, streams, infPtrs);

				opts = (Win32.AVICOMPRESSOPTIONS)Marshal.PtrToStructure(mem, typeof(Win32.AVICOMPRESSOPTIONS));

				Marshal.FreeHGlobal(mem);

				return ret;
			}

			Parameters parameters;
			public void OpenFile(string destPath, Parameters parameters, CodecToken videoCodecToken)
			{
				this.parameters = parameters;
				this.currVideoCodecToken = videoCodecToken;

				//TODO - try creating the file once first before we let vfw botch it up?

				//open the avi output file handle
				if (File.Exists(destPath))
					File.Delete(destPath);

				if (Win32.FAILED(Win32.AVIFileOpenW(ref pAviFile, destPath, Win32.OpenFileStyle.OF_CREATE | Win32.OpenFileStyle.OF_WRITE, 0)))
					throw new InvalidOperationException("Couldnt open dest path for avi file: " + destPath);

				//initialize the video stream
				Win32.AVISTREAMINFOW vidstream_header = new Win32.AVISTREAMINFOW();
				Win32.BITMAPINFOHEADER bmih = new Win32.BITMAPINFOHEADER();
				parameters.PopulateBITMAPINFOHEADER(ref bmih);
				vidstream_header.fccType = Win32.mmioFOURCC("vids");
				vidstream_header.dwRate = parameters.fps;
				vidstream_header.dwScale = parameters.fps_scale;
				vidstream_header.dwSuggestedBufferSize = (int)bmih.biSizeImage;
				if (Win32.FAILED(Win32.AVIFileCreateStreamW(pAviFile, out pAviRawVideoStream, ref vidstream_header)))
				{
					CloseFile();
					throw new InvalidOperationException("Failed opening raw video stream. Not sure how this could happen");
				}

				//initialize audio stream
				Win32.AVISTREAMINFOW audstream_header = new Win32.AVISTREAMINFOW();
				Win32.WAVEFORMATEX wfex = new Win32.WAVEFORMATEX();
				parameters.PopulateWAVEFORMATEX(ref wfex);
				audstream_header.fccType = Win32.mmioFOURCC("auds");
				audstream_header.dwQuality = -1;
				audstream_header.dwScale = wfex.nBlockAlign;
				audstream_header.dwRate = (int)wfex.nAvgBytesPerSec;
				audstream_header.dwSampleSize = wfex.nBlockAlign;
				audstream_header.dwInitialFrames = 1; // ??? optimal value?
				if (Win32.FAILED(Win32.AVIFileCreateStreamW(pAviFile, out pAviRawAudioStream, ref audstream_header)))
				{
					CloseFile();
					throw new InvalidOperationException("Failed opening raw audio stream. Not sure how this could happen");
				}

				outStatus = new OutputStatus();
				IsOpen = true;
			}

			/// <summary>
			/// Acquires a video codec configuration from the user
			/// </summary>
			public CodecToken AcquireVideoCodecToken(IntPtr hwnd)
			{
				if (!IsOpen) throw new InvalidOperationException("File must be opened before acquiring a codec token (or else the stream formats wouldnt be known)");

				//encoder params
				Win32.AVICOMPRESSOPTIONS comprOptions = new Win32.AVICOMPRESSOPTIONS();
				if (currVideoCodecToken != null)
				{
					comprOptions = currVideoCodecToken.comprOptions;
				}
				if (AVISaveOptions(pAviRawVideoStream, ref comprOptions, hwnd) != 0)
					return CodecToken.TakePossession(comprOptions);
				else return null;
			}

			/// <summary>
			/// begin recording
			/// </summary>
			public void OpenStreams()
			{
				if (currVideoCodecToken == null)
					throw new InvalidOperationException("set a video codec token before opening the streams!");

				//open compressed video stream
				if (Win32.FAILED(Win32.AVIMakeCompressedStream(out pAviCompressedVideoStream, pAviRawVideoStream, ref currVideoCodecToken.comprOptions, IntPtr.Zero)))
				{
					CloseStreams();
					throw new InvalidOperationException("Failed making compressed video stream");
				}

				//set the compressed video stream input format
				Win32.BITMAPINFOHEADER bmih = new Win32.BITMAPINFOHEADER();
				parameters.PopulateBITMAPINFOHEADER(ref bmih);
				if (Win32.FAILED(Win32.AVIStreamSetFormat(pAviCompressedVideoStream, 0, ref bmih, Marshal.SizeOf(bmih))))
				{
					CloseStreams();
					throw new InvalidOperationException("Failed setting compressed video stream input format");
				}

				//set audio stream input format
				Win32.WAVEFORMATEX wfex = new Win32.WAVEFORMATEX();
				parameters.PopulateWAVEFORMATEX(ref wfex);
				if (Win32.FAILED(Win32.AVIStreamSetFormat(pAviRawAudioStream, 0, ref wfex, Marshal.SizeOf(wfex))))
				{
					CloseStreams();
					throw new InvalidOperationException("Failed setting raw audio stream input format");
				}
			}

			/// <summary>
			/// wrap up the avi writing
			/// </summary>
			public void CloseFile()
			{
				CloseStreams();
				if (pAviRawAudioStream != IntPtr.Zero)
				{
					Win32.AVIStreamRelease(pAviRawAudioStream);
					pAviRawAudioStream = IntPtr.Zero;
				}
				if (pAviRawVideoStream != IntPtr.Zero)
				{
					Win32.AVIStreamRelease(pAviRawVideoStream);
					pAviRawVideoStream = IntPtr.Zero;
				}
				if (pAviFile != IntPtr.Zero)
				{
					Win32.AVIFileRelease(pAviFile);
					pAviFile = IntPtr.Zero;
				}
				if (pGlobalBuf != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(pGlobalBuf);
					pGlobalBuf = IntPtr.Zero;
					pGlobalBuf_size = 0;
				}
			}

			/// <summary>
			/// end recording
			/// </summary>
			public void CloseStreams()
			{
				FlushBufferedAudio();
				if (pAviCompressedVideoStream != IntPtr.Zero)
				{
					Win32.AVIStreamRelease(pAviCompressedVideoStream);
					pAviCompressedVideoStream = IntPtr.Zero;
				}
			}

			//todo - why couldnt this take an ISoundProvider? it could do the timekeeping as well.. hmm
			public unsafe void AddSamples(short[] samples)
			{
				int todo = samples.Length;
				int idx = 0;
				while (todo > 0)
				{
					int remain = OutputStatus.AUDIO_SEGMENT_SIZE - outStatus.audio_buffered_shorts;
					int chunk = Math.Min(remain, todo);
					for (int i = 0; i < chunk; i++)
						outStatus.BufferedShorts[outStatus.audio_buffered_shorts++] = samples[idx++];
					todo -= chunk;

					if (outStatus.audio_buffered_shorts == OutputStatus.AUDIO_SEGMENT_SIZE)
						FlushBufferedAudio();
				}
			}

			unsafe void FlushBufferedAudio()
			{
				int todo = outStatus.audio_buffered_shorts;
				int todo_realsamples = todo / 2;
				IntPtr buf = GetStaticGlobalBuf(todo * 2);

				short* sptr = (short*)buf.ToPointer();
				for (int i = 0; i < todo; i++)
				{
					sptr[i] = outStatus.BufferedShorts[i];
				}
				//(TODO - inefficient- build directly in a buffer)
				int bytes_written;
				Win32.AVIStreamWrite(pAviRawAudioStream, outStatus.audio_samples, todo_realsamples, buf, todo_realsamples * 4, 0, IntPtr.Zero, out bytes_written);
				outStatus.audio_samples += todo_realsamples;
				outStatus.audio_bytes += bytes_written;
				outStatus.audio_buffered_shorts = 0;
			}

			public unsafe void AddFrame(IVideoProvider source)
			{
				if (parameters.width != source.BufferWidth
					|| parameters.height != source.BufferHeight)
					throw new InvalidOperationException("video buffer changed between start and now");

				int todo = source.BufferHeight * source.BufferWidth;
				int w = source.BufferWidth;
				int h = source.BufferHeight;

				IntPtr buf = GetStaticGlobalBuf(todo * 3);

				int[] buffer = source.GetVideoBuffer();
				fixed (int* buffer_ptr = &buffer[0])
				{
					byte* bytes_ptr = (byte*)buf.ToPointer();
					{
						byte* bp = bytes_ptr;

						for (int idx = w * h - w, y = 0; y < h; y++)
						{
							for (int x = 0; x < w; x++, idx++)
							{
								int r = (buffer[idx] >> 0) & 0xFF;
								int g = (buffer[idx] >> 8) & 0xFF;
								int b = (buffer[idx] >> 16) & 0xFF;
								*bp++ = (byte)r;
								*bp++ = (byte)g;
								*bp++ = (byte)b;
							}
							idx -= w * 2;
						}

						int bytes_written;
						int ret = Win32.AVIStreamWrite(pAviCompressedVideoStream, outStatus.video_frames, 1, new IntPtr(bytes_ptr), todo * 3, Win32.AVIIF_KEYFRAME, IntPtr.Zero, out bytes_written);
						outStatus.video_bytes += bytes_written;
						outStatus.video_frames++;
					}

				}

			}
		}
	}
}

////TEST AVI
//AviWriter aw = new AviWriter();
//aw.SetVideoParameters(256, 256);
//aw.SetMovieParameters(60, 1);
//aw.OpenFile("d:\\bizhawk.avi");
//CreateHandle();
//var token = aw.AcquireVideoCodecToken(Handle);
//aw.SetVideoCodecToken(token);
//aw.OpenStreams();

//for (int i = 0; i < 100; i++)
//{
//    TestVideoProvider video = new TestVideoProvider();
//    Bitmap bmp = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
//    using (Graphics g = Graphics.FromImage(bmp))
//    {
//        g.Clear(Color.Red);
//        using (Font f = new Font(FontFamily.GenericMonospace, 10))
//            g.DrawString(i.ToString(), f, Brushes.Black, 0, 0);
//    }
//    //bmp.Save(string.Format("c:\\dump\\{0}.bmp", i), ImageFormat.Bmp);
//    for (int y = 0, idx = 0; y < 256; y++)
//        for (int x = 0; x < 256; x++)
//            video.buffer[idx++] = bmp.GetPixel(x, y).ToArgb();
//    aw.AddFrame(video);
//}
//aw.CloseStreams();
//aw.CloseFile();
////-----
