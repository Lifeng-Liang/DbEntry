using Lephone.Data.Caching;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class KeyGenTest : IDbObject
    {
        [Test]
        public void Test1()
        {
            KeyGenerator k = new KeyGenerator();
            string s = k.GetKey(typeof(KeyGenTest), 25);
            Assert.AreEqual("Lephone.UnitTest.Data.KeyGenTest,25", s);
        }

        [Test]
        public void Test2()
        {
            KeyGenerator k = new FullKeyGenerator();
            string s = k.GetKey(typeof(KeyGenTest), 25);
            string a = typeof(KeyGenTest).Assembly.FullName;
            Assert.AreEqual(a + ",Lephone.UnitTest.Data.KeyGenTest,25", s);
        }
    }
}
