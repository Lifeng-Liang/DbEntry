using Lephone.Data.Common;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.DotNetFramework
{
    class test
    {
        public int? n;
    }

    public abstract class test2 : IDbObject
    {
        public abstract int? n { get; set; }
    }

    [TestFixture]
    public class NullabelTest
    {
        [Test]
        public void Test1()
        {
            test o = new test();
            Assert.IsFalse(o.n.HasValue);
        }

        [Test]
        public void Test2()
        {
            var o = DynamicObjectBuilder.Instance.NewObject<test2>();
            Assert.IsFalse(o.n.HasValue);
        }

        [Test]
        public void Test3()
        {
            bool? n = false;
            Assert.IsTrue(false == n);
            Assert.IsTrue(null != n);
        }
    }
}
