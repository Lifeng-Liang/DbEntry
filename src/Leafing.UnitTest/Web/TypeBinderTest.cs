using System;
using Leafing.Data.Definition;
using Leafing.UnitTest.Data.Objects;
using Leafing.UnitTest.Mocks;
using Leafing.Web.Mvc.Core;
using NUnit.Framework;

namespace Leafing.UnitTest.Web
{
    public class MyClass
    {
        public string Name;
        public int Age;
    }

    [DbTable("SoftDelete")]
    public class BoolTest : DbObjectModel<BoolTest>
    {
        public string Name { get; set; }

        [DbColumn("IsDeleted")]
        public bool Visable { get; set; }
    }

    [TestFixture]
    public class TypeBinderTest : DataTestBase
    {
        protected override void OnSetUp()
        {
            MockHttpContextHandler.Request.Clear();
            base.OnSetUp();
        }

        [Test]
        public void TessOtherType()
        {
            MockHttpContextHandler.Request["Name"] = "tom";
            MockHttpContextHandler.Request["Age"] = "140";
            var n = (MyClass)TypeBinder.Instance.GetObject("MyClass", typeof(MyClass));
            Assert.IsNotNull(n);
            Assert.AreEqual("tom", n.Name);
            Assert.AreEqual(140, n.Age);
        }

        [Test]
        public void TestSimple()
        {
            MockHttpContextHandler.Request["test"] = "140";
            var n = TypeBinder.Instance.GetObject("test", typeof(int));
            Assert.AreEqual(140, n);

            MockHttpContextHandler.Request["test"] = "0";
            n = TypeBinder.Instance.GetObject("test", typeof(int));
            Assert.AreEqual(0, n);
        }

        [Test, ExpectedException(typeof(FormatException))]
        public void TestSimpleWithException()
        {
            MockHttpContextHandler.Request["test"] = "";
            TypeBinder.Instance.GetObject("test", typeof(int));
        }

        [Test]
        public void TestString()
        {
            MockHttpContextHandler.Request["test"] = "140";
            var n = TypeBinder.Instance.GetObject("test", typeof(string));
            Assert.AreEqual("140", n);

            MockHttpContextHandler.Request["test"] = "0";
            n = TypeBinder.Instance.GetObject("test", typeof(string));
            Assert.AreEqual("0", n);

            MockHttpContextHandler.Request["test"] = "";
            n = TypeBinder.Instance.GetObject("test", typeof(string));
            Assert.AreEqual("", n);
        }

        [Test, ExpectedException(typeof(NotSupportedException), ExpectedMessage = "The type [System.Byte] is not supported")]
        public void TestNotSupported()
        {
            TypeBinder.Instance.GetObject("test", typeof(byte));
        }

        [Test]
        public void TestSimpleNullable()
        {
            MockHttpContextHandler.Request["test"] = "140";
            var n = TypeBinder.Instance.GetObject("test", typeof(int?));
            Assert.AreEqual(140, n);

            MockHttpContextHandler.Request["test"] = "0";
            n = TypeBinder.Instance.GetObject("test", typeof(int?));
            Assert.AreEqual(0, n);

            MockHttpContextHandler.Request["test"] = "";
            n = TypeBinder.Instance.GetObject("test", typeof(int?));
            Assert.AreEqual(null, n);
        }

        [Test]
        public void TestNewModel()
        {
            MockHttpContextHandler.Request["Name"] = "next123";
            var n = (People)TypeBinder.Instance.GetObject("ewr", typeof(People));
            Assert.AreEqual(0, n.Id);
            n.Save();
            var m = People.FindById(n.Id);
            Assert.AreEqual("next123", m.Name);
        }

        [Test]
        public void TestUpdateModel()
        {
            var m = People.FindById(1);
            Assert.AreEqual("Tom", m.Name);

            MockHttpContextHandler.Request["Id"] = "1";
            MockHttpContextHandler.Request["Name"] = "next123";
            var n = (People)TypeBinder.Instance.GetObject("x", typeof(People));
            n.Save();
            m = People.FindById(n.Id);
            Assert.AreEqual("next123", m.Name);

            MockHttpContextHandler.Request["Id"] = "1";
            MockHttpContextHandler.Request["Name"] = "";
            n = (People)TypeBinder.Instance.GetObject("x", typeof(People));
            n.Save();
            m = People.FindById(n.Id);
            Assert.AreEqual("", m.Name);
        }

        [Test]
        public void TestBoolTest()
        {
            var o = BoolTest.FindById(1);
            Assert.AreEqual("tom", o.Name);
            Assert.IsFalse(o.Visable);

            MockHttpContextHandler.Request["Id"] = "1";
            MockHttpContextHandler.Request["Name"] = "next123";
            MockHttpContextHandler.Request["Visable"] = "on";
            o = (BoolTest)TypeBinder.Instance.GetObject("x", typeof(BoolTest));
            o.Save();
            Assert.AreEqual("next123", o.Name);
            Assert.IsTrue(o.Visable);

            MockHttpContextHandler.Request["Id"] = "1";
            MockHttpContextHandler.Request["Name"] = "123";
            MockHttpContextHandler.Request["Visable"] = null;
            o = (BoolTest)TypeBinder.Instance.GetObject("x", typeof(BoolTest));
            o.Save();
            Assert.AreEqual("123", o.Name);
            Assert.IsFalse(o.Visable);
        }
    }
}
