using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Leafing.Core;
using NUnit.Framework;

namespace Leafing.UnitTest.DotNetFramework
{
    [TestFixture]
    public class ReflectionTest
    {
        public class MyClass
        {
            static MyClass() { }
            private MyClass() { }
            protected MyClass(int n) {}
            public MyClass(long n) {}
        }

        [Test]
        public void TestGetAllConstructors()
        {
            Type t = typeof(MyClass);
            var cs = t.GetConstructors(ClassHelper.AllFlag);
            Assert.AreEqual(4, cs.Count());
        }

        [Test]
        public void TestGetPublicConstructor()
        {
            Type t = typeof(MyClass);
            var cs = t.GetConstructors();
            Assert.AreEqual(1, cs.Count());
        }

        [Test]
        public void TestProtectedConstructor()
        {
            Type t = typeof(MyClass);
            var cs = t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreEqual(2, cs.Count());
            var cl = new List<ConstructorInfo>();
            foreach(var info in cs)
            {
                if(info.IsFamily)
                {
                    cl.Add(info);
                }
            }
            Assert.AreEqual(1, cl.Count());
        }

        [Test]
        public void TestPublicOrProtectedConstructors()
        {
            Type t = typeof(MyClass);
            var cs = t.GetConstructors(ClassHelper.InstanceFlag);
            var cl = new List<ConstructorInfo>();
            foreach (var info in cs)
            {
                if (info.IsFamily || info.IsPublic)
                {
                    cl.Add(info);
                }
            }
            Assert.AreEqual(2, cl.Count());
        }
    }
}
