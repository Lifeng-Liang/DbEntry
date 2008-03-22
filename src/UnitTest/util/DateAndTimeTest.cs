using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class DateAndTimeTest
    {
        [Test]
        public void Test1()
        {
            Date d1 = new Date(2001, 1, 5);
            Date d2 = new Date(2001, 2, 1);
            Date d3 = new Date(2001, 1, 5);
            Assert.IsTrue(d1 < d2);
            Assert.IsTrue(d2 > d3);
            Assert.IsTrue(d1 == d3);
            Assert.IsTrue(d2 >= d3);
        }

        [Test]
        public void Test2()
        {
            Time t1 = new Time(3, 45, 23);
            Time t2 = new Time(6, 01, 33);
            Time t3 = new Time(3, 45, 23);
            Assert.IsTrue(t1 < t2);
            Assert.IsTrue(t2 > t3);
            Assert.IsTrue(t1 == t3);
            Assert.IsTrue(t2 >= t3);
        }
    }
}
