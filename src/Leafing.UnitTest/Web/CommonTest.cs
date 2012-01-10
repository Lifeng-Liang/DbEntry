using System;
using System.Reflection;
using Leafing.Core;
using Leafing.Web;
using NUnit.Framework;

namespace Leafing.UnitTest.Web
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

        [Test, ExpectedException(typeof(TargetInvocationException))]
        public void TestConvertType()
        {
            ClassHelper.CallFunction(typeof (SmartPageBase), "GetValue", "", false, "test", typeof (int));
        }

        [Test]
        public void TestConvertType1()
        {
            var i = ClassHelper.CallFunction(typeof(SmartPageBase), "GetValue", "", true, "test", typeof(int?));
            Assert.IsTrue(null == i);
        }

        [Test, ExpectedException(typeof(TargetInvocationException))]
        public void TestConvertType2()
        {
            ClassHelper.CallFunction(typeof(SmartPageBase), "GetValue", "", false, "test", typeof(string));
        }

        [Test]
        public void TestConvertType3()
        {
            var i = (string)ClassHelper.CallFunction(typeof(SmartPageBase), "GetValue", "", true, "test", typeof(string));
            Assert.IsTrue("" == i); //??
        }

        [Test, ExpectedException(typeof(TargetInvocationException))]
        public void TestConvertType4()
        {
            ClassHelper.CallFunction(typeof(SmartPageBase), "GetValue", "", false, "test", typeof(int?));
        }

        [Test, ExpectedException(typeof(TargetInvocationException))]
        public void TestConvertType5()
        {
            var i = ClassHelper.CallFunction(typeof(SmartPageBase), "GetValue", "aa", true, "test", typeof(int?));
            Assert.IsTrue(null == i);
        }

        [Test]
        public void TestConvertType6()
        {
            var i = (int?)ClassHelper.CallFunction(typeof(SmartPageBase), "GetValue", "12", true, "test", typeof(int?));
            Assert.IsTrue(12 == i);
        }

        [Test]
        public void TestConvertType7()
        {
            var i = (int?)ClassHelper.CallFunction(typeof(SmartPageBase), "GetValue", "12", false, "test", typeof(int));
            Assert.IsTrue(12 == i);
        }

        [Test]
        public void TestConvertType8()
        {
            var i = (DateTime?)ClassHelper.CallFunction(typeof(SmartPageBase), "GetValue", "2008-9-10", true, "test", typeof(DateTime?));
            Assert.IsTrue(new DateTime(2008, 9, 10) == i);
        }

    }
}
