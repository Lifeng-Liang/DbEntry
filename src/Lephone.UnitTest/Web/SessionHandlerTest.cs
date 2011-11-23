using Lephone.UnitTest.util;
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
            MockMiscProvider.MockSecends = 1000;
            var s = new SessionHandler();
            s["lose"] = 5678;
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(1, s.CurrentCount);
            MockMiscProvider.MockSecends += 600;
            s["keep"] = 1234;
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(2, s.CurrentCount);
            MockMiscProvider.MockSecends += 601;
            Assert.AreEqual(1, s.Count);
            MockMiscProvider.MockSecends += 601;
            Assert.AreEqual(0, s.Count);
        }

        [Test]
        public void Test2()
        {
            ((MockCookiesHandler)CookiesHandler.Instance).Clear();
            MockMiscProvider.MockSecends = 3000;
            var s = new SessionHandler();

            s["lose1"] = 5678;
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(1, s.CurrentCount);

            MockMiscProvider.MockSecends += 1800;

            var v = s["lose1"];
            Assert.IsNull(v);
        }
    }
}
