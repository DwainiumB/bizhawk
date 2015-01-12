﻿using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

//this rule is not supported correctly: `The first track number can be greater than one, but all track numbers after the first must be sequential.`

namespace BizHawk.Emulation.DiscSystem
{
	public class CUE_Format
	{
		/// <summary>
		/// Generates the CUE file for the provided DiscStructure
		/// </summary>
		public string GenerateCUE_OneBin(DiscStructure structure, CueBinPrefs prefs)
		{
			if (prefs.OneBlobPerTrack) throw new InvalidOperationException("OneBinPerTrack passed to GenerateCUE_OneBin");

			//this generates a single-file cue!!!!!!! dont expect it to generate bin-per-track!
			StringBuilder sb = new StringBuilder();

			foreach (var session in structure.Sessions)
			{
				if (!prefs.SingleSession)
				{
					//dont want to screw around with sessions for now
					sb.AppendFormat("SESSION {0:D2}\n", session.num);
					if (prefs.AnnotateCue) sb.AppendFormat("REM ; session (length={0})\n", session.length_aba);
				}

				foreach (var track in session.Tracks)
				{
					ETrackType trackType = track.TrackType;

					//mutate track type according to our principle of canonicalization 
					if (trackType == ETrackType.Mode1_2048 && prefs.DumpECM)
						trackType = ETrackType.Mode1_2352;

					sb.AppendFormat("  TRACK {0:D2} {1}\n", track.Number, Cue.TrackTypeStringForTrackType(trackType));
					if (prefs.AnnotateCue) sb.AppendFormat("  REM ; track (length={0})\n", track.LengthInSectors);
					
					foreach (var index in track.Indexes)
					{
						//cue+bin has an implicit 150 sector pregap which neither the cue nor the bin has any awareness of
						//except for the baked-in sector addressing.
						//but, if there is an extra-long pregap, we want to reflect it this way
						int lba = index.aba - 150;
						if (lba <= 0 && index.Number == 0 && track.Number == 1)
						{
						}
						//dont emit index 0 when it is the same as index 1, it is illegal for some reason
						else if (index.Number == 0 && index.aba == track.Indexes[1].aba)
						{
							//dont emit index 0 when it is the same as index 1, it confuses some cue parsers
						}
						else
						{
							sb.AppendFormat("    INDEX {0:D2} {1}\n", index.Number, new Timestamp(lba).Value);
						}
					}
				}
			}

			return sb.ToString();
		}
	}

	partial class Disc
	{
		/// <summary>
		/// finds a file in the same directory with an extension alternate to the supplied one.
		/// If two are found, an exception is thrown (later, we may have heuristics to try to acquire the desired content)
		/// TODO - this whole concept could be turned into a gigantic FileResolver class and be way more powerful
		/// </summary>
		string FindAlternateExtensionFile(string path, bool caseSensitive, string baseDir)
		{
			string targetFile = Path.GetFileName(path);
			string targetFragment = Path.GetFileNameWithoutExtension(path);
			var di = new FileInfo(path).Directory;
			
			//if the directory doesnt exist, it may be because it was a full path or something. try an alternate base directory
			if (!di.Exists)
				di = new DirectoryInfo(baseDir);

			var results = new List<FileInfo>();
			foreach (var fi in di.GetFiles())
			{
				//dont acquire cue files...
				if (Path.GetExtension(fi.FullName).ToLower() == ".cue")
					continue;

				string fragment = Path.GetFileNameWithoutExtension(fi.FullName);
				//match files with differing extensions
				int cmp = string.Compare(fragment, targetFragment, !caseSensitive);
				if(cmp != 0)
					//match files with another extension added on (likely to be mygame.bin.ecm)
					cmp = string.Compare(fragment, targetFile, !caseSensitive);
				if (cmp == 0)
					results.Add(fi);

			}
			if(results.Count == 0) throw new DiscReferenceException(path, "Cannot find the specified file");
			if (results.Count > 1) throw new DiscReferenceException(path, "Cannot choose between multiple options");
			return results[0].FullName;
		}

		//cue files can get their data from other sources using this
		readonly Dictionary<string, string> CueFileResolver = new Dictionary<string, string>();

		void FromCueInternal(Cue cue, string cueDir, CueBinPrefs prefs)
		{
			//TODO - add cue directory to CueBinPrefs???? could make things cleaner...

			Structure = new DiscStructure();
			var session = new DiscStructure.Session {num = 1};
			Structure.Sessions.Add(session);
			var pregap_sector = new Sector_Zero();

			int curr_track = 1;

			foreach (var cue_file in cue.Files)
			{
				//structural validation
				if (cue_file.Tracks.Count < 1) throw new Cue.CueBrokenException("`You must specify at least one track per file.`");

				string blobPath = Path.Combine(cueDir, cue_file.Path);

				if (CueFileResolver.ContainsKey(cue_file.Path))
					blobPath = CueFileResolver[cue_file.Path];

				int blob_sectorsize = Cue.BINSectorSizeForTrackType(cue_file.Tracks[0].TrackType);
				int blob_length_aba;
				long blob_length_bytes;
				IBlob cue_blob;

				//try any way we can to acquire a file
				if (!File.Exists(blobPath) && prefs.ExtensionAware)
				{
					blobPath = FindAlternateExtensionFile(blobPath, prefs.CaseSensitive, cueDir);
				}

				if (!File.Exists(blobPath))
					throw new DiscReferenceException(blobPath, "");

				//some simple rules to mutate the file type if we received something fishy
				string blobPathExt = Path.GetExtension(blobPath).ToLower();
				if (blobPathExt == ".ape") cue_file.FileType = Cue.CueFileType.Wave;
				if (blobPathExt == ".mp3") cue_file.FileType = Cue.CueFileType.Wave;
				if (blobPathExt == ".mpc") cue_file.FileType = Cue.CueFileType.Wave;
				if (blobPathExt == ".flac") cue_file.FileType = Cue.CueFileType.Wave;
				if (blobPathExt == ".ecm") cue_file.FileType = Cue.CueFileType.ECM;

				if (cue_file.FileType == Cue.CueFileType.Binary || cue_file.FileType == Cue.CueFileType.Unspecified)
				{
					//make a blob for the file
					Blob_RawFile blob = new Blob_RawFile {PhysicalPath = blobPath};
					Blobs.Add(blob);

					blob_length_aba = (int)(blob.Length / blob_sectorsize);
					blob_length_bytes = blob.Length;
					cue_blob = blob;
				}
				else if (cue_file.FileType == Cue.CueFileType.ECM)
				{
					if (!Blob_ECM.IsECM(blobPath))
					{
						throw new DiscReferenceException(blobPath, "an ECM file was specified or detected, but it isn't a valid ECM file. You've got issues. Consult your iso vendor.");
					}
					Blob_ECM blob = new Blob_ECM();
					Blobs.Add(blob);
					blob.Parse(blobPath);
					cue_blob = blob;
					blob_length_aba = (int)(blob.Length / blob_sectorsize);
					blob_length_bytes = blob.Length;
				}
				else if (cue_file.FileType == Cue.CueFileType.Wave)
				{
					Blob_WaveFile blob = new Blob_WaveFile();
					Blobs.Add(blob);

					try
					{
						//check whether we can load the wav directly
						bool loaded = false;
						if (File.Exists(blobPath) && Path.GetExtension(blobPath).ToUpper() == ".WAV")
						{
							try
							{
								blob.Load(blobPath);
								loaded = true;
							}
							catch
							{
							}
						}

						//if that didnt work or wasnt possible, try loading it through ffmpeg
						if (!loaded)
						{
							FFMpeg ffmpeg = new FFMpeg();
							if (!ffmpeg.QueryServiceAvailable())
							{
								throw new DiscReferenceException(blobPath, "No decoding service was available (make sure ffmpeg.exe is available. even though this may be a wav, ffmpeg is used to load oddly formatted wave files. If you object to this, please send us a note and we'll see what we can do. It shouldn't be too hard.)");
							}
							AudioDecoder dec = new AudioDecoder();
							byte[] buf = dec.AcquireWaveData(blobPath);
							blob.Load(new MemoryStream(buf));
							WasSlowLoad = true;
						}
					}
					catch (Exception ex)
					{
						throw new DiscReferenceException(blobPath, ex);
					}

					blob_length_aba = (int)(blob.Length / blob_sectorsize);
					blob_length_bytes = blob.Length;
					cue_blob = blob;
				}
				else throw new Exception("Internal error - Unhandled cue blob type");

				//TODO - make CueTimestamp better, and also make it a struct, and also just make it DiscTimestamp

				//start timekeeping for the blob. every time we hit an index, this will advance
				int blob_timestamp = 0;

				//because we can have different size sectors in a blob, we need to keep a file cursor within the blob
				long blob_cursor = 0;

				//the aba that this cue blob starts on
				int blob_disc_aba_start = Sectors.Count;


				//this is a bit dodgy.. lets fixup the indices so we have something for index 0
				//TODO - I WISH WE DIDNT HAVE TO DO THIS. WE SHOULDNT PAY SO MUCH ATTENTION TO THE INTEGRITY OF THE INDEXES
				Timestamp blob_ts = new Timestamp(0);
				for (int t = 0; t < cue_file.Tracks.Count; t++)
				{
					var cue_track = cue_file.Tracks[t];
					if (!cue_track.Indexes.ContainsKey(1))
						throw new Cue.CueBrokenException("Track was missing an index 01");
					for (int i = 0; i <= 99; i++)
					{
						if (cue_track.Indexes.ContainsKey(i))
						{
							blob_ts = cue_track.Indexes[i].Timestamp;
						}
						else if (i == 0)
						{
							var cti = new Cue.CueTrackIndex(0);
							cue_track.Indexes[0] = cti;
							cti.Timestamp = blob_ts;
						}
					}
				}

				//validate that the first index in the file is 00:00:00
				//"The first index of a file must start at 00:00:00"
				//zero 20-dec-2014 - NOTE - index 0 is OK. we've seen files that 'start' at non-zero but thats only with index 1 -- an index 0 was explicitly listed at time 0
				if (cue_file.Tracks[0].Indexes[0].Timestamp.Sector != 0) throw new Cue.CueBrokenException("`The first index of a blob must start at 00:00:00.`");

				//for each track within the file:
				for (int t = 0; t < cue_file.Tracks.Count; t++)
				{
					var cue_track = cue_file.Tracks[t];

					//record the disc ABA that this track started on
					int track_disc_aba_start = Sectors.Count;

					//record the pregap location. it will default to the start of the track unless we supplied a pregap command
					int track_disc_pregap_aba = track_disc_aba_start;

					int blob_track_start = blob_timestamp;

					//once upon a time we had a check here to prevent a single blob from containing variant sector sizes. but we support that now.

					//check integrity of track sequence and setup data structures
					//TODO - check for skipped tracks in cue parser instead
					if (cue_track.TrackNum != curr_track) throw new Cue.CueBrokenException("Found a cue with skipped tracks");
					var toc_track = new DiscStructure.Track();
					session.Tracks.Add(toc_track);
					toc_track.Number = curr_track;
					toc_track.TrackType = cue_track.TrackType;
					toc_track.ADR = 1; //safe assumption. CUE can't store this.

					//choose a Control value based on track type and other flags from cue
					//TODO - this might need to be controlled by cue loading prefs
					toc_track.Control = cue_track.Control;
					if (toc_track.TrackType == ETrackType.Audio)
						toc_track.Control |= EControlQ.StereoNoPreEmph;
					else toc_track.Control |= EControlQ.DataUninterrupted;

					if (curr_track == 1)
					{
						if (cue_track.PreGap.Sector != 0)
							throw new InvalidOperationException("not supported (yet): cue files with track 1 pregaps");
						//but now we add one anyway, because every existing cue+bin seems to implicitly specify this
						cue_track.PreGap = new Timestamp(150);
					}

					//check whether a pregap is requested.
					//this causes empty sectors to get generated without consuming data from the blob
					if (cue_track.PreGap.Sector > 0)
					{
						for (int i = 0; i < cue_track.PreGap.Sector; i++)
						{
							Sectors.Add(new SectorEntry(pregap_sector));
						}
					}

					//look ahead to the next track's index 1 so we can see how long this track's last index is
					//or, for the last track, use the length of the file
					int track_length_aba;
					if (t == cue_file.Tracks.Count - 1)
						track_length_aba = blob_length_aba - blob_timestamp;
					else track_length_aba = cue_file.Tracks[t + 1].Indexes[1].Timestamp.Sector - blob_timestamp;
					//toc_track.length_aba = track_length_aba; //xxx

					//find out how many indexes we have
					int num_indexes = 0;
					for (num_indexes = 0; num_indexes <= 99; num_indexes++)
						if (!cue_track.Indexes.ContainsKey(num_indexes)) break;

					//for each index, calculate length of index and then emit it
					for (int index = 0; index < num_indexes; index++)
					{
						bool is_last_index = index == num_indexes - 1;

						//install index into hierarchy
						var toc_index = new DiscStructure.Index {Number = index};
						toc_track.Indexes.Add(toc_index);
						if (index == 0)
						{
							//zero 18-dec-2014 - uhhhh cant make sense of this.
							//toc_index.aba = track_disc_pregap_aba - (cue_track.Indexes[1].Timestamp.Sector - cue_track.Indexes[0].Timestamp.Sector);
							toc_index.aba = track_disc_pregap_aba;
						}
						else toc_index.aba = Sectors.Count;

						//calculate length of the index
						//if it is the last index then we use our calculation from before, otherwise we check the next index
						int index_length_aba;
						if (is_last_index)
							index_length_aba = track_length_aba - (blob_timestamp - blob_track_start);
						else index_length_aba = cue_track.Indexes[index + 1].Timestamp.Sector - blob_timestamp;

						//emit sectors
						for (int aba = 0; aba < index_length_aba; aba++)
						{
							bool is_last_aba_in_index = (aba == index_length_aba - 1);
							bool is_last_aba_in_track = is_last_aba_in_index && is_last_index;

							switch (cue_track.TrackType)
							{
								case ETrackType.Audio:  //all 2352 bytes are present
								case ETrackType.Mode1_2352: //2352 bytes are present, containing 2048 bytes of user data as well as ECM
								case ETrackType.Mode2_2352: //2352 bytes are present, containing the entirety of a mode2 sector (could be form0,1,2)
									{
										//these cases are all 2352 bytes
										//in all these cases, either no ECM is present or ECM is provided.
										//so we just emit a Sector_Raw
										Sector_RawBlob sector_rawblob = new Sector_RawBlob
											{
												Blob = cue_blob,
												Offset = blob_cursor
											};
										blob_cursor += 2352;
										Sector_Mode1_or_Mode2_2352 sector_raw;
										if(cue_track.TrackType == ETrackType.Mode1_2352)
											sector_raw  = new Sector_Mode1_2352();
										else if (cue_track.TrackType == ETrackType.Audio)
											sector_raw = new Sector_Mode1_2352(); //TODO should probably make a new sector adapter which errors if 2048B are requested
										else if (cue_track.TrackType == ETrackType.Mode2_2352)
											sector_raw = new Sector_Mode2_2352();
										else throw new InvalidOperationException();

										sector_raw.BaseSector = sector_rawblob;

										Sectors.Add(new SectorEntry(sector_raw));
										break;
									}
								case ETrackType.Mode1_2048:
									//2048 bytes are present. ECM needs to be generated to create a full sector
									{
										//ECM needs to know the sector number so we have to record that here
										int curr_disc_aba = Sectors.Count;
										var sector_2048 = new Sector_Mode1_2048(curr_disc_aba + 150)
											{
												Blob = new ECMCacheBlob(cue_blob),
												Offset = blob_cursor
											};
										blob_cursor += 2048;
										Sectors.Add(new SectorEntry(sector_2048));
										break;
									}
							} //switch(TrackType)

							//we've emitted an ABA, so consume it from the blob
							blob_timestamp++;

						} //aba emit loop

					} //index loop

					//check whether a postgap is requested. if it is, we need to generate silent sectors
					for (int i = 0; i < cue_track.PostGap.Sector; i++)
					{
						Sectors.Add(new SectorEntry(pregap_sector));
					}

					//we're done with the track now.
					//record its length:
					toc_track.LengthInSectors = Sectors.Count - toc_track.Indexes[1].aba;
					curr_track++;

					//if we ran off the end of the blob, pad it with zeroes, I guess
					if (blob_cursor > blob_length_bytes)
					{
						//mutate the blob to an encapsulating Blob_ZeroPadAdapter
						Blobs[Blobs.Count - 1] = new Blob_ZeroPadAdapter(Blobs[Blobs.Count - 1], blob_length_bytes, blob_cursor - blob_length_bytes);
					}

				} //track loop
			} //file loop

			//finally, analyze the length of the sessions and the entire disc by summing the lengths of the tracks
			//this is a little more complex than it looks, because the length of a thing is not determined by summing it
			//but rather by the difference in abas between start and end
			//EDIT - or is the above nonsense? it should be the amount of data present, full stop.
			Structure.LengthInSectors = 0;
			foreach (var toc_session in Structure.Sessions)
			{
				var firstTrack = toc_session.Tracks[0];

				//track 0, index 0 is actually -150. but cue sheets will never say that
				//firstTrack.Indexes[0].aba -= 150;

				var lastTrack = toc_session.Tracks[toc_session.Tracks.Count - 1];
				session.length_aba = lastTrack.Indexes[1].aba + lastTrack.LengthInSectors - firstTrack.Indexes[0].aba;
				Structure.LengthInSectors += toc_session.length_aba;
			}
		}

		void FromCuePathInternal(string cuePath, CueBinPrefs prefs)
		{
			string cueDir = Path.GetDirectoryName(cuePath);
			var cue = new Cue();
			cue.LoadFromPath(cuePath);
			FromCueInternal(cue, cueDir, prefs);
		}
	}

	public class Cue
	{
		//TODO - export from isobuster and observe the SESSION directive, as well as the MSF directive.

		public string DebugPrint()
		{
			StringBuilder sb = new StringBuilder();
			foreach (CueFile cf in Files)
			{
				sb.AppendFormat("FILE \"{0}\"", cf.Path);
				if (cf.FileType == CueFileType.Binary) sb.Append(" BINARY");
				if (cf.FileType == CueFileType.Wave) sb.Append(" WAVE");
				sb.AppendLine();
				foreach (CueTrack ct in cf.Tracks)
				{
					sb.AppendFormat("  TRACK {0:D2} {1}\n", ct.TrackNum, ct.TrackType.ToString().Replace("_", "/").ToUpper());
					foreach (CueTrackIndex cti in ct.Indexes.Values)
					{
						sb.AppendFormat("    INDEX {0:D2} {1}\n", cti.IndexNum, cti.Timestamp.Value);
					}
				}
			}

			return sb.ToString();
		}

		public enum CueFileType
		{
			Unspecified, Binary, Wave, ECM
		}

		public class CueFile
		{
			public string Path;
			public List<CueTrack> Tracks = new List<CueTrack>();

			public CueFileType FileType = CueFileType.Unspecified;
			public string StrFileType;
		}

		public List<CueFile> Files = new List<CueFile>();

		public static int BINSectorSizeForTrackType(ETrackType type)
		{
			switch (type)
			{
				case ETrackType.Mode1_2352:
				case ETrackType.Mode2_2352:
				case ETrackType.Audio:
					return 2352;
				case ETrackType.Mode1_2048:
					return 2048;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static string TrackTypeStringForTrackType(ETrackType type)
		{
			switch (type)
			{
				case ETrackType.Mode1_2352: return "MODE1/2352";
				case ETrackType.Mode2_2352: return "MODE2/2352";
				case ETrackType.Audio: return "AUDIO";
				case ETrackType.Mode1_2048: return "MODE1/2048";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static string RedumpTypeStringForTrackType(ETrackType type)
		{
			switch (type)
			{
				case ETrackType.Mode1_2352: return "Data/Mode 1";
				case ETrackType.Mode1_2048: throw new InvalidOperationException("guh dunno what to put here");
				case ETrackType.Mode2_2352: return "Data/Mode 2";
				case ETrackType.Audio: return "Audio";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public class CueTrack
		{
			public EControlQ Control;
			public ETrackType TrackType;
			public int TrackNum;
			public Timestamp PreGap = new Timestamp();
			public Timestamp PostGap = new Timestamp();
			public Dictionary<int, CueTrackIndex> Indexes = new Dictionary<int, CueTrackIndex>();
		}

		public class CueTrackIndex
		{
			public CueTrackIndex(int num) { IndexNum = num; }
			public int IndexNum;

			/// <summary>
			/// Is this an ABA or a LBA? please say.
			/// </summary>
			public Timestamp Timestamp;
		}

		[Serializable]
		public class CueBrokenException : Exception
		{
			public CueBrokenException(string why)
				: base(why)
			{
			}
		}

		public void LoadFromPath(string cuePath)
		{
			FileInfo fiCue = new FileInfo(cuePath);
			if (!fiCue.Exists) throw new FileNotFoundException();
			string cueString = File.ReadAllText(cuePath);
			LoadFromString(cueString);
		}

		public void LoadFromString(string cueString)
		{
			TextReader tr = new StringReader(cueString);

			bool track_has_pregap = false;
			bool track_has_postgap = false;
			int last_index_num = -1;
			CueFile currFile = null;
			CueTrack currTrack = null;
			for (; ; )
			{
				string line = tr.ReadLine();
				if (line == null) break;
				line = line.Trim();
				if (line == "") continue;
				var clp = new CueLineParser(line);

				string key = clp.ReadToken().ToUpper();
				switch (key)
				{
					case "REM":
						break;

					case "FILE":
						{
							currTrack = null;
							currFile = new CueFile();
							Files.Add(currFile);
							currFile.Path = clp.ReadPath().Trim('"');
							if (!clp.EOF)
							{
								string temp = clp.ReadToken().ToUpper();
								switch (temp)
								{
									case "BINARY":
										currFile.FileType = CueFileType.Binary;
										break;
									case "WAVE":
									case "MP3":
										currFile.FileType = CueFileType.Wave;
										break;
								}
								currFile.StrFileType = temp;
							}
							break;
						}
					case "TRACK":
						{
							if (currFile == null) throw new CueBrokenException("invalid cue structure");
							if (clp.EOF) throw new CueBrokenException("invalid cue structure");
							string strtracknum = clp.ReadToken();
							int tracknum;
							if (!int.TryParse(strtracknum, out tracknum))
								throw new CueBrokenException("malformed track number");
							if (clp.EOF) throw new CueBrokenException("invalid cue structure");
							if (tracknum < 0 || tracknum > 99) throw new CueBrokenException("`All track numbers must be between 1 and 99 inclusive.`");
							string strtracktype = clp.ReadToken().ToUpper();
							currTrack = new CueTrack();
							switch (strtracktype)
							{
								case "MODE1/2352": currTrack.TrackType = ETrackType.Mode1_2352; break;
								case "MODE1/2048": currTrack.TrackType = ETrackType.Mode1_2048; break;
								case "MODE2/2352": currTrack.TrackType = ETrackType.Mode2_2352; break;
								case "AUDIO": currTrack.TrackType = ETrackType.Audio; break;
								default:
									throw new CueBrokenException("unhandled track type");
							}
							currTrack.TrackNum = tracknum;
							currFile.Tracks.Add(currTrack);
							track_has_pregap = false;
							track_has_postgap = false;
							last_index_num = -1;
							break;
						}
					case "INDEX":
						{
							if (currTrack == null) throw new CueBrokenException("invalid cue structure");
							if (clp.EOF) throw new CueBrokenException("invalid cue structure");
							if (track_has_postgap) throw new CueBrokenException("`The POSTGAP command must appear after all INDEX commands for the current track.`");
							string strindexnum = clp.ReadToken();
							int indexnum;
							if (!int.TryParse(strindexnum, out indexnum))
								throw new CueBrokenException("malformed index number");
							if (clp.EOF) throw new CueBrokenException("invalid cue structure (missing index timestamp)");
							string str_timestamp = clp.ReadToken();
							if (indexnum < 0 || indexnum > 99) throw new CueBrokenException("`All index numbers must be between 0 and 99 inclusive.`");
							if (indexnum != 1 && indexnum != last_index_num + 1) throw new CueBrokenException("`The first index must be 0 or 1 with all other indexes being sequential to the first one.`");
							last_index_num = indexnum;
							CueTrackIndex cti = new CueTrackIndex(indexnum)
								{
									Timestamp = new Timestamp(str_timestamp), IndexNum = indexnum
								};
							currTrack.Indexes[indexnum] = cti;
							break;
						}
					case "PREGAP":
						if (track_has_pregap) throw new CueBrokenException("`Only one PREGAP command is allowed per track.`");
						if (currTrack.Indexes.Count > 0) throw new CueBrokenException("`The PREGAP command must appear after a TRACK command, but before any INDEX commands.`");
						currTrack.PreGap = new Timestamp(clp.ReadToken());
						track_has_pregap = true;
						break;
					case "POSTGAP":
						if (track_has_postgap) throw new CueBrokenException("`Only one POSTGAP command is allowed per track.`");
						track_has_postgap = true;
						currTrack.PostGap = new Timestamp(clp.ReadToken());
						break;
					case "CATALOG":
					case "PERFORMER":
					case "SONGWRITER":
					case "TITLE":
					case "ISRC":
					case "FLAGS":
						//TODO - keep these for later?
						//known flags:
						//FLAGS DCP
						{
							var flags = clp.ReadToken();
							if (flags == "DCP")
							{
								currTrack.Control |= EControlQ.CopyPermittedMask;
							} else throw new CueBrokenException("Unknown flags: " + flags);
						}
						break;
					default:
						throw new CueBrokenException("unsupported cue command: " + key);
				}
			} //end cue parsing loop
		}


		class CueLineParser
		{
			int index;
			string str;
			public bool EOF;
			public CueLineParser(string line)
			{
				str = line;
			}

			public string ReadPath() { return ReadToken(true); }
			public string ReadToken() { return ReadToken(false); }

			public string ReadToken(bool isPath)
			{
				if (EOF) return null;

				int startIndex = index;
				bool inToken = false;
				bool inQuote = false;
				for (; ; )
				{
					bool done = false;
					char c = str[index];
					bool isWhiteSpace = (c == ' ' || c == '\t');

					if (isWhiteSpace)
					{
						if (inQuote)
							index++;
						else
						{
							if (inToken)
								done = true;
							else
								index++;
						}
					}
					else
					{
						bool startedQuote = false;
						if (!inToken)
						{
							startIndex = index;
							if (isPath && c == '"')
								startedQuote = inQuote = true;
							inToken = true;
						}
						switch (str[index])
						{
							case '"':
								index++;
								if (inQuote && !startedQuote)
								{
									done = true;
								}
								break;
							case '\\':
								index++;
								break;

							default:
								index++;
								break;
						}
					}
					if (index == str.Length)
					{
						EOF = true;
						done = true;
					}
					if (done) break;
				}

				return str.Substring(startIndex, index - startIndex);
			}

		}
	}
}