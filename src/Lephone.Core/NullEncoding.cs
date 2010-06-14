using System.Text;

namespace Lephone.Core
{
	public class NullEncoding : Encoding
	{
		public static readonly NullEncoding Instance = new NullEncoding();

		private NullEncoding() {}

		public override int GetByteCount(char[] chars, int index, int count)
		{
			return 0;
		}

		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			return 0;
		}

		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			return 0;
		}

		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
			return 0;
		}

		public override int GetMaxByteCount(int charCount)
		{
			return 0;
		}

		public override int GetMaxCharCount(int byteCount)
		{
			return 0;
		}
	}
}
