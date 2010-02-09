using System.Collections.Generic;
using System.Reflection;
using Lephone.Util;
using Lephone.Web;
using NUnit.Framework;

namespace Lephone.UnitTest.Web
{
    [TestFixture]
    public class MvcTest
    {
        [Test]
        public void Test1()
        {
            var Parameters = new List<object>();
            var ss = new[] {"controller", "action", "test", "ok", "3rd", "18", "30"};
            MethodInfo mi = typeof (MvcTest).GetMethod("testHelper", ClassHelper.AllFlag);
            var pis = mi.GetParameters();
            MvcDispatcher.ProcessArray(Parameters, ss, 2, pis, 0);

            Assert.AreEqual(3, Parameters.Count);
            Assert.AreEqual(typeof(string[]), Parameters[0].GetType());
            Assert.AreEqual("test", ((string[])Parameters[0])[0]);
            Assert.AreEqual("ok", ((string[])Parameters[0])[1]);
            Assert.AreEqual("3rd", ((string[])Parameters[0])[2]);
            Assert.AreEqual(18, Parameters[1]);
            Assert.AreEqual(30, Parameters[2]);
        }

        private void testHelper(string[] ss, int i1, int i2) {}

        [Test]
        public void Test2()
        {
            var Parameters = new List<object>();
            var ss = new[] { "controller", "action", "test", "ok", "3rd", "18", "30" };
            MethodInfo mi = typeof(MvcTest).GetMethod("test2Helper", ClassHelper.AllFlag);
            var pis = mi.GetParameters();
            MvcDispatcher.ProcessArray(Parameters, ss, 2, pis, 0);

            Assert.AreEqual(3, Parameters.Count);
            Assert.AreEqual(typeof(string[]), Parameters[0].GetType());
            Assert.AreEqual("test", ((string[])Parameters[0])[0]);
            Assert.AreEqual("ok", ((string[])Parameters[0])[1]);
            Assert.AreEqual("3rd", ((string[])Parameters[0])[2]);
            Assert.AreEqual("18", Parameters[1]);
            Assert.AreEqual("30", Parameters[2]);
        }

        private void test2Helper(string[] ss, string s1, string s2) {}
    }
}
