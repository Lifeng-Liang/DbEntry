using System;
using Leafing.Core.Ioc;
using NUnit.Framework;

namespace Leafing.UnitTest.util {
    [DependenceEntry]
    public interface ITest {
        string Run();
    }

    [Implementation(1, "test")]
    public class TestImpl : ITest {
        public string Run() {
            return "1st impl";
        }
    }

    [Implementation(2, "test2")]
    public class NewTestImpl : ITest {
        public string Run() {
            return "2nd impl";
        }
    }

    [Implementation("test3")]
    public class NewNewTestImpl : ITest {
        public string Run() {
            return "3rd impl";
        }
    }

    [DependenceEntry, Implementation(1)]
    public class IocSame {
        public virtual string Run() {
            return "same";
        }

        [Injection(2)]
        public ITest TestProperty { get; set; }
    }

    [Implementation(3)]
    public class IocSameSub : IocSame {
        public override string Run() {
            return "sub class";
        }
    }

    public class IocSameReg : IocSame {
        public override string Run() {
            return "reg class";
        }
    }

    [DependenceEntry, Implementation(1)]
    public class IocConstractor {
        private readonly ITest _test;

        public IocConstractor([Injection(2)]ITest test) {
            this._test = test;
        }

        public virtual string Run() {
            return _test.Run();
        }
    }

    [TestFixture]
    public class IocTest {
        [Test]
        public void Test1() {
            var t = SimpleContainer.Get<ITest>();
            Assert.IsNotNull(t);
            Assert.IsTrue(t is NewTestImpl);

            var t1 = SimpleContainer.Get<ITest>(1);
            Assert.IsNotNull(t1);
            Assert.IsTrue(t1 is TestImpl);

            var t2 = SimpleContainer.Get<ITest>(2);
            Assert.IsNotNull(t2);
            Assert.IsTrue(t2 is NewTestImpl);

            var t3 = SimpleContainer.Get<IocSame>(1);
            Assert.IsNotNull(t3);
            Assert.AreEqual("same", t3.Run());

            var t4 = SimpleContainer.Get<IocSame>(3);
            Assert.IsNotNull(t4);
            Assert.IsTrue(t4 is IocSameSub);
            Assert.AreEqual("sub class", t4.Run());
        }

        [Test]
        public void Test2() {
            SimpleContainer.Register(typeof(IocSame), typeof(IocSameReg), 4, null);
            var t = SimpleContainer.Get<IocSame>(4);
            Assert.IsNotNull(t);
            Assert.AreEqual("reg class", t.Run());
            Assert.IsNotNull(t.TestProperty);
            Assert.AreEqual("2nd impl", t.TestProperty.Run());
        }

        [Test]
        public void Test3() {
            Assert.Throws<ArgumentException>(() => {
                SimpleContainer.Register(typeof(ITest), typeof(ITest), 7, null);
            });
        }

        [Test]
        public void Test4() {
            var item = SimpleContainer.Get<IocConstractor>();
            Assert.IsNotNull(item);
            Assert.AreEqual("2nd impl", item.Run());
        }

        [Test]
        public void TestName() {
            var item = SimpleContainer.Get<ITest>("test");
            Assert.IsNotNull(item);
            Assert.AreEqual("1st impl", item.Run());

            item = SimpleContainer.Get<ITest>("test2");
            Assert.IsNotNull(item);
            Assert.AreEqual("2nd impl", item.Run());

            item = SimpleContainer.Get<ITest>("test3");
            Assert.IsNotNull(item);
            Assert.AreEqual("3rd impl", item.Run());
        }
    }
}