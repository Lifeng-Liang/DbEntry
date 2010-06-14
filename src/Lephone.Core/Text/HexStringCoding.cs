﻿using System;

namespace Lephone.Core.Text
{
    public static class HexStringCoding
    {
        private static readonly char[] HexChar = new[] {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
        private static readonly byte[] ReverseHexChar = new byte[128];

        static HexStringCoding()
        {
            SetReverseHexChar('0', '9', 0);
            SetReverseHexChar('A', 'F', 10);
            SetReverseHexChar('a', 'f', 10);
        }

        private static void SetReverseHexChar(char a, char b, int add)
        {
            for ( int i=a; i<=b; i++ )
            {
                ReverseHexChar[i] = (byte)(i - a + add);
            }
        }

        public static byte[] Encode(string src)
        {
            int n = src.Length;
            if (( n % 2 ) == 0 )
            {
                int m = n / 2;
                var ret = new byte [m];
                unsafe
                {
                    fixed ( char* p = src )
                    fixed (byte* r = ReverseHexChar)
                    {
                        for( int i=0, j=0; i<m; i++ )
                        {
                            var b1 = (byte)(r[ p[j++] ] << 4 );
                            byte b2 = r[ p[j++] ];
                            ret[i] = (byte)(b1 | b2);
                        }
                    }
                }
                return ret;
            }
            throw new ArgumentException("String length error!");
        }

        public static string Decode(byte[] src)
        {
            int n = src.Length;
            var ret = new string((char)0, n + n);
            unsafe
            {
                fixed (char* p = ret, h = HexChar)
                {
                    for( int i=0, j=0; i<n; i++ )
                    {
                        p[j+1]	= h[ src[i] & 15 ];
                        p[j]	= h[ src[i] >> 4 ];
                        j += 2;
                    }
                }
            }
            return ret;
        }
    }
}