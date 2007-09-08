
using System;

namespace Lephone.Util.Coding
{
	public class HexStringCoding : StringCoding
	{
		private static char[] HexChar = new char[] {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
		private static byte[] ReverseHexChar = new byte[128];

		static HexStringCoding()
		{
			SetReverseHexChar('0', '9', 0);
			SetReverseHexChar('A', 'F', 10);
			SetReverseHexChar('a', 'f', 10);
		}

		private static void SetReverseHexChar(char a, char b, int Add)
		{
			for ( int i=(int)a; i<=(int)b; i++ )
			{
				ReverseHexChar[i] = (byte)(i - (int)a + Add);
			}
		}

		public override byte[] Encode(string Src)
		{
			int n = Src.Length;
			if (( n % 2 ) == 0 )
			{
				int m = n / 2;
				byte[] ret = new byte [m];
				unsafe
				{
					fixed ( char* p = Src )
                    fixed (byte* r = ReverseHexChar)
                    {
						for( int i=0, j=0; i<m; i++ )
						{
							byte b1 = (byte)(r[ p[j++] ] << 4 );
							byte b2 = (byte)(r[ p[j++] ]);
							ret[i] = (byte)(b1 | b2);
						}
					}
				}
				return ret;
			}
			else
			{
				throw new ArgumentException("String length error!");
			}
		}

		public override string Decode(byte[] Src)
		{
			int n = Src.Length;
			string ret = new string((char)0, n + n);
			unsafe
			{
                fixed (char* p = ret, h = HexChar)
				{
					for( int i=0, j=0; i<n; i++ )
					{
						p[j+1]	= h[ Src[i] & 15 ];
						p[j]	= h[ Src[i] >> 4 ];
						j += 2;
					}
				}
			}
			return ret;
		}
	}
}
