using System;

namespace Lephone.Core.Text
{
    public static class Base32StringCoding
    {
        private static readonly char[] Base32Table = "0123456789abcdefghijklmnopqrstuv".ToCharArray();
        private static readonly byte[] ReverseByteArray = new byte[128];

        static Base32StringCoding()
        {
            SetReverseHexChar('0', '9', 0);
            SetReverseHexChar('A', 'V', 10);
            SetReverseHexChar('a', 'v', 10);
        }

        private static void SetReverseHexChar(char a, char b, int add)
        {
            for (int i = a; i <= b; i++)
            {
                ReverseByteArray[i] = (byte)(i - a + add);
            }
        }

        public static byte[] Encode(string src)
        {
            int n = src.Length;
            int rest = n % 8;
            int len = GetLength(n, rest);
            var ret = new byte[len];
            int j = 0;
            switch (rest)
            {
                case 2:
                    ret[j++] = (byte)(ReverseByteArray[src[0]] << 5 | ReverseByteArray[src[1]]);
                    break;
                case 4:
                    ret[j++] =
                        (byte)
                        (ReverseByteArray[src[0]] << 7 | ReverseByteArray[src[1]] << 2 | ReverseByteArray[src[2]] >> 3);
                    ret[j++] = (byte)((ReverseByteArray[src[2]] & 7) << 5 | ReverseByteArray[src[3]]);
                    break;
                case 5:
                    byte ix2 = ReverseByteArray[src[1]];
                    byte ix3 = ReverseByteArray[src[3]];
                    ret[j++] = (byte)(ReverseByteArray[src[0]] << 4 | ix2 >> 1);
                    ret[j++] = (byte)((ix2 & 1) << 7 | ReverseByteArray[src[2]] << 2 | ix3 >> 3);
                    ret[j++] = (byte)((ix3 & 7) << 5 | ReverseByteArray[src[4]]);
                    break;
                case 7:
                    byte i3 = ReverseByteArray[src[2]];
                    byte i4 = ReverseByteArray[src[3]];
                    byte i6 = ReverseByteArray[src[5]];
                    ret[j++] = (byte)(ReverseByteArray[src[0]] << 6 | ReverseByteArray[src[1]] << 1 | i3 >> 4);
                    ret[j++] = (byte)((i3 & 0xf) << 4 | i4 >> 1);
                    ret[j++] = (byte)((i4 & 1) << 7 | ReverseByteArray[src[4]] << 2 | i6 >> 3);
                    ret[j++] = (byte)((i6 & 7) << 5 | ReverseByteArray[src[6]]);
                    break;
            }
            for (int i = rest; i < n; i += 8)
            {
                byte i2 = ReverseByteArray[src[i + 1]];
                byte i4 = ReverseByteArray[src[i + 3]];
                byte i5 = ReverseByteArray[src[i + 4]];
                byte i7 = ReverseByteArray[src[i + 6]];
                ret[j++] = (byte)(ReverseByteArray[src[i]] << 3 | i2 >> 2);
                ret[j++] = (byte)((i2 & 3) << 6 | ReverseByteArray[src[i + 2]] << 1 | i4 >> 4);
                ret[j++] = (byte)((i4 & 0xf) << 4 | i5 >> 1);
                ret[j++] = (byte)((i5 & 1) << 7 | ReverseByteArray[src[i + 5]] << 2 | i7 >> 3);
                ret[j++] = (byte)((i7 & 7) << 5 | ReverseByteArray[src[i + 7]]);
            }
            return ret;
        }

        private static int GetLength(int n, int rest)
        {
            int len = n / 8 * 5;
            switch (rest)
            {
                case 0:
                    break;
                case 2:
                    len++;
                    break;
                case 4:
                    len += 2;
                    break;
                case 5:
                    len += 3;
                    break;
                case 7:
                    len += 4;
                    break;
                default:
                    throw new UtilException("Source string length error.");
            }
            return len;
        }

        public static string Decode(byte[] src)
        {
            int n = src.Length;
            int rest = n % 5;
            var len = (int)Math.Round((double)n * 8 / 5 + 0.5);
            var ret = new string((char)0, len);
            unsafe
            {
                fixed (char* p = ret, h = Base32Table)
                {
                    int j = 0;
                    switch(rest)
                    {
                        case 1:
                            p[j++] = h[src[0] >> 5];
                            p[j++] = h[src[0] & 0x1f];
                            break;
                        case 2:
                            p[j++] = h[src[0] >> 7];
                            p[j++] = h[src[0] >> 2 & 0x1f];
                            p[j++] = h[(src[0] & 3) << 3 | src[1] >> 5];
                            p[j++] = h[src[1] & 0x1f];
                            break;
                        case 3:
                            p[j++] = h[src[0] >> 4];
                            p[j++] = h[(src[0] & 0xf) << 1 | src[1] >> 7];
                            p[j++] = h[src[1] >> 2 & 0x1f];
                            p[j++] = h[(src[1] & 3) << 3 | src[2] >> 5];
                            p[j++] = h[src[2] & 0x1f];
                            break;
                        case 4:
                            p[j++] = h[src[0] >> 6];
                            p[j++] = h[src[0] >> 1 & 0x1f];
                            p[j++] = h[(src[0] & 1) << 4 | src[1] >> 4];
                            p[j++] = h[(src[1] & 0xf) << 1 | src[2] >> 7];
                            p[j++] = h[src[2] >> 2 & 0x1f];
                            p[j++] = h[(src[2] & 3) << 3 | src[3] >> 5];
                            p[j++] = h[src[3] & 0x1f];
                            break;
                    }
                    for (int i = rest; i < n; i += 5)
                    {
                        p[j++] = h[src[i] >> 3];
                        p[j++] = h[(src[i] & 7) << 2 | src[i + 1] >> 6];
                        p[j++] = h[src[i + 1] >> 1 & 0x1f];
                        p[j++] = h[(src[i + 1] & 1) << 4 | src[i + 2] >> 4];
                        p[j++] = h[(src[i + 2] & 0xf) << 1 | src[i + 3] >> 7];
                        p[j++] = h[src[i + 3] >> 2 & 0x1f];
                        p[j++] = h[(src[i + 3] & 3) << 3 | src[i + 4] >> 5];
                        p[j++] = h[src[i + 4] & 0x1f];
                    }
                }
            }
            return ret;
        }
    }
}
