using System;
using NUnit.Framework;

namespace Lephone.UnitTest.DotNetFramework
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
    }
}
