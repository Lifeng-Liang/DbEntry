
#region usings

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace Lephone.Util.Coding
{
	public class SerializationCoding : ICoding
	{
		private static BinaryFormatter bf = new BinaryFormatter();

		public byte[] Encode(object Src)
		{
			MemoryStream ms = new MemoryStream();
			bf.Serialize(ms, Src);
			return ms.ToArray();
		}

		public object Decode(byte[] Src)
		{
			MemoryStream ms = new MemoryStream(Src);
			return bf.Deserialize(ms);
		}
	}
}
