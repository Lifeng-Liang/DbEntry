using System;
using Lephone.Util.IoC;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [DependenceEntry]
    public interface ITest
    {
        string Run();
    }

    [Implementation]
    public class TestImpl : ITest
    {
        public string Run()
        {
            return "1st impl";
        }
    }

    [Implementation("2nd")]
    public class NewTestImpl : ITest
    {
        public string Run()
        {
            return "2nd impl";
        }
    }

    [DependenceEntry, Implementation]
    public class IocSame
    {
        public virtual string Run()
        {
            return "same";
        }

        [Injection("2nd")]
        public ITest TestProperty { get; set; }
    }

    [Implementation("sub")]
    public class IocSameSub : IocSame
    {
        public override string Run()
        {
            return "sub class";
        }
    }

    public class IocSameReg : IocSame
    {
        public override string Run()
        {
            return "reg class";
        }
    }

    [DependenceEntry, Implementation]
    public class IocConstractor
    {
        private readonly ITest test;

        public IocConstractor([Injection("2nd")]ITest test)
        {
            this.test = test;
        }

        public virtual string Run()
        {
            return test.Run();
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
            Assert.AreEqual("sub class", t4.Run());
        }

        [Test]
        public void Test2()
        {
            SimpleContainer.Register<IocSame, IocSameReg>("reg");
            var t = SimpleContainer.Get<IocSame>("reg");
            Assert.IsNotNull(t);
            Assert.AreEqual("reg class", t.Run());
            Assert.IsNotNull(t.TestProperty);
            Assert.AreEqual("2nd impl", t.TestProperty.Run());
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void Test3()
        {
            SimpleContainer.Register<ITest, ITest>("error");
        }

        [Test]
        public void Test4()
        {
            var item = SimpleContainer.Get<IocConstractor>();
            Assert.IsNotNull(item);
            Assert.AreEqual("2nd impl", item.Run());
        }
    }
}