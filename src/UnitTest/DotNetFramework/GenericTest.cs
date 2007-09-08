
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

#endregion

namespace Lephone.UnitTest.Framework
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
    }
}
