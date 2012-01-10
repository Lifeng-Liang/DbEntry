using Leafing.Data.Caching;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class KeyGenTest : IDbObject
    {
        [Test]
        public void Test1()
        {
            var k = new KeyGenerator();
            string s = k.GetKey(typeof(KeyGenTest), 25);
            Assert.AreEqual("Leafing.UnitTest.Data.KeyGenTest,25", s);
        }
    }
}
