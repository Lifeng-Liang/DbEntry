using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
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

        public ctUser()
        {
            info = new HasOne<ctInfo>(this, "");
        }
    }

    public class ctInfo : DbObject
    {
        public string iMsg;
        [DbColumn("user_id")]
        public BelongsTo<ctUser> user;

        public ctInfo()
        {
            user = new BelongsTo<ctUser>(this);
        }
    }

    public class ctmUser : DbObject
    {
        public string Name;
        public HasMany<ctmInfo> infos;

        public ctmUser()
        {
            infos = new HasMany<ctmInfo>(this, new OrderBy("Id"));
        }
    }

    public class ctmInfo : DbObject
    {
        public string iMsg;
        [DbColumn("user_id")]
        public BelongsTo<ctmUser> user;

        public ctmInfo()
        {
            user = new BelongsTo<ctmUser>(this);
        }
        public ctmInfo(string msg) : this()
        {
            iMsg = msg;
        }
    }

    [DbTable("cmmReader")]
    public class cmmReader : DbObject
    {
        public string Name;
        [DbColumn("cmmArticle_id")]
        public HasAndBelongsToMany<cmmArticle> arts;
        public cmmReader()
        {
            arts = new HasAndBelongsToMany<cmmArticle>(this, new OrderBy("Id"));
        }
        public cmmReader(string Name) : this() { this.Name = Name; }
    }

    [DbTable("cmmArticle")]
    public class cmmArticle : DbObject
    {
        public string Title;
        [DbColumn("cmmReader_id")]
        public HasAndBelongsToMany<cmmReader> rads;
        public cmmArticle()
        {
            rads = new HasAndBelongsToMany<cmmReader>(this, new OrderBy("Id"));
        }
        public cmmArticle(string Title) : this() { this.Title = Title; }
    }

    public enum MyEnum
    {
        Worker,
        Manager,
        Costomer,
    }

    [DbTable("EnumTest")]
    public abstract class EnumTest : DbObjectModel<EnumTest>
    {
        [Length(50)]
        public abstract string Name { get; set; }
        public abstract MyEnum MyType { get; set; }
        public abstract DateTime MyDate { get; set; }
    }

    public enum UserRole
    {
        Manager,
        Worker,
        Client
    }

    [DbTable("SampleData")]
    public abstract class SampleData : DbObjectModel<SampleData>
    {
        [Length(50)]
        public abstract string Name { get; set; }

        public abstract UserRole Role { get; set; }

        public abstract DateTime JoinDate { get; set; }

        public abstract bool Enabled { get; set; }

        public abstract int? NullInt { get; set; }

        public SampleData Init(string name, UserRole role, DateTime joinDate, bool enabled, int? nullInt)
        {
            Name = name;
            Role = role;
            JoinDate = joinDate;
            Enabled = enabled;
            NullInt = nullInt;
            return this;
        }
    }


    #endregion

    [TestFixture]
    public class AutoCreateTableTest : DataTestBase
    {
        [Test]
        public void TestGetTableNames()
        {
            var tables = new List<string> { "Article", "BelongsMore", "Books", "Categories", "LockVersionTest",
                "DateAndTime", "File", "NullTest", "PCs", "People", "R_Article_Reader", "Reader", "SoftDelete",
                "DCS_USERS", "REF_ORG_UNIT", "HRM_EMPLOYEES", "DCS_PERSONS", "REL_EMP_JOB_ROLE", "REL_JOB_ROLE_ORG_UNIT", "HRM_JOB_ROLES" };
            tables.Sort();
            string[] ts = tables.ToArray();

            List<string> li = DbEntry.Context.GetTableNames();
            li.Sort();
            Assert.AreEqual(ts, li.ToArray());
        }

        [Test]
        public void TestAutoCreateTable()
        {
            var o = new MyTestTable {Name = "Tom", Gender = true, Age = 18, Birthday = DateTime.Now};
            DbEntry.Save(o);
            List<MyTestTable> ls = DbEntry.From<MyTestTable>().Where(WhereCondition.EmptyCondition).Select();
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("Tom", ls[0].Name);
            Assert.AreEqual(true, ls[0].Gender);
            Assert.AreEqual(18, ls[0].Age);
            Assert.AreEqual(o.Birthday, ls[0].Birthday);
        }

        [Test]
        public void TestHasOne()
        {
            var u = new ctUser { Name = "Tom" };
            u.info.Value = new ctInfo {iMsg = "ok"};
            DbEntry.Save(u);
            var o = DbEntry.GetObject<ctUser>(u.Id);
            Assert.AreEqual("Tom", o.Name);
            Assert.AreEqual("ok", o.info.Value.iMsg);
        }

        [Test]
        public void TestHasMany()
        {
            var u = new ctmUser {Name = "Jerry"};
            u.infos.Add(new ctmInfo("aha"));
            u.infos.Add(new ctmInfo("let me c"));
            DbEntry.Save(u);
            var o = DbEntry.GetObject<ctmUser>(u.Id);
            Assert.AreEqual("Jerry", o.Name);
            Assert.AreEqual(2, o.infos.Count);
            Assert.AreEqual("aha", o.infos[0].iMsg);
            Assert.AreEqual("let me c", o.infos[1].iMsg);
        }

        [Test]
        public void TestHasAndBelongsToMany()
        {
            var u = new cmmReader("Tom");
            u.arts.Add(new cmmArticle("do"));
            u.arts.Add(new cmmArticle("ok"));
            u.arts.Add(new cmmArticle("go"));
            DbEntry.Save(u);
            var a = DbEntry.GetObject<cmmArticle>(u.arts[2].Id);
            a.rads.Add(new cmmReader("Jerry"));
            a.rads[0].arts.Add(new cmmArticle("pp"));
            DbEntry.Save(a);
            var o1 = DbEntry.GetObject<cmmReader>(u.Id);
            Assert.AreEqual("Tom", o1.Name);
            Assert.AreEqual(3, o1.arts.Count);
            Assert.AreEqual("do", o1.arts[0].Title);
            Assert.AreEqual("ok", o1.arts[1].Title);
            Assert.AreEqual("go", o1.arts[2].Title);
            var o2 = DbEntry.GetObject<cmmReader>(a.rads[0].Id);
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

        [Test]
        public void TestEnum()
        {
            EnumTest u = EnumTest.New();
            u.Name = "test";
            u.MyType = MyEnum.Manager;
            u.MyDate = new DateTime(2000, 1, 1);
            u.Save();

            EnumTest u1 = EnumTest.FindById(u.Id);
            Assert.AreEqual("test", u1.Name);
            Assert.AreEqual(MyEnum.Manager, u1.MyType);
            Assert.AreEqual(new DateTime(2000, 1, 1), u.MyDate);

            List<EnumTest> ls = EnumTest.Find(CK.K["Id"] > 0, new OrderBy("Id"));
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("test", ls[0].Name);
        }

        [Test]
        public void TestSampleData()
        {
            SampleData.New().Init("angel", UserRole.Worker, new DateTime(2004, 2, 27, 15, 10, 23), true, null).Save();
            SampleData.New().Init("tom", UserRole.Manager, new DateTime(2001, 3, 17, 7, 12, 4), false, null).Save();
            SampleData.New().Init("jerry", UserRole.Client, new DateTime(1999, 1, 31, 21, 22, 55), true, 10).Save();
            List<SampleData> ls1 = SampleData.Find(CK.K["Id"] > 1, new OrderBy("Id"));
            Assert.AreEqual(2, ls1.Count);
        }
    }
}
