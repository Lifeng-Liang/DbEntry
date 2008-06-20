using System;
using Lephone.Util.Ioc;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [IocEntry]
    public interface ITest
    {
        void Run();
    }

    [IocImpl]
    public class TestImpl : ITest
    {
        #region ITest Members

        public void Run()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [IocImpl("2nd")]
    public class NewTestImpl : ITest
    {
        #region ITest Members

        public void Run()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [IocEntry]
    [IocImpl]
    public class IocSame
    {
        public virtual string Run()
        {
            return "same";
        }
    }

    [IocImpl("sub")]
    public class IocSameSub : IocSame
    {
        public override string Run()
        {
            return "sub";
        }
    }

    [TestFixture]
    public class IocTest
    {
        [Test]
        public void Test1()
        {
            var t = SimpleContainer.Get<ITest>();
            Assert.IsNotNull(t);
            Assert.IsTrue(t is TestImpl);

            var t2 = SimpleContainer.Get<ITest>("2nd");
            Assert.IsNotNull(t2);
            Assert.IsTrue(t2 is NewTestImpl);

            var t3 = SimpleContainer.Get<IocSame>();
            Assert.IsNotNull(t3);
            Assert.AreEqual("same", t3.Run());

            var t4 = SimpleContainer.Get<IocSame>("sub");
            Assert.IsNotNull(t4);
            Assert.IsTrue(t4 is IocSameSub);
            Assert.AreEqual("sub", t4.Run());
        }
    }
}