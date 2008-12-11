using System.Collections.Generic;
using System.Reflection;
using Lephone.Util;
using Lephone.Web;
using NUnit.Framework;

namespace Lephone.UnitTest.Web
{
    [TestFixture]
    public class RailsTest
    {
        [Test]
        public void Test1()
        {
            var paramters = new List<object>();
            var ss = new[] {"controller", "action", "test", "ok", "3rd", "18", "30"};
            MethodInfo mi = typeof (RailsTest).GetMethod("testHelper", ClassHelper.AllFlag);
            var pis = mi.GetParameters();
            RailsDispatcher.ProcessArray(paramters, ss, 2, pis, 0);

            Assert.AreEqual(3, paramters.Count);
            Assert.AreEqual(typeof(string[]), paramters[0].GetType());
            Assert.AreEqual("test", ((string[])paramters[0])[0]);
            Assert.AreEqual("ok", ((string[])paramters[0])[1]);
            Assert.AreEqual("3rd", ((string[])paramters[0])[2]);
            Assert.AreEqual(18, paramters[1]);
            Assert.AreEqual(30, paramters[2]);
        }

        private void testHelper(string[] ss, int i1, int i2) {}
    }
}
