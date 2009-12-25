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
            var misc = (MockMiscProvider) MiscProvider.Instance;
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
    }
}
