using System;
using NUnit.Framework;

namespace Leafing.UnitTest.DotNetFramework
{
    [TestFixture]
    public class MathTest
    {
        [Test]
        public void Test1()
        {
            var n = Math.Floor(5f/2);
            Assert.AreEqual(2f, n);

            n = Math.Round(5f/2);
            Assert.AreEqual(2f, n);

            n = Math.Round(29f/10);
            Assert.AreEqual(3f, n);
        }

        [Test]
        public void Test2()
        {
            var n = (long)Math.Floor((double)(-1) / 10) + 1;
            Assert.AreEqual(0, n);

            n = (long)Math.Floor((double)(0) / 10) + 1;
            Assert.AreEqual(1, n);

            n = (long)Math.Floor((double)(9) / 10) + 1;
            Assert.AreEqual(1, n);

            n = (long)Math.Floor((double)(10) / 10) + 1;
            Assert.AreEqual(2, n);
        }
    }
}
