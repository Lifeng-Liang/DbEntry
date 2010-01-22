using System;
using Lephone.UnitTest.util;
using Lephone.Util;
using Lephone.Web.Rails;
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
            misc.SetNow(new DateTime(1999, 12, 30, 22, 30, 10));
            var s = new SessionHandler();
            s["lose"] = 5678;
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(2, s.CurrentCount);
            misc.Add(new TimeSpan(0, 10, 0));
            s["keep"] = 1234;
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(3, s.CurrentCount);
            misc.Add(new TimeSpan(0, 10, 1));
            Assert.AreEqual(1, s.Count);
            misc.Add(new TimeSpan(0, 10, 1));
            Assert.AreEqual(0, s.Count);
        }

        [Test]
        public void Test2()
        {
            ((MockCookiesHandler)CookiesHandler.Instance).Clear();
            var misc = (MockMiscProvider)MiscProvider.Instance;
            misc.SetNow(new DateTime(2000, 12, 30, 22, 30, 10));
            var s = new SessionHandler();

            s["lose1"] = 5678;
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(2, s.CurrentCount);

            misc.Add(new TimeSpan(0, 30, 0));

            var v = s["lose1"];
            Assert.IsNull(v);
        }
    }
}
