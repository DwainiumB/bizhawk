﻿using System;
using System.Linq;

namespace BizHawk
{
	public static class StringHelpers
	{
		public static int HowMany(string str, char c)
		{
			if (!String.IsNullOrEmpty(str))
			{
				return str.Count(t => t == c);
			}
			else
			{
				return 0;
			}
		}

		public static int HowMany(string str, string s)
		{
			int count = 0;
			for (int x = 0; x < (str.Length - s.Length); x++)
			{
				if (str.Substring(x, s.Length) == s)
					count++;
			}
			return count;
		}
	}

    //TODO: put it in its own file
    public static class IntHelpers //TODO: a less lame name
    {
        public static int GetNumDigits(Int32 i)
        {
            //if (i == 0) return 0;
            //if (i < 0x10) return 1;
            //if (i < 0x100) return 2;
            //if (i < 0x1000) return 3; //adelikat: commenting these out because I decided that regardless of domain, 4 digits should be the minimum
            if (i < 0x10000) return 4;
            if (i < 0x1000000) return 6;
            else return 8;
        }
    }
}
