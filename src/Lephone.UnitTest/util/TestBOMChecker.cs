using Lephone.Core.Text;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class TestBOMChecker
    {
        [Test]
        public void Test1()
        {
            var bs = new byte[] { 0, 0, 0xfe, 0xff };
            Assert.AreEqual(UtfEncoding.BigEndianUTF32, BOMChecker.GetUtfEncoding(bs));
        }

        [Test]
        public void Test2()
        {
            var bs = new byte[] { 0xff, 0xfe, 0, 0 };
            Assert.AreEqual(UtfEncoding.LittleEndianUTF32, BOMChecker.GetUtfEncoding(bs));
        }

        [Test]
        public void Test3()
        {
            var bs = new byte[] { 0xfe, 0xff };
            Assert.AreEqual(UtfEncoding.BigEndianUTF16, BOMChecker.GetUtfEncoding(bs));
        }

        [Test]
        public void Test4()
        {
            var bs = new byte[] { 0xff, 0xfe };
            Assert.AreEqual(UtfEncoding.LittleEndianUTF16, BOMChecker.GetUtfEncoding(bs));
        }

        [Test]
        public void Test5()
        {
            var bs = new byte[] { 0xef, 0xbb, 0xbf };
            Assert.AreEqual(UtfEncoding.UTF8, BOMChecker.GetUtfEncoding(bs));
        }

        [Test]
        public void Test6()
        {
            var bs = new byte[] { 0xef, 0xba, 0xbf };
            Assert.AreEqual(UtfEncoding.Unknown, BOMChecker.GetUtfEncoding(bs));
        }

        [Test]
        public void Test7()
        {
            var bs = new byte[0];
            Assert.AreEqual(UtfEncoding.Unknown, BOMChecker.GetUtfEncoding(bs));
        }
    }
}