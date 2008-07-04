using Lephone.Util.Text;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class CodingHexStringTest
    {
        [Test]
        public void Test1()
        {
            byte[] bs = HexStringCoding.Encode("01");
            Assert.AreEqual(1, bs.Length);
            Assert.AreEqual(1, bs[0]);

            bs = HexStringCoding.Encode("0a");
            Assert.AreEqual(1, bs.Length);
            Assert.AreEqual(10, bs[0]);

            bs = HexStringCoding.Encode("0f");
            Assert.AreEqual(1, bs.Length);
            Assert.AreEqual(15, bs[0]);

            bs = HexStringCoding.Encode("34");
            Assert.AreEqual(1, bs.Length);
            Assert.AreEqual(52, bs[0]);

            bs = HexStringCoding.Encode("fE");
            Assert.AreEqual(1, bs.Length);
            Assert.AreEqual(254, bs[0]);
        }

        [Test]
        public void Test2way()
        {
            byte[] bs = new byte[] {1, 2, 3, 4, 5, 15, 34, 135, 212, 4, 32, 67, 166};
            string s = HexStringCoding.Decode(bs);
            byte[] bs1 = HexStringCoding.Encode(s);
            Assert.AreEqual(bs, bs1);
        }

        [Test]
        public void Test3()
        {
            const string s = "179C024222626F41622E1C9E57";
            byte[] bs = new byte[] {23, 156, 2, 66, 34, 98, 111, 65, 98, 46, 28, 158, 87};
            byte[] bs1 = HexStringCoding.Encode(s);
            Assert.AreEqual(bs, bs1);
            string s1 = HexStringCoding.Decode(bs);
            Assert.AreEqual(s, s1);
        }
    }
}