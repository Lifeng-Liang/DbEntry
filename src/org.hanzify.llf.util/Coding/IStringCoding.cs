namespace Lephone.Util.Coding
{
	public interface IStringCoding
	{
		byte[] Encode(string Src);
		string Decode(byte[] Src);
	}
}
