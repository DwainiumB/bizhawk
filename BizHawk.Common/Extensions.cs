using System;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace BizHawk.Common
{
	public static class Extensions
	{
		public static string GetDirectory(this Assembly asm)
		{
			string codeBase = asm.CodeBase;
			UriBuilder uri = new UriBuilder(codeBase);
			string path = Uri.UnescapeDataString(uri.Path);
			return Path.GetDirectoryName(path);
		}

		public static int LowerBoundBinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key) where TKey : IComparable<TKey>
		{
			int min = 0;
			int max = list.Count;
			int mid = 0;
			TKey midKey;
			while (min < max)
			{
				mid = (max + min) / 2;
				T midItem = list[mid];
				midKey = keySelector(midItem);
				int comp = midKey.CompareTo(key);
				if (comp < 0)
				{
					min = mid + 1;
				}
				else if (comp > 0)
				{
					max = mid - 1;
				}
				else
				{
					return mid;
				}
			}

			//did we find it exactly?
			if (min == max && keySelector(list[min]).CompareTo(key) == 0)
			{
				return min;
			}

			mid = min;

			//we didnt find it. return something corresponding to lower_bound semantics

			if (mid == list.Count)
				return max; //had to go all the way to max before giving up; lower bound is max
			if (mid == 0)
				return -1; //had to go all the way to min before giving up; lower bound is min

			midKey = keySelector(list[mid]);
			if (midKey.CompareTo(key) >= 0) return mid - 1;
			else return mid;
		}

		public static string ToHexString(this int n, int numdigits)
		{
			return string.Format("{0:X" + numdigits + "}", n);
		}

		//http://stackoverflow.com/questions/1766328/can-linq-use-binary-search-when-the-collection-is-ordered
		public static T BinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
		where TKey : IComparable<TKey>
		{
			int min = 0;
			int max = list.Count;
			while (min < max)
			{
				int mid = (max + min) / 2;
				T midItem = list[mid];
				TKey midKey = keySelector(midItem);
				int comp = midKey.CompareTo(key);
				if (comp < 0)
				{
					min = mid + 1;
				}
				else if (comp > 0)
				{
					max = mid - 1;
				}
				else
				{
					return midItem;
				}
			}
			if (min == max &&
				keySelector(list[min]).CompareTo(key) == 0)
			{
				return list[min];
			}
			throw new InvalidOperationException("Item not found");
		}

		public static void CopyTo(this Stream src, Stream dest)
		{
			int size = (src.CanSeek) ? Math.Min((int)(src.Length - src.Position), 0x2000) : 0x2000;
			byte[] buffer = new byte[size];
			int n;
			do
			{
				n = src.Read(buffer, 0, buffer.Length);
				dest.Write(buffer, 0, n);
			} while (n != 0);
		}

		public static void CopyTo(this MemoryStream src, Stream dest)
		{
			dest.Write(src.GetBuffer(), (int)src.Position, (int)(src.Length - src.Position));
		}

		public static void CopyTo(this Stream src, MemoryStream dest)
		{
			if (src.CanSeek)
			{
				int pos = (int)dest.Position;
				int length = (int)(src.Length - src.Position) + pos;
				dest.SetLength(length);

				while (pos < length)
					pos += src.Read(dest.GetBuffer(), pos, length - pos);
			}
			else
				src.CopyTo(dest);
		}


		public static bool IsBinary(this string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (c == '0' || c == '1')
					continue;
				return false;
			}
			return true;
		}

		public static bool Bit(this byte b, int index)
		{
			return (b & (1 << index)) != 0;
		}

		public static bool Bit(this int b, int index)
		{
			return (b & (1 << index)) != 0;
		}

		public static bool Bit(this ushort b, int index)
		{
			return (b & (1 << index)) != 0;
		}

		public static string GetPrecedingString(this string str, string value)
		{
			int index = str.IndexOf(value);
			if (index < 0)
				return null;
			if (index == 0)
				return "";
			return str.Substring(0, index);
		}

		public static bool In(this string str, params string[] options)
		{
			foreach (string opt in options)
			{
				if (opt.Equals(str, StringComparison.CurrentCultureIgnoreCase)) return true;
			}
			return false;
		}

		public static bool In(this string str, IEnumerable<string> options)
		{
			foreach (string opt in options)
			{
				if (opt.Equals(str, StringComparison.CurrentCultureIgnoreCase)) return true;
			}
			return false;
		}

		public static bool In<T>(this string str, IEnumerable<T> options, Func<T, string, bool> eval)
		{
			foreach (T opt in options)
			{
				if (eval(opt, str))
					return true;
			}
			return false;
		}

		public static bool NotIn(this string str, params string[] options)
		{
			foreach (string opt in options)
			{
				if (opt.ToLower() == str.ToLower()) return false;
			}
			return true;
		}

		public static bool NotIn(this string str, IEnumerable<string> options)
		{
			foreach (string opt in options)
			{
				if (opt.ToLower() == str.ToLower()) return false;
			}
			return true;
		}

		public static bool In(this int i, params int[] options)
		{
			foreach (int j in options)
			{
				if (i == j) return true;
			}
			return false;
		}

		public static bool In(this int i, IEnumerable<int> options)
		{
			foreach (int j in options)
			{
				if (i == j) return true;
			}
			return false;
		}

		public static bool ContainsStartsWith(this IEnumerable<string> options, string str)
		{
			foreach (string opt in options)
			{
				if (opt.StartsWith(str)) return true;
			}
			return false;
		}

		public static string GetOptionValue(this IEnumerable<string> options, string str)
		{
			try
			{
				foreach (string opt in options)
				{
					if (opt.StartsWith(str))
					{
						return opt.Split('=')[1];
					}
				}
			}
			catch (Exception) { }
			return null;
		}

		public static bool IsValidRomExtentsion(this string str, params string[] romExtensions)
		{
			string strUpper = str.ToUpper();
			foreach (string ext in romExtensions)
			{
				if (strUpper.EndsWith(ext.ToUpper())) return true;
			}
			return false;
		}

		public static string ToCommaSeparated(this List<string> list)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < list.Count; i++)
			{
				if (i > 0) sb.Append(",");
				sb.Append(list[i]);
			}
			return sb.ToString();
		}

		public static void SaveAsHex(this byte[] buffer, TextWriter writer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				writer.Write("{0:X2}", buffer[i]);
			}
			writer.WriteLine();
		}

		public unsafe static void SaveAsHexFast(this byte[] buffer, TextWriter writer)
		{
			char* table = Util.HexConvPtr;
			if (buffer.Length > 0)
			{
				int len = buffer.Length;
				fixed (byte* src = &buffer[0])
					for (int i = 0; i < len; i++)
					{
						writer.Write(table[src[i] >> 4]);
						writer.Write(table[src[i] & 15]);
					}
			}
			writer.WriteLine();
		}

		public static void SaveAsHex(this byte[] buffer, TextWriter writer, int length)
		{
			for (int i = 0; i < length; i++)
			{
				writer.Write("{0:X2}", buffer[i]);
			}
			writer.WriteLine();
		}

		public static void SaveAsHex(this short[] buffer, TextWriter writer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				writer.Write("{0:X4}", buffer[i]);
			}
			writer.WriteLine();
		}

		public static void SaveAsHex(this ushort[] buffer, TextWriter writer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				writer.Write("{0:X4}", buffer[i]);
			}
			writer.WriteLine();
		}

		public static void SaveAsHex(this int[] buffer, TextWriter writer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				writer.Write("{0:X8}", buffer[i]);
			}
			writer.WriteLine();
		}

		public static void SaveAsHex(this uint[] buffer, TextWriter writer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				writer.Write("{0:X8}", buffer[i]);
			}
			writer.WriteLine();
		}

		public static void Write(this BinaryWriter bw, int[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
				bw.Write(buffer[i]);
		}

		public static void Write(this BinaryWriter bw, uint[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
				bw.Write(buffer[i]);
		}

		public static void Write(this BinaryWriter bw, short[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
				bw.Write(buffer[i]);
		}

		public static void Write(this BinaryWriter bw, ushort[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
				bw.Write(buffer[i]);
		}

		public static int[] ReadInt32s(this BinaryReader br, int num)
		{
			int[] ret = new int[num];
			for (int i = 0; i < num; i++)
				ret[i] = br.ReadInt32();
			return ret;
		}

		public static short[] ReadInt16s(this BinaryReader br, int num)
		{
			short[] ret = new short[num];
			for (int i = 0; i < num; i++)
				ret[i] = br.ReadInt16();
			return ret;
		}

		public static ushort[] ReadUInt16s(this BinaryReader br, int num)
		{
			ushort[] ret = new ushort[num];
			for (int i = 0; i < num; i++)
				ret[i] = br.ReadUInt16();
			return ret;
		}

		public static void ReadFromHex(this byte[] buffer, string hex)
		{
			if (hex.Length % 2 != 0)
				throw new Exception("Hex value string does not appear to be properly formatted.");
			for (int i = 0; i < buffer.Length && i * 2 < hex.Length; i++)
			{
				string bytehex = "" + hex[i * 2] + hex[i * 2 + 1];
				buffer[i] = byte.Parse(bytehex, NumberStyles.HexNumber);
			}
		}

		private static int Hex2Int(char c)
		{
			if (c <= '9')
				return c - '0';
			else if (c <= 'F')
				return c - '7';
			else
				return c - 'W';
		}

		public static void ReadFromHexFast(this byte[] buffer, string hex)
		{
			//if (hex.Length % 2 != 0)
			//	throw new Exception("Hex value string does not appear to be properly formatted.");
			for (int i = 0; i < buffer.Length && i * 2 < hex.Length; i++)
			{
				buffer[i] = (byte)(Hex2Int(hex[i * 2]) * 16 + Hex2Int(hex[i * 2 + 1]));
			}
			/*
			var b = new byte[buffer.Length];
			b.ReadFromHex(hex);
			for (int i = 0; i < buffer.Length; i++)
			{
				if (b[i] != buffer[i])
					throw new Exception();
			}*/
		}

		public static void ReadFromHex(this short[] buffer, string hex)
		{
			if (hex.Length % 4 != 0)
				throw new Exception("Hex value string does not appear to be properly formatted.");
			for (int i = 0; i < buffer.Length && i * 4 < hex.Length; i++)
			{
				string shorthex = "" + hex[i * 4] + hex[(i * 4) + 1] + hex[(i * 4) + 2] + hex[(i * 4) + 3];
				buffer[i] = short.Parse(shorthex, NumberStyles.HexNumber);
			}
		}

		public static void ReadFromHex(this ushort[] buffer, string hex)
		{
			if (hex.Length % 4 != 0)
				throw new Exception("Hex value string does not appear to be properly formatted.");
			for (int i = 0; i < buffer.Length && i * 4 < hex.Length; i++)
			{
				string ushorthex = "" + hex[i * 4] + hex[(i * 4) + 1] + hex[(i * 4) + 2] + hex[(i * 4) + 3];
				buffer[i] = ushort.Parse(ushorthex, NumberStyles.HexNumber);
			}
		}

		public static void ReadFromHex(this int[] buffer, string hex)
		{
			if (hex.Length % 8 != 0)
				throw new Exception("Hex value string does not appear to be properly formatted.");
			for (int i = 0; i < buffer.Length && i * 8 < hex.Length; i++)
			{
				//string inthex = "" + hex[i * 8] + hex[(i * 8) + 1] + hex[(i * 4) + 2] + hex[(i * 4) + 3] + hex[(i*4
				string inthex = hex.Substring(i * 8, 8);
				buffer[i] = int.Parse(inthex, NumberStyles.HexNumber);
			}
		}

		//public static void SaveAsHex(this uint[] buffer, BinaryWriter bw)
		//{
		//    for (int i = 0; i < buffer.Length; i++)
		//        bw.Write(buffer[i]);
		//}

		//these don't work??? they dont get chosen by compiler
		public static void WriteBit(this BinaryWriter bw, Bit bit) { bw.Write((bool)bit); }
		public static Bit ReadBit(this BinaryReader br) { return br.ReadBoolean(); }
	}


}