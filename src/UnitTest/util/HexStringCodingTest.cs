
#region usings

using System;
using Lephone.Util.Coding;
using NUnit.Framework;

#endregion

namespace Lephone.UnitTest.util
{
	[TestFixture]
	public class CodingHexStringTest
	{
		private IStringCoding Coding = new HexStringCoding();

		[Test]
		public void Test1()
		{
			byte[] bs = Coding.Encode("01");
			Assert.AreEqual( 1, bs.Length );
			Assert.AreEqual( (byte)1, bs[0] );

			bs = Coding.Encode("0a");
			Assert.AreEqual( 1, bs.Length );
            Assert.AreEqual((byte)10, bs[0]);

			bs = Coding.Encode("0f");
			Assert.AreEqual( 1, bs.Length );
            Assert.AreEqual((byte)15, bs[0]);

			bs = Coding.Encode("34");
			Assert.AreEqual( 1, bs.Length );
            Assert.AreEqual((byte)52, bs[0]);

			bs = Coding.Encode("fE");
			Assert.AreEqual( 1, bs.Length );
            Assert.AreEqual((byte)254, bs[0]);
		}

		[Test]
		public void Test2way()
		{
			byte[] bs = new byte[] {1,2,3,4,5,15,34,135,212,4,32,67,166};
			string s = Coding.Decode(bs);
			byte[] bs1 = Coding.Encode(s);
            Assert.AreEqual(bs, bs1);
		}

		[Test]
		public void Test3()
		{
			string s = "179C024222626F41622E1C9E57";
			byte[] bs = new byte[] {23,156,2,66,34,98,111,65,98,46,28,158,87};
			byte[] bs1 = Coding.Encode(s);
            Assert.AreEqual(bs, bs1);
			string s1 = Coding.Decode(bs);
            Assert.AreEqual(s, s1);
		}
    }
}
