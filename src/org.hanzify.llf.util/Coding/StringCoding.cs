
using System;

namespace Lephone.Util.Coding
{
	public abstract class StringCoding : ICoding, IStringCoding
	{
        public static readonly StringCoding Hex = new HexStringCoding();
        public static readonly StringCoding Base64 = new Base64Coding();

		byte[] ICoding.Encode(object Src)
		{
			return Encode((string)Src);
		}

		object ICoding.Decode(byte[] Src)
		{
			return Decode(Src);
		}

		public abstract byte[] Encode(string Src);

		public abstract string Decode(byte[] Src);
	}
}
