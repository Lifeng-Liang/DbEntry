﻿using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.Driver;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    #region objects

    [SoftDelete, DbTable("SoftDelete")]
    public class SoftDelete : DbObjectModel<SoftDelete>
    {
        public string Name { get; set; }
    }

    [SoftDelete, DbTable("SoftDelete"), DbContext("SQLite")]
    public class SoftDeleteSqlite : DbObjectModel<SoftDeleteSqlite>
    {
        public string Name { get; set; }
    }

    [SoftDelete]
    public class SoftDeleteIndex : DbObjectModel<SoftDeleteIndex>
    {
        [Index(UNIQUE = true)]
        public string Name { get; set; }
    }

    [DbTable("SoftDelete")]
    public class SoftDeleteFull : DbObjectModel<SoftDeleteFull>
    {
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    [SoftDelete, DbTable("Tests")]
    public class Test : DbObjectModel<Test>
    {
        [Length(100), StringColumn(IsUnicode = false)]
        public string Nome { get; set; }
    }

    #endregion

    [TestFixture]
    public class SoftDeleteTest : DataTestBase
    {
        [Test]
        public void TestInsert()
        {
            var o = new SoftDelete {Name = "test"};
            o.Save();

            SoftDeleteFull o1 = SoftDeleteFull.FindById(o.Id);
            Assert.AreEqual("test", o1.Name);
            Assert.AreEqual(false, o1.IsDeleted);
        }

        [Test]
        public void TestRead()
        {
            List<SoftDelete> ls = SoftDelete.Find(Condition.Empty, new OrderBy("Id"));
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
            var n = SoftDelete.GetCount(Condition.Empty);

            var o = new SoftDelete {Name = "aaa"};
            o.Name = "bbb";
            o.Save();

            var o2 = new SoftDelete {Name = "ccc"};
            o2.Save();

            var m = SoftDelete.GetCount(Condition.Empty);

            Assert.AreEqual(2, m - n);

            o2.Delete();

            m = SoftDelete.GetCount(Condition.Empty);

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
            long n = DbEntry.From<SoftDelete>().Where(Condition.Empty).GetCount();
            Assert.AreEqual(3, n);
        }

        [Test]
        public void TestGroupBy()
        {
            DbDriverFactory.Instance.GetInstance("SQLite").TableNames = null;
            DbEntry.From<SoftDeleteSqlite>().Where(Condition.Empty).GroupBy<string>("tom");
            Assert.AreEqual("CREATE TABLE [SoftDelete] (\n	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n	[Name] NTEXT NOT NULL ,\n	[IsDeleted] BOOL NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
            Assert.AreEqual("SELECT [tom],COUNT([tom]) AS it__count__ FROM [SoftDelete] WHERE [IsDeleted] = @IsDeleted_0 GROUP BY [tom];\n<Text><60>(@IsDeleted_0=False:Boolean)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreateTable()
        {
            DbDriverFactory.Instance.GetInstance("SQLite").TableNames = null;
            DbEntry.Create(typeof(SoftDeleteSqlite));
            Assert.AreEqual("CREATE TABLE [SoftDelete] (\n	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n	[Name] NTEXT NOT NULL ,\n	[IsDeleted] BOOL NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSoftDeleteOnlyWorksForTheRightOne()
        {
            DbEntry.DropAndCreate(typeof(Test));

            var t = new Test {Nome = "myName"};
            t.Save();
            t = new Test {Nome = "myName2"};
            t.Save();
            t = Test.FindById(1);
            t.Delete();

            t = Test.FindById(1);
            Assert.IsNull(t);

            t = Test.FindById(2);
            Assert.IsNotNull(t);
            Assert.AreEqual("myName2", t.Nome);
        }

        [Test]
        public void TestVerify()
        {
            var x = new SoftDeleteIndex {Name = "a"};
            x.Save();
            x.Delete();

            var y = new SoftDeleteIndex {Name = "a"};
            bool b = y.Validate().IsValid;
            Assert.IsFalse(b);
        }

        [Test]
        public void TestDeleteAll()
        {
            SoftDelete.DeleteBy(Condition.Empty);
            var o = SoftDelete.FindById(1);
            Assert.IsNull(o);

            SoftDeleteFull o1 = SoftDeleteFull.FindById(1);
            Assert.IsNotNull(o1);
            Assert.AreEqual("tom", o1.Name);
            Assert.AreEqual(true, o1.IsDeleted);
        }
    }
}
