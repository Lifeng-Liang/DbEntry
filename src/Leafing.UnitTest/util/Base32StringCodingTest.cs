using Leafing.Core.Text;
using NUnit.Framework;

namespace Leafing.UnitTest.util {
    [TestFixture]
    public class Base32StringCodingTest {
        [Test]
        public void TestEncoding0() {
            var bs = Base32StringCoding.Encode("e928g6fc");
            Assert.AreEqual(new byte[] { 0x72, 0x44, 0x88, 0x19, 0xEC }, bs);
        }

        [Test]
        public void TestEncoding1() {
            var bs = Base32StringCoding.Encode("35e928g6fc");
            Assert.AreEqual(new byte[] { 0x65, 0x72, 0x44, 0x88, 0x19, 0xEC }, bs);
        }

        [Test]
        public void TestEncoding2() {
            var bs = Base32StringCoding.Encode("0e35e928g6fc");
            Assert.AreEqual(new byte[] { 0x38, 0x65, 0x72, 0x44, 0x88, 0x19, 0xEC }, bs);
        }

        [Test]
        public void TestEncoding3() {
            var bs = Base32StringCoding.Encode("aee35e928g6fc");
            Assert.AreEqual(new byte[] { 0xa7, 0x38, 0x65, 0x72, 0x44, 0x88, 0x19, 0xEC }, bs);
        }

        [Test]
        public void TestEncoding4() {
            var bs = Base32StringCoding.Encode("2fqee35e928g6fc");
            Assert.AreEqual(new byte[] { 0x9f, 0xa7, 0x38, 0x65, 0x72, 0x44, 0x88, 0x19, 0xEC }, bs);
        }

        [Test]
        public void TestDecoding0() {
            //01110010 01000100 10001000 00011001 11101100
            var s = Base32StringCoding.Decode(new byte[] { 0x72, 0x44, 0x88, 0x19, 0xEC });
            Assert.AreEqual("e928g6fc", s);
        }

        [Test]
        public void TestDecoding1() {
            //01100101 01110010 01000100 10001000 00011001 11101100
            var s = Base32StringCoding.Decode(new byte[] { 0x65, 0x72, 0x44, 0x88, 0x19, 0xEC });
            Assert.AreEqual("35e928g6fc", s);
        }

        [Test]
        public void TestDecoding2() {
            //00111000 01100101 01110010 01000100 10001000 00011001 11101100
            var s = Base32StringCoding.Decode(new byte[] { 0x38, 0x65, 0x72, 0x44, 0x88, 0x19, 0xEC });
            Assert.AreEqual("0e35e928g6fc", s);
        }

        [Test]
        public void TestDecoding3() {
            //10100111 00111000 01100101 01110010 01000100 10001000 00011001 11101100
            var s = Base32StringCoding.Decode(new byte[] { 0xa7, 0x38, 0x65, 0x72, 0x44, 0x88, 0x19, 0xEC });
            Assert.AreEqual("aee35e928g6fc", s);
        }

        [Test]
        public void TestDecoding4() {
            //10011111 10100111 00111000 01100101 01110010 01000100 10001000 00011001 11101100
            var s = Base32StringCoding.Decode(new byte[] { 0x9f, 0xa7, 0x38, 0x65, 0x72, 0x44, 0x88, 0x19, 0xEC });
            Assert.AreEqual("2fqee35e928g6fc", s);
        }
    }
}