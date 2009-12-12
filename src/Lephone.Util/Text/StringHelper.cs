using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lephone.Util.Text
{
	public static class StringHelper
	{
		public static bool IsIndentityName(string s)
		{
			return Regex.IsMatch(s, "^[ \t]*(_*)([a-zA-Z]+)([a-zA-Z0-9_]*)[ \t]*$", RegexOptions.Compiled);
		}

        public static bool IsSpName(string s)
        {
            var i = s.IndexOf('.');
            if(i > 0)
            {
                string s1 = s.Substring(0, i);
                string s2 = s.Substring(i + 1);
                char c1 = s1[s1.Length - 1];
                char c2 = s2[0];
                if(c1 == ' ' || c1 == '\t' || c2 == ' ' || c2 == '\t')
                {
                    return false;
                }
                return IsIndentityName(s1) && IsIndentityName(s2);
            }
            return IsIndentityName(s);
        }

        public static string[] Split(string s, char c, int count)
        {
            string[] ss = s.Split(new[] { c }, count);
            if (ss.Length == count)
            {
                return ss;
            }
            var l = new List<string>(ss);
            for (int i = ss.Length; i < count; i++)
            {
                l.Add("");
            }
            return l.ToArray();
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

		public static string MultiLineAddPrefix(string source)
		{
			return MultiLineAddPrefix(source, "\t");
		}

		public static string MultiLineAddPrefix(string source, string prefix)
		{
			return MultiLineAddPrefix(source, prefix, '\n');
		}

		public static string MultiLineAddPrefix(string source, string prefix, char splitBy)
		{
			var sb = new StringBuilder();
			string[] ss = source.Split(splitBy);
			foreach ( string s in ss )
			{
				sb.Append(prefix);
				sb.Append(s);
				sb.Append(splitBy);
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
		    return s;
		}

		public static byte[] GetBytesByLength(byte[] bs, int length)
		{
			var ms = new MemoryStream(bs, 0, length);
			return ms.ToArray();
		}

        public static int GetAnsiLength(string s)
        {
            return EncodingEx.Default.GetByteCount(s);
        }

		public static string GetMultiByteSubString(string s, int count)
		{
			char[] cs = s.ToCharArray();
			int n = 0;
			for ( int i = 0; i < cs.Length; i++ )
			{
				int c = cs[i];
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
				if ( n == count )
				{
					return s.Substring(0, i + 1);
				}
			    if ( n > count )
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

        public static string EnumToString(Type enumType, string name)
        {
            var os = (ShowStringAttribute[])enumType.GetField(name)
                .GetCustomAttributes(typeof(ShowStringAttribute), false);
            if (os != null && os.Length == 1)
            {
                return os[0].ShowString;
            }
            return name;
        }

		public static string StreamReadToEnd(Stream s)
		{
			return StreamReadToEnd( new StreamReader(s) );
		}

        public static string StreamReadToEnd(Stream s, long position)
        {
            s.Position = position;
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

        private static readonly SHA512 hash = SHA512.Create();

        public static byte[] Hash(string s)
        {
            byte[] input = Encoding.Unicode.GetBytes(s);
            return hash.ComputeHash(input);
        }

        private static readonly MD5 md5 = MD5.Create();

        public static byte[] HashMD5(string s)
        {
            byte[] input = Encoding.Unicode.GetBytes(s);
            return md5.ComputeHash(input);
        }

        public static string ProcessSymbol(string text, string left, string right,
            CallbackReturnHandler<string, string> callback)
        {
            var ret = new StringBuilder();
            int last = 0;
            int leftLen = left.Length;
            int rightLen = right.Length;
            while (true)
            {
                int m = text.IndexOf(left, last);
                if (m >= last)
                {
                    int n = text.IndexOf(right, m + leftLen);
                    if (n > m)
                    {
                        ret.Append(text.Substring(last, m - last));
                        if (callback != null)
                        {
                            var inner = text.Substring(m + leftLen, n - m - leftLen);
                            ret.Append(callback(inner));
                        }
                        last = n + rightLen;
                        continue;
                    }
                }
                ret.Append(text.Substring(last));
                break;
            }
            return ret.ToString();
        }
	}
}
