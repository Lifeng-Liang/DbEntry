
using System;

namespace org.hanzify.llf.util.Coding
{
	public interface IStringCoding
	{
		byte[] Encode(string Src);
		string Decode(byte[] Src);
	}
}
