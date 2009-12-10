using System;
using Lephone.Data.Common;
using Lephone.Data.Definition;
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
        public void TestForObjectAttribute()
        {
            Type t = AssemblyHandler.Instance.GetImplType(typeof(AbstractClass));

            object[] ats = t.GetCustomAttributes(false);
            Assert.AreEqual(1, ats.Length);
            Assert.IsTrue(ats[0] is DbTableAttribute);
            var da = (DbTableAttribute)ats[0];
            Assert.AreEqual("Abstract_Class", da.TableName);
        }

        [Test]
        public void TestCreateObjectByParams()
        {
            var c = DynamicObjectBuilder.Instance.NewObject<AbstractClass>("abs");
            Assert.IsNotNull(c);
            Assert.AreEqual("abs", c.Name);
        }

        [Test]
        public void TestCreateObjectByMutilParams()
        {
            DateTime dt = DateTime.Now;
            var c = DynamicObjectBuilder.Instance.NewObject<ConstructorBase>("a1", 11, "a2", 22, true, dt);
            Assert.IsNotNull(c);
            Assert.AreEqual("a1", c.p1);
            Assert.AreEqual(11, c.p2);
            Assert.AreEqual("a2", c.p3);
            Assert.AreEqual(22, c.p4);
            Assert.AreEqual(true, c.p5);
            Assert.AreEqual(dt, c.p6);
        }

        [Test]
        public void TestNamedClass()
        {
            Type t = AssemblyHandler.Instance.GetImplType(typeof(NamedClass));

            object[] ats = t.GetCustomAttributes(false);
            Assert.IsNotNull(ats);
            Assert.AreEqual(1, ats.Length);
            Assert.IsTrue(ats[0] is DbTableAttribute);
            var da = (DbTableAttribute)ats[0];
            Assert.AreEqual("abc", da.TableName);
        }

        [Test]
        public void TestSerializableClass()
        {
            Type t = AssemblyHandler.Instance.GetImplType(typeof(SerializableClass));

            object[] ats = t.GetCustomAttributes(false);
            Assert.IsNotNull(ats);
            Assert.AreEqual(2, ats.Length);
            Assert.IsTrue(ats[0] is SerializableAttribute || ats[1] is SerializableAttribute);
        }

        [Test]
        public void TestJoinClass()
        {
            Type t = AssemblyHandler.Instance.GetImplType(typeof(JoinClass));
            object[] js = t.GetCustomAttributes(false);
            Assert.AreEqual(2, js.Length);
            var j = (JoinOnAttribute)js[0];
            int a1 = 0;
            int a2 = 1;
            if (j.Index == 2)
            {
                a1 = 1;
                a2 = 0;
            }
            j = (JoinOnAttribute)js[a1];
            Assert.AreEqual(1, j.Index);
            Assert.AreEqual("abc", j.Joinner.Table1);
            Assert.AreEqual("a1", j.Joinner.Key1);
            Assert.AreEqual("Abstract_Class_Of_Age", j.Joinner.Table2);
            Assert.AreEqual("a2", j.Joinner.Key2);
            Assert.AreEqual(CompareOpration.LessOrEqual, j.Joinner.Comp);
            Assert.AreEqual(JoinMode.Left, j.Joinner.Mode);

            j = (JoinOnAttribute)js[a2];
            Assert.AreEqual(2, j.Index);
            Assert.AreEqual("abc", j.Joinner.Table1);
            Assert.AreEqual("b1", j.Joinner.Key1);
            Assert.AreEqual("Serializable_Class", j.Joinner.Table2);
            Assert.AreEqual("b2", j.Joinner.Key2);
            Assert.AreEqual(CompareOpration.Like, j.Joinner.Comp);
            Assert.AreEqual(JoinMode.Right, j.Joinner.Mode);
        }
    }
}
