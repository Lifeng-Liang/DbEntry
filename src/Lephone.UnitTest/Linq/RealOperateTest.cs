using System;
using System.Collections.Generic;
using System.Linq;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Linq
{
    [TestFixture]
    public class RealOperateTest : DataTestBase
    {
        [DbTable("People")]
        public class Person : DbObjectModel<Person>
        {
            [DbColumn("Name")]
            public string FirstName { get; set; }
        }

        [DbTable("Categories")]
        public class lCategory : DbObjectModel<lCategory>
        {
            public string Name { get; set; }
            [HasMany] public IList<lBook> Books { get; private set; }
        }

        [DbTable("Books")]
        public class lBook : DbObjectModel<lBook>
        {
            public string Name { get; set; }
            [BelongsTo, DbColumn("Category_Id")] public lCategory Category { get; set; }
        }

        [DbTable("People")]
        public class MyTable : IDbObject
        {
            [DbKey] public long Id { get; set; }
            public string Name { get; set; }
        }

        [DbTable("People")]
        public class MyTable2 : IDbObject
        {
            [DbKey] public long Id;
            public string Name;
        }

        public class Tst : DbObjectModel<Tst>
        {
            public DateTime Dt { get; set; }
        }

        [Test]
        public void TestDateTime()
        {
            var t = Tst.Find(p => p.Dt <= DateTime.Now);
        }

        [Test]
        public void Test15()
        {
            var list = from s in Person.Table select s;
            Assert.AreEqual(3, list.ToArray().Length);
        }

        [Test]
        public void Test16()
        {
            var list = from s in Person.Table where s.Id == 1 select s;
            Assert.AreEqual(1, list.ToArray().Length);
            Assert.AreEqual("Tom", list.ToArray()[0].FirstName);
        }

        [Test]
        public void Test3()
        {
            var list = Person.Find(p => p.FirstName.Contains("T"));
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Tom", list[0].FirstName);
        }

        [Test]
        public void Test3_2()
        {
            var list = Person.Find(p => !p.FirstName.Contains("T"), p => p.Id);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Jerry", list[0].FirstName);
            Assert.AreEqual("Mike", list[1].FirstName);
        }

        [Test]
        public void Test17()
        {
            var list = lBook.Find(p => p.Category.Id == 3, p => p.Id);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("Beijing", list[0].Name);
            Assert.AreEqual("Shanghai", list[1].Name);
        }

        [Test]
        public void Test18()
        {
            var list = Person.Find(p => p.FirstName == "Tom");
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("Tom", list[0].FirstName);
        }

        [Test]
        public void TestIDbObject()
        {
            var list = DbEntry.From<MyTable>().Where(p => p.Name == "Mike").Select();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(3, list[0].Id);
        }

        [Test]
        public void TestIDbObject2()
        {
            var list = DbEntry.From<MyTable2>().Where(p => p.Name == "Mike").Select();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(3, list[0].Id);
        }
    }
}
