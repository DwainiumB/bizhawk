﻿using System;
using System.Text;
using System.Collections.Generic;

namespace BizHawk.Emulation.DiscSystem
{
	/// <summary>
	/// Contains structural information for the disc broken down into c# data structures for easy interrogation.
	/// This represents a best-effort interpretation of the raw disc image.
	/// You cannot assume a disc can be round-tripped into its original format through this (especially if it came from a format more detailed)
	/// </summary>
	public class DiscStructure
	{
		/// <summary>
		/// Right now support for anything other than 1 session is totally not working
		/// </summary>
		public List<Session> Sessions = new List<Session>();

		/// <summary>
		/// List of Points described by the TOC.
		/// TODO - this is kind of garbage, but... I was using it for Synthesize_SubcodeFromStructure() :/
		/// Really, what it is, is a series of points where the tno/index change. Kind of an agenda. And thats how that function uses it.
		/// Maybe I should rename it something different, or at least comment it
		/// Or, you could look at this as a kind of compressed disc map
		/// </summary>
		public List<TOCPoint> Points;

		/// <summary>
		/// How many sectors in the disc, including the 150 lead-in sectors
		/// </summary>
		public int LengthInSectors;

		/// <summary>
		/// Length (including lead-in) of the disc as a timestamp
		/// </summary>
		public Timestamp FriendlyLength { get { return new Timestamp(LengthInSectors); } }

		/// <summary>
		/// How many bytes of data in the disc (including lead-in). Disc sectors are really 2352 bytes each, so this is LengthInSectors * 2352
		/// </summary>
		public long BinarySize
		{
			get { return LengthInSectors * 2352; }
		}

		/// <summary>
		/// Synthesizes the DiscStructure from a DiscTOCRaw
		/// </summary>
		public class SynthesizeFromDiscTOCRawJob
		{
			public DiscTOCRaw TOCRaw;
			public DiscStructure Result;

			public void Run()
			{
				Result = new DiscStructure();
				Result.Sessions.Add(new Session());
				Track lastTrack = null;
				for (int tnum = TOCRaw.FirstRecordedTrackNumber; tnum <= TOCRaw.LastRecordedTrackNumber; tnum++)
				{
					var ti = TOCRaw.TOCItems[tnum];
					var track = new Track();
					Result.Sessions[0].Tracks.Add(track);
					track.Number = tnum;
					track.Control = ti.Control;
					track.TrackType = ETrackType.Unknown; //not known yet
					track.Start_ABA = ti.LBATimestamp.Sector + 150;
					if (lastTrack != null)
					{
						lastTrack.LengthInSectors = track.Start_ABA - lastTrack.Start_ABA;
					}
					lastTrack = track;
				}
				if(lastTrack != null)
					lastTrack.LengthInSectors = (TOCRaw.LeadoutTimestamp.Sector + 150) - lastTrack.Start_ABA;

				//total length of the disc is counted up to the leadout, including the lead-in.
				//i guess supporting storing the leadout is future work.
				Result.LengthInSectors = TOCRaw.LeadoutTimestamp.Sector;
			}
		}

		/// <summary>
		/// seeks the point immediately before (or equal to) this LBA
		/// </summary>
		public TOCPoint SeekPoint(int lba)
		{
			int aba = lba + 150;
			for(int i=0;i<Points.Count;i++)
			{
				TOCPoint tp = Points[i];
				if (tp.ABA > aba)
					return Points[i - 1];
			}
			return Points[Points.Count - 1];
		}


		/// <summary>
		/// 
		/// </summary>
		public class TOCPoint
		{
			public int Num;
			public int ABA, TrackNum, IndexNum;
			public Track Track;

			public int ADR;
			public EControlQ Control;

			public int LBA
			{
				get { return ABA - 150; }
			}
		}

		/// <summary>
		/// Generates the Points list from the current logical TOC
		/// </summary>
		public void Synthesize_TOCPointsFromSessions()
		{
			Points = new List<TOCPoint>();

			int num = 0;
			foreach (var ses in Sessions)
			{
				for(int t=0;t<ses.Tracks.Count;t++)
				{
					int tnum = t + 1;
					var track = ses.Tracks[t];
					for(int i=0;i<track.Indexes.Count;i++)
					{
						var index = track.Indexes[i];
						bool repeat = false;
						int aba = index.aba;
					REPEAT:
						var tp = new TOCPoint
						{
							Num = num++,
							ABA = aba,
							TrackNum = track.Number,
							IndexNum = index.Number,
							Track = track,
							ADR = track.ADR,
							Control = track.Control
						};

						//special case!
						//yellow-book says:
						//pre-gap for "first part of a digital data track not containing user data and encoded as a pause"
						//first interval: at least 75 sectors coded as preceding track
						//second interval: at least 150 sectors coded as user data track.
						//TODO - add pause flag tracking to TOCPoint
						//see mednafen's "TODO: Look into how we're supposed to handle subq control field in the four combinations of track types(data/audio)."
						if (tnum != 1 && i == 0 && track.TrackType != ETrackType.Audio && !repeat)
						{
							//NOTE: we dont implement this exactly the same as mednafen, I think my logic is closer to the docs, but who knows, its complicated
							int distance = track.Indexes[i + 1].aba - track.Indexes[i].aba;
							//well, how do we know to apply this logic?
							//we assume the 150 sector pregap is more important. so if thats all there is, theres no 75 sector pregap like the old track
							//if theres a longer pregap, then we generate weird old track pregap to contain the rest.
							if (distance > 150)
							{
								int weirdPregapSize = distance - 150;

								//need a new point. fix the old one
								tp.ADR = Points[Points.Count - 1].ADR;
								tp.Control = Points[Points.Count - 1].Control;
								Points.Add(tp);

								aba += weirdPregapSize;
								repeat = true;
								goto REPEAT;
							}
						}


						Points.Add(tp);
					}
				}

				var tpLeadout = new TOCPoint();
				var lastTrack = ses.Tracks[ses.Tracks.Count - 1];
				tpLeadout.Num = num++;
				tpLeadout.ABA = lastTrack.Indexes[1].aba + lastTrack.LengthInSectors;
				tpLeadout.IndexNum = 0;
				tpLeadout.TrackNum = 100;
				tpLeadout.Track = null; //no leadout track.. now... or ever?
				Points.Add(tpLeadout);
			}
		}

		public class Session
		{
			public int num;

			/// <summary>
			/// All the tracks in the session.. but... Tracks[0] should be "Track 1". So beware of this.
			/// We might should keep this organized as a dictionary as well.
			/// </summary>
			public List<Track> Tracks = new List<Track>();

			//the length of the session (should be the sum of all track lengths)
			public int length_aba;
			public Timestamp FriendlyLength { get { return new Timestamp(length_aba); } }
		}

		public class Track
		{
			/// <summary>
			/// The number of the track (1-indexed)
			/// </summary>
			public int Number;

			/// <summary>
			/// Conceptual track type, not necessarily stored this way anywhere
			/// </summary>
			public ETrackType TrackType;

			/// <summary>
			/// The 'control' properties of the track indicated by the subchannel Q.
			/// While in principle these could vary during the track, illegally (or maybe legally according to weird redbook rules)
			/// they normally don't; they're useful for describing what type of contents the track is.
			/// </summary>
			public EControlQ Control;

			/// <summary>
			/// Well, it seems a track can have an ADR property (used to fill the subchannel Q). This is delivered from a CCD file but may have to be guessed from 
			/// </summary>
			public int ADR = 1;

			/// <summary>
			/// All the indexes related to the track. These will be 0-Indexed, but they could be non-consecutive.
			/// </summary>
			public List<Index> Indexes = new List<Index>();

			/// <summary>
			/// a track logically starts at index 1. 
			/// so this is the length from this index 1 to the next index 1 (or the end of the disc)
			/// the time before track 1 index 1 is the lead-in and isn't accounted for in any track...
			/// </summary>
			public int LengthInSectors;

			/// <summary>
			/// The beginning ABA of the track (index 1). This isn't well-supported, yet
			/// WHAT? IS THIS NOT AN ABA SOMETIMES?
			/// IS IT THE INDEX 0 OF THE TRACK? THATS FUCKED UP. COMPARE TO TOCRAW ENTRIES. IT SHOULD BE MATCHING THAT
			/// HEY??? SHOULD THIS EVEN BE HERE? YOURE SUPPOSED TO USE THE INDEXES INSTEAD.
			/// WELL, IF WE KEEP THIS THE MEANING SHOULD BE  SAME AS INDEX[1].LBA (or ABA) SO BE SURE TO WRITE THAT COMMENT HERE
			/// </summary>
			public int Start_ABA;

			/// <summary>
			/// The length as a timestamp (for accessing as a MM:SS:FF)
			/// </summary>
			public Timestamp FriendlyLength { get { return new Timestamp(LengthInSectors); } }
		}

		public class Index
		{
			public int Number;
			public int aba;

			public int LBA
			{
				get { return aba - 150; }
				set { aba = value + 150; }
			}

			//the length of the section
			//HEY! This is commented out because it is a bad idea.
			//The length of a `section`? (what's a section?) is almost useless, and if you want it, you are probably making an error.
			//public int length_lba;
			//public Cue.Timestamp FriendlyLength { get { return new Cue.Timestamp(length_lba); } }
		}

	


		public void AnalyzeLengthsFromIndexLengths()
		{
			//this is a little more complex than it looks, because the length of a thing is not determined by summing it
			//but rather by the difference in lbas between start and end
			LengthInSectors = 0;
			foreach (var session in Sessions)
			{
				var firstTrack = session.Tracks[0];
				var lastTrack = session.Tracks[session.Tracks.Count - 1];
				session.length_aba = lastTrack.Indexes[0].aba + lastTrack.LengthInSectors - firstTrack.Indexes[0].aba;
				LengthInSectors += session.length_aba;
			}
		}
	}

}