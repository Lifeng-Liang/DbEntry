using System.Threading;
using Lephone.Util;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class FlyweightTest
    {
        public class Flyweight : FlyweightBase<string, Flyweight>
        {
            public static int Count;

            protected override void Init(string t)
            {
                Count++;
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void Test1()
        {
            Flyweight.Count = 0;

            var t1 = new Thread(DoWork);
            var t2 = new Thread(DoWork);

            t1.Start();
            t2.Start();

            Thread.Sleep(2000);
            Assert.AreEqual(1, Flyweight.Count);
        }

        public static void DoWork()
        {
            Flyweight.GetInstance("test");
        }
    }
}
