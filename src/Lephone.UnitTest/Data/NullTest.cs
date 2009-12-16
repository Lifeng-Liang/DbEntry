using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    #region objects

    [DbTable("NullTest")]
    public abstract class NullableTable : DbObjectModel<NullableTable>
    {
        [AllowNull]
        public abstract string Name { get; set; }
        public abstract int? MyInt { get; set; }
        public abstract bool? MyBool { get; set; }
    }

    [DbTable("NullTest")]
    public abstract class NullTable : DbObjectModel<NullTable>
    {
        [AllowNull]
        public abstract string Name { get; set; }
        public abstract int MyInt { get; set; }
        public abstract bool MyBool { get; set; }
    }

    [DbTable("NullTest")]
    public abstract class NullableTableLazyInt : DbObjectModel<NullableTableLazyInt>
    {
        [AllowNull]
        public abstract string Name { get; set; }
        [LazyLoad]
        public abstract int? MyInt { get; set; }
        public abstract bool? MyBool { get; set; }
    }

    [DbTable("NullTest")]
    public abstract class NullableTableLazyString : DbObjectModel<NullableTableLazyString>
    {
        [LazyLoad, AllowNull]
        public abstract string Name { get; set; }
        public abstract int? MyInt { get; set; }
        public abstract bool? MyBool { get; set; }
    }

    #endregion

    [TestFixture]
    public class NullTest : DataTestBase
    {
        [Test]
        public void TestNullTableWithNonNullValue()
        {
            NullTable o = NullTable.FindById(4);
            Assert.IsNotNull(o);
            Assert.AreEqual(4, o.Id);
            Assert.AreEqual("tom", o.Name);
            Assert.AreEqual(1, o.MyInt);
            Assert.AreEqual(true, o.MyBool);
        }

        [Test]
        public void TestNullString()
        {
            NullTable o = NullTable.FindById(2);
            Assert.IsNotNull(o);
            Assert.AreEqual(2, o.Id);
            Assert.AreEqual(null, o.Name);
            Assert.AreEqual(1, o.MyInt);
            Assert.AreEqual(false, o.MyBool);
        }

        [Test]
        public void Test1()
        {
            NullableTable o = NullableTable.FindById(1);
            Assert.IsNotNull(o);
            Assert.AreEqual(1, o.Id);
            Assert.AreEqual("tom", o.Name);
            Assert.IsTrue(null == o.MyInt);
            Assert.IsTrue(true == o.MyBool);
        }

        [Test]
        public void Test2()
        {
            NullableTable o = NullableTable.FindById(2);
            Assert.IsNotNull(o);
            Assert.AreEqual(2, o.Id);
            Assert.AreEqual(null, o.Name);
            Assert.IsTrue(1 == o.MyInt);
            Assert.IsTrue(false == o.MyBool);
        }

        [Test]
        public void Test3()
        {
            NullableTable o = NullableTable.FindById(3);
            Assert.IsNotNull(o);
            Assert.AreEqual(3, o.Id);
            Assert.AreEqual(null, o.Name);
            Assert.IsTrue(null == o.MyInt);
            Assert.IsTrue(null == o.MyBool);
        }

        [Test]
        public void Test4()
        {
            NullableTable o = NullableTable.FindById(4);
            Assert.IsNotNull(o);
            Assert.AreEqual(4, o.Id);
            Assert.AreEqual("tom", o.Name);
            Assert.IsTrue(1 == o.MyInt);
            Assert.IsTrue(true == o.MyBool);
        }

        [Test]
        public void Test3WithSQL()
        {
            List<NullableTable> ls = DbEntry.Context.ExecuteList<NullableTable>(
                "select * from NullTest where Id = 3");
            Assert.AreEqual(1, ls.Count);
            NullableTable o = ls[0];
            Assert.IsNotNull(o);
            Assert.AreEqual(3, o.Id);
            Assert.AreEqual(null, o.Name);
            Assert.IsTrue(null == o.MyInt);
            Assert.IsTrue(null == o.MyBool);
        }

        [Test]
        public void TestCRUD()
        {
            NullableTable o = NullableTable.New;
            o.Name = null;
            o.MyInt = null;
            o.MyBool = null;
            o.Save();

            NullableTable o1 = NullableTable.FindById(o.Id);
            Assert.AreEqual(null, o1.Name);
            Assert.IsTrue(null == o1.MyInt);
            Assert.IsTrue(null == o1.MyBool);

            o1.Name = "jerry";
            o1.MyInt = 11;
            o1.Save();

            NullableTable o2 = NullableTable.FindById(o.Id);
            Assert.AreEqual("jerry", o2.Name);
            Assert.IsTrue(11 == o2.MyInt);
            Assert.IsTrue(null == o2.MyBool);

            o2.MyInt = 18;
            o2.MyBool = true;
            o2.Save();

            NullableTable o3 = NullableTable.FindById(o.Id);
            Assert.AreEqual("jerry", o3.Name);
            Assert.IsTrue(18 == o3.MyInt);
            Assert.IsTrue(true == o3.MyBool);

            o3.Name = null;
            o3.MyInt = null;
            o3.MyBool = null;
            o3.Save();

            NullableTable o4 = NullableTable.FindById(o.Id);
            Assert.AreEqual(null, o4.Name);
            Assert.IsTrue(null == o4.MyInt);
            Assert.IsTrue(null == o4.MyBool);

            o4.Delete();

            NullableTable o5 = NullableTable.FindById(o.Id);
            Assert.IsNull(o5);
        }

        [Test]
        public void TestLazyInt()
        {
            NullableTableLazyInt o = NullableTableLazyInt.FindById(1);
            Assert.IsNotNull(o);
            Assert.AreEqual(1, o.Id);
            Assert.AreEqual("tom", o.Name);
            Assert.IsTrue(null == o.MyInt);
            Assert.IsTrue(true == o.MyBool);
        }

        [Test]
        public void TestLazyString()
        {
            NullableTableLazyString o = NullableTableLazyString.FindById(2);
            Assert.IsNotNull(o);
            Assert.AreEqual(2, o.Id);
            Assert.AreEqual(null, o.Name);
            Assert.IsTrue(1 == o.MyInt);
            Assert.IsTrue(false == o.MyBool);
        }

        [Test]
        public void TestEmptyString()
        {
            var o = NullableTable.New;
            o.Name = "";
            o.Save();

            var o1 = NullableTable.FindById(o.Id);
            Assert.IsNotNull(o1);
            Assert.IsNotNull(o1.Name);
            Assert.AreEqual("", o1.Name);
        }
    }
}
