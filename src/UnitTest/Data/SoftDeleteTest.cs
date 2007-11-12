
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;

namespace Lephone.UnitTest.Data
{
    #region objects

    [SoftDelete, DbTable("SoftDelete")]
    public abstract class SoftDelete : DbObjectModel<SoftDelete>
    {
        public abstract string Name { get; set; }

        public SoftDelete Init(string Name)
        {
            this.Name = Name;
            return this;
        }
    }

    [DbTable("SoftDelete")]
    public abstract class SoftDeleteFull : DbObjectModel<SoftDeleteFull>
    {
        public abstract string Name { get; set; }
        public abstract bool IsDeleted { get; set; }
    }

    #endregion

    [TestFixture]
    public class SoftDeleteTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
            StaticRecorder.ClearMessages();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

        [Test]
        public void TestInsert()
        {
            SoftDelete o = SoftDelete.New().Init("test");
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
            DbContext de = new DbContext("SQLite");
            de.From<SoftDelete>().Where(WhereCondition.EmptyCondition).GroupBy<string>("tom");
            Assert.AreEqual("CREATE TABLE [SoftDelete] (\n	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n	[Name] ntext NOT NULL ,\n	[IsDeleted] bool NOT NULL \n);\n", StaticRecorder.Messages[0]);
            Assert.AreEqual("Select [tom],Count([tom]) As it__count__ From [SoftDelete] Where [IsDeleted] = @IsDeleted_0 Group By [tom];\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreateTable()
        {
            DbContext de = new DbContext("SQLite");
            de.Create(typeof(SoftDelete));
            Assert.AreEqual("CREATE TABLE [SoftDelete] (\n	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n	[Name] ntext NOT NULL ,\n	[IsDeleted] bool NOT NULL \n);\n", StaticRecorder.LastMessage);
        }
    }
}
