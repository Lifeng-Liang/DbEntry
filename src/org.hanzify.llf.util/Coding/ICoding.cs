
using System;

namespace org.hanzify.llf.util.Coding
{
	public interface ICoding
	{
		byte[] Encode(object Src);
		object Decode(byte[] Src);
	}
}
