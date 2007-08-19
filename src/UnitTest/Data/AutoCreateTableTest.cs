
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Definition;
using NUnit.Framework;

#endregion

namespace org.hanzify.llf.UnitTest.Data
{
    #region objects

    public class MyTestTable : DbObject
    {
        public string Name;
        public bool Gender;
        public int Age;
        public DateTime Birthday;
    }

    public class ctUser : DbObject
    {
        public string Name;
        public HasOne<ctInfo> info;
    }

    public class ctInfo : DbObject
    {
        public string iMsg;
        [DbColumn("user_id")]
        public BelongsTo<ctUser> user;
    }

    public class ctmUser : DbObject
    {
        public string Name;
        public HasMany<ctmInfo> infos = new HasMany<ctmInfo>(new OrderBy("Id"));
    }

    public class ctmInfo : DbObject
    {
        public string iMsg;
        [DbColumn("user_id")]
        public BelongsTo<ctmUser> user;

        public ctmInfo() { }
        public ctmInfo(string msg)
        {
            iMsg = msg;
        }
    }

    public class cmmReader : DbObject
    {
        public string Name;
        [DbColumn("cmmArticle_id")]
        public HasAndBelongsToMany<cmmArticle> arts = new HasAndBelongsToMany<cmmArticle>(new OrderBy("Id"));
        public cmmReader() { }
        public cmmReader(string Name) { this.Name = Name; }
    }

    public class cmmArticle : DbObject
    {
        public string Title;
        [DbColumn("cmmReader_id")]
        public HasAndBelongsToMany<cmmReader> rads = new HasAndBelongsToMany<cmmReader>(new OrderBy("Id"));
        public cmmArticle() { }
        public cmmArticle(string Title) { this.Title = Title; }
    }

    #endregion

    [TestFixture]
    public class AutoCreateTableTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

        [Test]
        public void TestGetTableNames()
        {
            List<string> li = DbEntry.Context.GetTableNames();
            li.Sort();
            Assert.AreEqual(8, li.Count);
            Assert.AreEqual("Article", li[0]);
            Assert.AreEqual("Article_Reader", li[1]);
            Assert.AreEqual("Books", li[2]);
            Assert.AreEqual("Categories", li[3]);
            Assert.AreEqual("File", li[4]);
            Assert.AreEqual("PCs", li[5]);
            Assert.AreEqual("People", li[6]);
            Assert.AreEqual("Reader", li[7]);
        }

        [Test]
        public void TestAutoCreateTable()
        {
            MyTestTable o = new MyTestTable();
            o.Name = "Tom";
            o.Gender = true;
            o.Age = 18;
            o.Birthday = DateTime.Now;
            DbEntry.Save(o);
            List<MyTestTable> ls = DbEntry.From<MyTestTable>().Where(null).Select();
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("Tom", ls[0].Name);
            Assert.AreEqual(true, ls[0].Gender);
            Assert.AreEqual(18, ls[0].Age);
            Assert.AreEqual(o.Birthday, ls[0].Birthday);
        }

        [Test]
        public void TestHasOne()
        {
            ctUser u = new ctUser();
            u.Name = "Tom";
            u.info.Value = new ctInfo();
            u.info.Value.iMsg = "ok";
            DbEntry.Save(u);
            ctUser o = DbEntry.GetObject<ctUser>(u.Id);
            Assert.AreEqual("Tom", o.Name);
            Assert.AreEqual("ok", o.info.Value.iMsg);
        }

        [Test]
        public void TestHasMany()
        {
            ctmUser u = new ctmUser();
            u.Name = "Jerry";
            u.infos.Add(new ctmInfo("aha"));
            u.infos.Add(new ctmInfo("let me c"));
            DbEntry.Save(u);
            ctmUser o = DbEntry.GetObject<ctmUser>(u.Id);
            Assert.AreEqual("Jerry", o.Name);
            Assert.AreEqual(2, o.infos.Count);
            Assert.AreEqual("aha", o.infos[0].iMsg);
            Assert.AreEqual("let me c", o.infos[1].iMsg);
        }

        [Test]
        public void TestHasAndBelongsToMany()
        {
            cmmReader u = new cmmReader("Tom");
            u.arts.Add(new cmmArticle("do"));
            u.arts.Add(new cmmArticle("ok"));
            u.arts.Add(new cmmArticle("go"));
            DbEntry.Save(u);
            cmmArticle a = DbEntry.GetObject<cmmArticle>(u.arts[2].Id);
            a.rads.Add(new cmmReader("Jerry"));
            a.rads[0].arts.Add(new cmmArticle("pp"));
            DbEntry.Save(a);
            cmmReader o1 = DbEntry.GetObject<cmmReader>(u.Id);
            Assert.AreEqual("Tom", o1.Name);
            Assert.AreEqual(3, o1.arts.Count);
            Assert.AreEqual("do", o1.arts[0].Title);
            Assert.AreEqual("ok", o1.arts[1].Title);
            Assert.AreEqual("go", o1.arts[2].Title);
            cmmReader o2 = DbEntry.GetObject<cmmReader>(a.rads[0].Id);
            Assert.AreEqual("Jerry", o2.Name);
            Assert.AreEqual(2, o2.arts.Count);
            Assert.AreEqual("go", o2.arts[0].Title);
            Assert.AreEqual("pp", o2.arts[1].Title);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject5()
        {
            // read from database, the updateColumns is empty
            asUser u = asUser.New("Tom", 18);
            u.Save();
            asUser u1 = asUser.FindById(u.Id);
            Assert.AreEqual(0, u1.GetUpdateColumns().Count);
            u1.Name = "Jerry";
            Assert.AreEqual(1, u1.GetUpdateColumns().Count);
            Assert.IsTrue(u1.GetUpdateColumns().ContainsKey("theName"));
            u1.Save();
            asUser u2 = asUser.FindById(u.Id);
            Assert.AreEqual("Jerry", u2.Name);
        }
    }
}
