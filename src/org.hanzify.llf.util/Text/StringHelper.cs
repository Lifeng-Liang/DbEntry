
#region usings

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace Lephone.Util.Text
{
	public static class StringHelper
	{
		public static bool IsIndentityName(string s)
		{
			return Regex.IsMatch(s, "^[ \t]*(_*)([a-zA-Z]+)([a-zA-Z0-9_]*)[ \t]*$", RegexOptions.Compiled);
		}

        public static string[] Split(string s, char c, int count)
        {
            string[] ss = s.Split(new char[] { c }, count);
            if (ss.Length == count)
            {
                return ss;
            }
            else
            {
                List<string> l = new List<string>(ss);
                for (int i = ss.Length; i < count; i++)
                {
                    l.Add("");
                }
                return l.ToArray();
            }
        }

        public static string EnsureEndsWith(string s, char c)
		{
			return EnsureEndsWith(s, new string(c, 1));
		}

		public static string EnsureEndsWith(string s, string s1)
		{
			if (!s.EndsWith(s1))
			{
				return s + s1;
			}
			return s;
		}

		public static string MultiLineAddPrefix(string Source)
		{
			return MultiLineAddPrefix(Source, "\t");
		}

		public static string MultiLineAddPrefix(string Source, string Prefix)
		{
			return MultiLineAddPrefix(Source, Prefix, '\n');
		}

		public static string MultiLineAddPrefix(string Source, string Prefix, char SplitBy)
		{
			StringBuilder sb = new StringBuilder();
			string[] ss = Source.Split(SplitBy);
			foreach ( string s in ss )
			{
				sb.Append(Prefix);
				sb.Append(s);
				sb.Append(SplitBy);
			}
			return sb.ToString();
		}

		public static string GetStringLeft(string s)
		{
			return GetStringLeft(s, 1);
		}

		public static string GetStringLeft(string s, int n)
		{
			if ( s.Length > n )
			{
				return s.Substring(0, s.Length - n);
			}
			throw new ArgumentOutOfRangeException();
		}

		public static string GetCString(string s)
		{
			int n = s.IndexOf('\0');
			if ( n > 0 )
			{
				return s.Substring(0, n);
			}
			else
			{
				return s;
			}
		}

		public static byte[] GetBytesByLength(byte[] bs, int length)
		{
			MemoryStream ms = new MemoryStream(bs, 0, length);
			return ms.ToArray();
		}

        public static int GetAnsiLength(string s)
        {
            return EncodingEx.Default.GetByteCount(s);
        }

		public static string GetMultiByteSubString(string s, int Count)
		{
			char[] cs = s.ToCharArray();
			int n = 0;
			for ( int i = 0; i < cs.Length; i++ )
			{
				int c = (int)cs[i];
				if ( c >=32 && c <128 )
				{
					n++;
				}
				else if ( c > 256 )
				{
					n += 2;
				}
				else
				{
					return s.Substring(0, i);
				}
				if ( n == Count )
				{
					return s.Substring(0, i + 1);
				}
				else if ( n > Count )
				{
					return s.Substring(0, i);
				}
			}
			return s;
		}

		public static string EnumToString(object o)
		{
            return EnumToString(o.GetType(), o.ToString());
		}

        public static string EnumToString(Type EnumType, string Name)
        {
            ShowStringAttribute[] os = (ShowStringAttribute[])EnumType.GetField(Name)
                .GetCustomAttributes(typeof(ShowStringAttribute), false);
            if (os != null && os.Length == 1)
            {
                return os[0].ShowString;
            }
            return Name;
        }

		public static string StreamReadToEnd(Stream s)
		{
			return StreamReadToEnd( new StreamReader(s) );
		}

        public static string StreamReadToEnd(Stream s, long Position)
        {
            s.Position = Position;
            return StreamReadToEnd(new StreamReader(s));
        }

        public static string StreamReadToEnd(StreamReader s)
		{
			using ( s )
			{
				return s.ReadToEnd();
			}
		}

        public static string Capitalize(string s)
        {
            return s[0].ToString().ToUpper() + s.Substring(1);
        }
	}
}
