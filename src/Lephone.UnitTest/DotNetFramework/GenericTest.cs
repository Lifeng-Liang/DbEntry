using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Lephone.UnitTest.DotNetFramework
{
    [TestFixture]
    public class GenericTest
    {
        [Test]
        public void Test1()
        {
            Type t = typeof(List<object>);
            Type t1 = typeof(List<int>);
            Assert.AreEqual(t.GetGenericTypeDefinition(), t1.GetGenericTypeDefinition());
        }

        [Test]
        public void Test2()
        {
            Type t = typeof(List<int>);
            Assert.AreEqual(typeof(int), t.GetGenericArguments()[0]);
        }

        [Test]
        public void Test3()
        {
            int? x = 3;
            Assert.IsTrue(x.GetType().IsValueType);
            Assert.IsTrue(typeof(int?).IsValueType);
        }
    }
}