﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

using BizHawk.Common;
using BizHawk.Emulation.Common;

// IDEA: put filesizes in DB too. then scans can go real quick by only scanning filesizes that match (and then scanning filesizes that dont match, in case of an emergency)
// this would be adviseable if we end up with a very large firmware file

namespace BizHawk.Client.Common
{
	public class FirmwareManager
	{
		// represents a file found on disk in the user's firmware directory matching a file in our database
		public class RealFirmwareFile
		{
			public FileInfo FileInfo { get; set; }
			public string Hash { get; set; }
		}

		public class ResolutionInfo
		{
			public bool UserSpecified { get; set; }
			public bool Missing { get; set; }
			public bool KnownMismatching { get; set; }
			public FirmwareDatabase.FirmwareFile KnownFirmwareFile { get; set; }
			public string FilePath { get; set; }
			public string Hash { get; set; }
		}

		private readonly Dictionary<FirmwareDatabase.FirmwareRecord, ResolutionInfo> _resolutionDictionary = new Dictionary<FirmwareDatabase.FirmwareRecord, ResolutionInfo>();

		public ResolutionInfo Resolve(string sysId, string firmwareId)
		{
			return Resolve(FirmwareDatabase.LookupFirmwareRecord(sysId, firmwareId));
		}

		public ResolutionInfo Resolve(FirmwareDatabase.FirmwareRecord record)
		{
			bool first = true;

		RETRY:

			ResolutionInfo resolved;
			_resolutionDictionary.TryGetValue(record, out resolved);

			// couldnt find it! do a scan and resolve to try harder
			if (resolved == null && first)
			{
				DoScanAndResolve();
				first = false;
				goto RETRY;
			}

			return resolved;
		}

		// Requests the spcified firmware. tries really hard to scan and resolve as necessary
		public string Request(string sysId, string firmwareId)
		{
			var resolved = Resolve(sysId, firmwareId);
			if (resolved == null) return null;
			return resolved.FilePath;
		}

		public class RealFirmwareReader
		{
			byte[] buffer = new byte[0];
			public RealFirmwareFile Read(FileInfo fi)
			{
				var rff = new RealFirmwareFile { FileInfo = fi };
				long len = fi.Length;
				if (len > buffer.Length)
				{
					buffer = new byte[len];
				}

				using (var fs = fi.OpenRead())
				{
					fs.Read(buffer, 0, (int)len);
				}

				rff.Hash = Util.Hash_SHA1(buffer, 0, (int)len);
				dict[rff.Hash] = rff;
				_files.Add(rff);
				return rff;
			}

			public readonly Dictionary<string, RealFirmwareFile> dict = new Dictionary<string, RealFirmwareFile>();
			private readonly List<RealFirmwareFile> _files = new List<RealFirmwareFile>();
		}

		public void DoScanAndResolve()
		{
			var reader = new RealFirmwareReader();

			// build a list of files under the global firmwares path, and build a hash for each of them while we're at it
			var todo = new Queue<DirectoryInfo>();
			todo.Enqueue(new DirectoryInfo(PathManager.MakeAbsolutePath(Global.Config.PathEntries.FirmwaresPathFragment, null)));
	
			while (todo.Count != 0)
			{
				var di = todo.Dequeue();

				if (!di.Exists)
					continue;

				// we're going to allow recursing into subdirectories, now. its been verified to work OK
				foreach (var disub in di.GetDirectories())
				{
					todo.Enqueue(disub);
				}
				
				foreach (var fi in di.GetFiles())
				{
					reader.Read(fi);
				}
			}

			// now, for each firmware record, try to resolve it
			foreach (var fr in FirmwareDatabase.FirmwareRecords)
			{
				// clear previous resolution results
				_resolutionDictionary.Remove(fr);

				// get all options for this firmware (in order)
				var fr1 = fr;
				var options =
					from fo in FirmwareDatabase.FirmwareOptions
					where fo.systemId == fr1.systemId && fo.firmwareId == fr1.firmwareId
					select fo;

				// try each option
				foreach (var fo in options)
				{
					var hash = fo.hash;

					// did we find this firmware?
					if (reader.dict.ContainsKey(hash))
					{
						// rad! then we can use it
						var ri = new ResolutionInfo
							{
								FilePath = reader.dict[hash].FileInfo.FullName,
								KnownFirmwareFile = FirmwareDatabase.FirmwareFilesByHash[hash],
								Hash = hash
							};
						_resolutionDictionary[fr] = ri;
						goto DONE_FIRMWARE;
					}
				}

			DONE_FIRMWARE: ;

			}

			// apply user overrides
			foreach (var fr in FirmwareDatabase.FirmwareRecords)
			{
				string userSpec;
				
				// do we have a user specification for this firmware record?
				if (Global.Config.FirmwareUserSpecifications.TryGetValue(fr.ConfigKey, out userSpec))
				{
					// flag it as user specified
					ResolutionInfo ri;
					if (!_resolutionDictionary.TryGetValue(fr, out ri))
					{
						ri = new ResolutionInfo();
						_resolutionDictionary[fr] = ri;
					}
					ri.UserSpecified = true;
					ri.KnownFirmwareFile = null;
					ri.FilePath = userSpec;
					ri.Hash = null;

					// check whether it exists
					var fi = new FileInfo(userSpec);
					if (!fi.Exists)
					{
						ri.Missing = true;
						continue;
					}

					// compute its hash 
					var rff = reader.Read(fi);
					ri.Hash = rff.Hash;

					// check whether it was a known file anyway, and go ahead and bind to the known file, as a perk (the firmwares config doesnt really use this information right now)
					FirmwareDatabase.FirmwareFile ff;
					if (FirmwareDatabase.FirmwareFilesByHash.TryGetValue(rff.Hash,out ff))
					{
						ri.KnownFirmwareFile = ff;

						// if the known firmware file is for a different firmware, flag it so we can show a warning
						var option =
							(from fo in FirmwareDatabase.FirmwareOptions
							where fo.hash == rff.Hash && fo.ConfigKey != fr.ConfigKey
							select fr).FirstOrDefault();

						if (option != null)
						{
							ri.KnownMismatching = true;
						}
					}
				}
			}
		}
	}
}