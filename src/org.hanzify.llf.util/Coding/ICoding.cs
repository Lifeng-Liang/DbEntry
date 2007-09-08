
using System;

namespace Lephone.Util.Coding
{
	public interface ICoding
	{
		byte[] Encode(object Src);
		object Decode(byte[] Src);
	}
}
