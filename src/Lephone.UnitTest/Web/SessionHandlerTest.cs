using Lephone.UnitTest.util;
using Lephone.Core;
using Lephone.Web.Mvc;
using NUnit.Framework;

namespace Lephone.UnitTest.Web
{
    [TestFixture]
    public class SessionHandlerTest
    {
        [Test]
        public void Test1()
        {
            ((MockCookiesHandler)CookiesHandler.Instance).Clear();
            var misc = (MockMiscProvider)MiscProvider.Instance;
            misc.SetSecends(1000);
            var s = new SessionHandler();
            s["lose"] = 5678;
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(1, s.CurrentCount);
            misc.AddSecends(600);
            s["keep"] = 1234;
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(2, s.CurrentCount);
            misc.AddSecends(601);
            Assert.AreEqual(1, s.Count);
            misc.AddSecends(601);
            Assert.AreEqual(0, s.Count);
        }

        [Test]
        public void Test2()
        {
            ((MockCookiesHandler)CookiesHandler.Instance).Clear();
            var misc = (MockMiscProvider)MiscProvider.Instance;
            misc.SetSecends(3000);
            var s = new SessionHandler();

            s["lose1"] = 5678;
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(1, s.CurrentCount);

            misc.AddSecends(1800);

            var v = s["lose1"];
            Assert.IsNull(v);
        }
    }
}
