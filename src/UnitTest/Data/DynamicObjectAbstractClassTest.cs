
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using NUnit.Framework;

using org.hanzify.llf.util;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Definition;
using org.hanzify.llf.UnitTest.Data.Objects;

#endregion

namespace org.hanzify.llf.UnitTest.Data
{
    [TestFixture]
    public class DynamicObjectAbstractClassTest
    {
        [Test]
        public void TestCreateAbstractClass()
        {
            AbstractClass c = DynamicObject.NewObject<AbstractClass>();
            Assert.IsNotNull(c);
            c.Name = "Tom";
            Assert.AreEqual("Tom", c.Name);
        }

        [Test]
        public void TestCreateInheritedAbstractClass()
        {
            AbstractClassOfAge c = DynamicObject.NewObject<AbstractClassOfAge>();
            Assert.IsNotNull(c);
            c.Name = "Tom";
            c.Age = 18;
            Assert.AreEqual("Tom", c.Name);
            Assert.AreEqual(18, c.Age);
        }

        [Test]
        public void TestCreateInheritsedAbstractClassWithImplProperty()
        {
            AbstractClassWithOneImplProperty c = DynamicObject.NewObject<AbstractClassWithOneImplProperty>();
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
            Type t1 = DynamicObject.GetImplType(typeof(AbstractClassWithOneImplProperty));
            Type t2 = DynamicObject.GetImplType(typeof(AbstractClassWithOneImplProperty));
            Assert.AreEqual(t1, t2);
        }

        [Test]
        public void TestForObjectAttribute()
        {
            Type t = DynamicObject.GetImplType(typeof(AbstractClass));

            object[] ats = t.GetCustomAttributes(false);
            Assert.AreEqual(1, ats.Length);
            Assert.IsTrue(ats[0] is DbTableAttribute);
            DbTableAttribute da = (DbTableAttribute)ats[0];
            Assert.AreEqual("AbstractClass", da.TableName);
        }

        [Test]
        public void TestCreateObjectByParams()
        {
            AbstractClass c = DynamicObject.NewObject<AbstractClass>("abs");
            Assert.IsNotNull(c);
            Assert.AreEqual("abs", c.Name);
        }

        [Test]
        public void TestCreateObjectByMutilParams()
        {
            DateTime dt = DateTime.Now;
            ConstructorBase c = DynamicObject.NewObject<ConstructorBase>("a1", 11, "a2", 22, true, dt);
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
            Type t = DynamicObject.GetImplType(typeof(NamedClass));

            object[] ats = t.GetCustomAttributes(false);
            Assert.IsNotNull(ats);
            Assert.AreEqual(1, ats.Length);
            Assert.IsTrue(ats[0] is DbTableAttribute);
            DbTableAttribute da = (DbTableAttribute)ats[0];
            Assert.IsNull(da.LinkNames);
            Assert.AreEqual("abc", da.TableName);
        }

        [Test]
        public void TestJoinByDbTableClass()
        {
            Type t = DynamicObject.GetImplType(typeof(JoinByDbTableClass));

            object[] ats = t.GetCustomAttributes(false);
            Assert.IsNotNull(ats);
            Assert.AreEqual(1, ats.Length);
            Assert.IsTrue(ats[0] is DbTableAttribute);
            DbTableAttribute da = (DbTableAttribute)ats[0];
            Assert.IsNull(da.TableName);
            Assert.AreEqual(2, da.LinkNames.Length);
            Assert.AreEqual("abc", da.LinkNames[0]);
            Assert.AreEqual("xyz", da.LinkNames[1]);
        }

        [Test]
        public void TestSerializableClass()
        {
            Type t = DynamicObject.GetImplType(typeof(SerializableClass));

            object[] ats = t.GetCustomAttributes(false);
            Assert.IsNotNull(ats);
            Assert.AreEqual(2, ats.Length);
            Assert.IsTrue(ats[0] is SerializableAttribute || ats[1] is SerializableAttribute);
        }

        [Test]
        public void TestJoinClass()
        {
            Type t = DynamicObject.GetImplType(typeof(JoinClass));
            object[] js = t.GetCustomAttributes(false);
            Assert.AreEqual(2, js.Length);
            JoinOnAttribute j = (JoinOnAttribute)js[0];
            int a1 = 0;
            int a2 = 1;
            if (j.Index == 2)
            {
                a1 = 1;
                a2 = 0;
            }
            j = (JoinOnAttribute)js[a1];
            Assert.AreEqual(1, j.Index);
            Assert.AreEqual("a1", j.joinner.Key1);
            Assert.AreEqual("a2", j.joinner.Key2);
            Assert.AreEqual(CompareOpration.LessOrEqual, j.joinner.comp);
            Assert.AreEqual(JoinMode.Left, j.joinner.mode);

            j = (JoinOnAttribute)js[a2];
            Assert.AreEqual(2, j.Index);
            Assert.AreEqual("b1", j.joinner.Key1);
            Assert.AreEqual("b2", j.joinner.Key2);
            Assert.AreEqual(CompareOpration.Like, j.joinner.comp);
            Assert.AreEqual(JoinMode.Right, j.joinner.mode);
        }
    }
}
