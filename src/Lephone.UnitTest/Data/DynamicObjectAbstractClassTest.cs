using System;
using Lephone.Data.Common;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class DynamicObjectAbstractClassTest
    {
        [Test]
        public void TestCreateAbstractClass()
        {
            var c = DynamicObjectBuilder.Instance.NewObject<AbstractClass>();
            Assert.IsNotNull(c);
            c.Name = "Tom";
            Assert.AreEqual("Tom", c.Name);
        }

        [Test]
        public void TestCreateInheritedAbstractClass()
        {
            var c = DynamicObjectBuilder.Instance.NewObject<AbstractClassOfAge>();
            Assert.IsNotNull(c);
            c.Name = "Tom";
            c.Age = 18;
            Assert.AreEqual("Tom", c.Name);
            Assert.AreEqual(18, c.Age);
        }

        [Test]
        public void TestCreateInheritsedAbstractClassWithImplProperty()
        {
            var c = DynamicObjectBuilder.Instance.NewObject<AbstractClassWithOneImplProperty>();
            Assert.IsNotNull(c);
            c.Name = "Tom";
            c.Age = 18;
            Assert.AreEqual("Tom", c.Name);
            Assert.AreEqual(18, c.Age);

            c.Gender = true;
            Assert.AreEqual(false, c.Gender);
        }

        [Test]
        public void TestTwoTimesAsTheSameType()
        {
            Type t1 = AssemblyHandler.Instance.GetImplType(typeof(AbstractClassWithOneImplProperty));
            Type t2 = AssemblyHandler.Instance.GetImplType(typeof(AbstractClassWithOneImplProperty));
            Assert.AreEqual(t1, t2);
        }

        [Test]
        public void TestCreateObjectByParams()
        {
            var c = DynamicObjectBuilder.Instance.NewObject<AbstractClass>("abs");
            Assert.IsNotNull(c);
            Assert.AreEqual("abs", c.Name);
        }

        [Test]
        public void TestSerializableClass()
        {
            Type t = AssemblyHandler.Instance.GetImplType(typeof(SerializableClass));

            object[] ats = t.GetCustomAttributes(false);
            Assert.IsNotNull(ats);
            Assert.AreEqual(1, ats.Length);
            Assert.IsTrue(ats[0] is SerializableAttribute);
        }
    }
}
