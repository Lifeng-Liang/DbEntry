using Lephone.Web;
using NUnit.Framework;

namespace Lephone.UnitTest.Web
{
    [TestFixture]
    public class CommonTest
    {
        [Test]
        public void Test1()
        {
            var w = new WebException("{0}-{1}", "aa", 13);
            Assert.AreEqual("aa-13", w.Message);
        }
    }
}
