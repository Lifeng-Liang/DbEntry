using System.Text;

namespace Lephone.Util
{
	public class EncodingEx
	{
		private static readonly Encoding _GBK;
		private static readonly Encoding _BIG5;
		private static readonly Encoding _ShiftJIS;
		private static readonly Encoding _Korea;

		static EncodingEx()
		{
			_GBK = GetCoding(936);
			_BIG5 = GetCoding(950);
			_ShiftJIS = GetCoding(932);
			_Korea = GetCoding(949);
		}

		private static Encoding GetCoding(int n)
		{
			Encoding c;
			try
			{
				c = Encoding.GetEncoding(n);
			}
			catch
			{
				c = NullEncoding.Instance;
			}
			return c;
		}

		public static Encoding GBK
		{
			get { return _GBK; }
		}

		public static Encoding BIG5
		{
			get { return _BIG5; }
		}

		private static Encoding ShiftJIS
		{
			get { return _ShiftJIS; }
		}

		private static Encoding Korea
		{
			get { return _Korea; }
		}

		public static Encoding ASCII
		{
			get { return Encoding.ASCII; }
		}

		public static Encoding BigEndianUnicode
		{
			get { return Encoding.BigEndianUnicode; }
		}

		public static Encoding Default
		{
			get { return Encoding.Default; }
		}

		public static Encoding Unicode
		{
			get { return Encoding.Unicode; }
		}

		public static Encoding UTF7
		{
			get { return Encoding.UTF7; }
		}

		public static Encoding UTF8
		{
			get { return Encoding.UTF8; }
		}

		public static Encoding GetEncoding(string Name)
		{
			return Encoding.GetEncoding(Name);
		}

		public static Encoding GetEncoding(int CodePage)
		{
			return Encoding.GetEncoding(CodePage);
		}
	}
}
