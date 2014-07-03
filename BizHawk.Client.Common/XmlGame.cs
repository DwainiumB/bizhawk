﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using BizHawk.Common;
using BizHawk.Common.BufferExtensions;
using BizHawk.Common.IOExtensions;
using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common
{
	public class XmlGame
	{
		public XmlDocument Xml { get; set; }
		public GameInfo GI = new GameInfo();
		public Dictionary<string, byte[]> Assets = new Dictionary<string, byte[]>();

		public static XmlGame Create(HawkFile f)
		{
			try
			{
				var x = new XmlDocument();
				x.Load(f.GetStream());
				var y = x.SelectSingleNode("./BizHawk-XMLGame");
				if (y == null)
				{
					return null;
				}

				var ret = new XmlGame
					{
						GI =
							{
								System = y.Attributes["System"].Value,
								Name = y.Attributes["Name"].Value,
								Status = RomStatus.Unknown
							},
						Xml = x
					};

				var n = y.SelectSingleNode("./LoadAssets");
				if (n != null)
				{
					var HashStream = new MemoryStream();
					int? OriginalIndex = null;

					foreach (XmlNode a in n.ChildNodes)
					{
						string name = a.Name;
						string filename = a.Attributes["FileName"].Value;
						byte[] data;
						if (filename[0] == '|')
						{
							// in same archive
							var ai = f.FindArchiveMember(filename.Substring(1));
							if (ai != null)
							{
								if (OriginalIndex == null)
								{
									OriginalIndex = f.GetBoundIndex();
								}

								f.Unbind();
								f.BindArchiveMember(ai);
								data = f.GetStream().ReadAllBytes();
							}
							else
							{
								throw new Exception("Couldn't load XMLGame LoadAsset \"" + name + "\"");
							}
						}
						else
						{
							// relative path
							var fullpath = Path.GetDirectoryName(f.CanonicalFullPath.Split('|').First()) ?? string.Empty;
							fullpath = Path.Combine(fullpath, filename.Split('|').First());
							try
							{
								data = File.ReadAllBytes(fullpath.Split('|').First());
							}
							catch
							{
								throw new Exception("Couldn't load XMLGame LoadAsset \"" + name + "\"");
							}
						}

						ret.Assets[name] = data;

						using (var sha1 = System.Security.Cryptography.SHA1.Create())
						{
							sha1.TransformFinalBlock(data, 0, data.Length);
							HashStream.Write(sha1.Hash, 0, sha1.Hash.Length);
						}
					}

					ret.GI.Hash = HashStream.GetBuffer().HashSHA1(0, (int)HashStream.Length);
					HashStream.Close();
					if (OriginalIndex != null)
					{
						f.Unbind();
						f.BindArchiveMember((int)OriginalIndex);
					}
				}
				else
				{
					ret.GI.Hash = "0000000000000000000000000000000000000000";
				}

				return ret;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.ToString());
			}
		}
	}
}
