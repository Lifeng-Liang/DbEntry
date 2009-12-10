using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    #region objects

    [SoftDelete, DbTable("SoftDelete")]
    public abstract class SoftDelete : DbObjectModel<SoftDelete>
    {
        public abstract string Name { get; set; }

        public SoftDelete Init(string name)
        {
            this.Name = name;
            return this;
        }
    }

    [SoftDelete]
    public abstract class SoftDeleteIndex : DbObjectModel<SoftDeleteIndex>
    {
        [Index(UNIQUE = true)]
        public abstract string Name { get; set; }

        public SoftDeleteIndex Init(string name)
        {
            this.Name = name;
            return this;
        }
    }

    [DbTable("SoftDelete")]
    public abstract class SoftDeleteFull : DbObjectModel<SoftDeleteFull>
    {
        public abstract string Name { get; set; }
        public abstract bool IsDeleted { get; set; }
    }

    [SoftDelete, DbTable("Tests")]
    public abstract class Test : DbObjectModel<Test>
    {
        [Length(100), StringColumn(IsUnicode = false)]
        public abstract string Nome { get; set; }

        public Test Init(string nome)
        {
            Nome = nome;
            return this;
        }
    }

    #endregion

    [TestFixture]
    public class SoftDeleteTest : DataTestBase
    {
        [Test]
        public void TestInsert()
        {
            SoftDelete o = SoftDelete.New.Init("test");
            o.Save();

            SoftDeleteFull o1 = SoftDeleteFull.FindById(o.Id);
            Assert.AreEqual("test", o1.Name);
            Assert.AreEqual(false, o1.IsDeleted);
        }

        [Test]
        public void TestRead()
        {
            List<SoftDelete> ls = SoftDelete.FindAll(new OrderBy("Id"));
            Assert.AreEqual(3, ls.Count);
            Assert.AreEqual("tom", ls[0].Name);
            Assert.AreEqual("jerry", ls[1].Name);
            Assert.AreEqual("mike", ls[2].Name);
        }

        [Test]
        public void TestUpdate()
        {
            SoftDelete o = SoftDelete.FindById(1);
            Assert.AreEqual("tom", o.Name);

            o.Name = "oh";
            o.Save();

            o = SoftDelete.FindById(1);
            Assert.AreEqual("oh", o.Name);
        }

        [Test]
        public void TestDelete()
        {
            SoftDelete o = SoftDelete.FindById(1);
            o.Delete();
            o = SoftDelete.FindById(1);
            Assert.IsNull(o);

            SoftDeleteFull o1 = SoftDeleteFull.FindById(1);
            Assert.IsNotNull(o1);
            Assert.AreEqual("tom", o1.Name);
            Assert.AreEqual(true, o1.IsDeleted);
        }

        [Test]
        public void TestDelete2()
        {
            var n = SoftDelete.GetCount(null);

            var o = SoftDelete.New.Init("aaa");
            o.Name = "bbb";
            o.Save();

            var o2 = SoftDelete.New;
            o2.Name = "ccc";
            o2.Save();

            var m = SoftDelete.GetCount(null);

            Assert.AreEqual(2, m - n);

            o2.Delete();

            m = SoftDelete.GetCount(null);

            Assert.AreEqual(1, m - n);
        }

        [Test]
        public void TestRead1()
        {
            SoftDelete o = SoftDelete.FindById(1);
            Assert.AreEqual("tom", o.Name);
            o = SoftDelete.FindById(4);
            Assert.IsNull(o);
        }

        [Test]
        public void TestCount()
        {
            long n = DbEntry.From<SoftDelete>().Where(WhereCondition.EmptyCondition).GetCount();
            Assert.AreEqual(3, n);
        }

        [Test]
        public void TestGroupBy()
        {
            var dc = new DbContext("SQLite");
            dc.From<SoftDelete>().Where(WhereCondition.EmptyCondition).GroupBy<string>("tom");
            Assert.AreEqual("CREATE TABLE [SoftDelete] (\n	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n	[Name] NTEXT NOT NULL ,\n	[IsDeleted] BOOL NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
            Assert.AreEqual("SELECT [tom],COUNT([tom]) AS it__count__ FROM [SoftDelete] WHERE [IsDeleted] = @IsDeleted_0 GROUP BY [tom];\n<Text><60>(@IsDeleted_0=False:Boolean)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreateTable()
        {
            var dc = new DbContext("SQLite");
            dc.Create(typeof(SoftDelete));
            Assert.AreEqual("CREATE TABLE [SoftDelete] (\n	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n	[Name] NTEXT NOT NULL ,\n	[IsDeleted] BOOL NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSoftDeleteOnlyWorksForTheRightOne()
        {
            DbEntry.Context.DropAndCreate(typeof(Test));

            var t = Test.New.Init("myName");
            t.Save();
            t = Test.New.Init("myName2");
            t.Save();
            t = Test.FindById(1);
            t.Delete();

            t = Test.FindById(1);
            Assert.IsNull(t);

            t = Test.FindById(2);
            Assert.IsNotNull(t);
            Assert.AreEqual("myName2", t.Nome);
        }

        [Test, Ignore("for now")]
        public void TestVerify()
        {
            var x = SoftDeleteIndex.New.Init("a");
            x.Save();
            x.Delete();

            var y = SoftDeleteIndex.New.Init("a");
            bool b = y.Validate().IsValid;
            Assert.IsFalse(b);
        }
    }
}
