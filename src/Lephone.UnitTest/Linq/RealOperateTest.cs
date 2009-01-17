using System.Collections.Generic;
using System.Linq;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Linq;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Linq
{
    [TestFixture]
    public class RealOperateTest : DataTestBase
    {
        [DbTable("People")]
        public abstract class Person : LinqObjectModel<Person>
        {
            [DbColumn("Name")]
            public abstract string FirstName { get; set; }
        }

        [DbTable("Categories")]
        public abstract class lCategory : LinqObjectModel<lCategory>
        {
            public abstract string Name { get; set; }
            [HasMany] public abstract IList<lBook> Books { get; set; }
        }

        [DbTable("Books")]
        public abstract class lBook : LinqObjectModel<lBook>
        {
            public abstract string Name { get; set; }
            [BelongsTo, DbColumn("Category_Id")] public abstract lCategory Category { get; set; }
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
