using Lephone.Util.Text;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class NameMapperTest
    {
        [Test]
        public void Test1()
        {
            Assert.AreEqual("TomCat", (new NameMapper()).MapName("TomCat"));
        }

        [Test]
        public void Test2()
        {
            Assert.AreEqual("Tom_Cat", (new UnderlineNameMapper()).MapName("TomCat"));
        }

        [Test]
        public void Test3()
        {
            Assert.AreEqual("ETom_Cat", (new UnderlineNameMapper()).MapName("ETomCat"));
        }

        [Test]
        public void Test4()
        {
            Assert.AreEqual("e_Exception", (new UnderlineNameMapper()).MapName("eException"));
        }

        [Test]
        public void Test5()
        {
            Assert.AreEqual("Tom_DCat", (new UnderlineNameMapper()).MapName("TomDCat"));
        }

        [Test]
        public void Test6()
        {
            Assert.AreEqual("Tom_Cat", (new UnderlineNameMapper()).MapName("Tom_Cat"));
        }

        [Test]
        public void Test7()
        {
            Assert.AreEqual("test", (new UnderlineNameMapper()).MapName("test"));
        }
    }
}
