using System;
using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.UnitTest.Data.Objects;
using NUnit.Framework;
using Leafing.Data.Builder;
using Leafing.Data.Builder.Clause;
using System.Linq;

namespace Leafing.UnitTest.Data
{
    #region objects

    public class asUser2 : DbObjectModel<asUser2>
    {
        [DbColumn("theName")]
        public string Name { get; set; }
        public int Age { get; set; }

		public int GetUpdateColumnCount()
		{
			return GetUpdateColumns ().Count;
		}

		public List<KeyOpValue> GetUpdateColumns()
        {
			var builder = new UpdateStatementBuilder(new FromClause("theName"));
			this.FindUpdateColumns (builder);
			return builder.Values;
        }
    }

    public class MyTestTable : DbObject
    {
        public string Name;
        public bool Gender;
        public int Age;
        public DateTime Birthday;
    }

    public class ctUser : DbObjectModel<ctUser>
    {
        public string Name { get; set; }

        public HasOne<ctInfo> info;

        public ctUser()
        {
            info = new HasOne<ctInfo>(this, null, "user_id");
        }
    }

    public class ctInfo : DbObjectModel<ctInfo>
    {
        public string iMsg { get; set; }

        [DbColumn("user_id")]
        public BelongsTo<ctUser, long> user;

        public ctInfo()
        {
            user = new BelongsTo<ctUser, long>(this, "user_id");
        }
    }

    public class ctmUser : DbObjectModel<ctmUser>
    {
        public string Name { get; set; }

        public HasMany<ctmInfo> infos;

        public ctmUser()
        {
            infos = new HasMany<ctmInfo>(this, "Id", "user_id");
        }
    }

    public class ctmInfo : DbObjectModel<ctmInfo>
    {
        public string iMsg { get; set; }

        [DbColumn("user_id")]
        public BelongsTo<ctmUser, long> user;

        public ctmInfo()
        {
            user = new BelongsTo<ctmUser, long>(this, "user_id");
        }
        public ctmInfo(string msg) : this()
        {
            iMsg = msg;
        }
    }

    [DbTable("cmmReader")]
    public class cmmReader : DbObjectModel<cmmReader>
    {
        public string Name { get; set; }
        [DbColumn("cmmArticle_id")]
        public HasAndBelongsToMany<cmmArticle> arts;
        public cmmReader()
        {
            arts = new HasAndBelongsToMany<cmmArticle>(this, "Id", "cmmReader_id");
        }
        public cmmReader(string name) : this() { this.Name = name; }
    }

    [DbTable("cmmArticle")]
    public class cmmArticle : DbObjectModel<cmmArticle>
    {
        public string Title { get; set; }
        [DbColumn("cmmReader_id")]
        public HasAndBelongsToMany<cmmReader> rads;
        public cmmArticle()
        {
            rads = new HasAndBelongsToMany<cmmReader>(this, "Id", "cmmArticle_id");
        }
        public cmmArticle(string title) : this() { this.Title = title; }
    }

    public enum MyEnum
    {
        Worker,
        Manager,
        Costomer,
    }

    [DbTable("EnumTest")]
    public class EnumTest : DbObjectModel<EnumTest>
    {
        [Length(50)]
        public string Name { get; set; }
        public MyEnum MyType { get; set; }
        public DateTime MyDate { get; set; }
    }

    public enum UserRole
    {
        Manager,
        Worker,
        Client
    }

    [DbTable("SampleData")]
    public class SampleData : DbObjectModel<SampleData>
    {
        [Length(50)]
        public string Name { get; set; }

        public UserRole Role { get; set; }

        public DateTime JoinDate { get; set; }

        public bool Enabled { get; set; }

        public int? NullInt { get; set; }
    }

    [JoinOn(0, typeof(People), "Id", typeof(PCs), "Id")]
    public class JoinTableNoCreate : IDbObject
    {
        [DbColumn("People.Id")]
        public long Id;

        [DbColumn("People.Name")]
        public string Name;

        [DbColumn("PCs.Name")]
        public string PcName;
    }

    [DbTable(typeof(DateAndTimeSqlite)), DbContext("SQLite")]
    public class DtPart : DbObjectModel<DtPart>
    {
        public DateTime dtValue { get; set; }
    }

	[DbTable("AutoAlterTableTest"), DbContext("Scheme")]
    public class AutoAlterTableTest : DbObjectModel<AutoAlterTableTest>
    {
        [Length(50)]
        public string Name { get; set; }

        public int Age { get; set; }

        public bool Gender { get; set; }

        public double Salary { get; set; }
    }

    #endregion

    [TestFixture]
    public class AutoCreateOrAlterTableTest : DataTestBase
    {
        [Test]
        public void TestGetTableNames()
        {
            var tables = new List<string> { "Article", "ArticleMore", "AutoAlterTableTest", "Bao_Xiu_RS", "BelongsMore", "Books", "Categories", "Lock_Book", "LockVersionTest",
                "Co_User", "Co_User1", "DateAndTime", "File", "NullTest", "PCs", "People", "R_Article_Reader", "Reader", "ReaderMore", "Required_Model", "Required_Two", "SoftDelete",
				"DCS_USERS", "REF_ORG_UNIT", "HRM_EMPLOYEES", "DCS_PERSONS", "REL_EMP_JOB_ROLE", "REL_JOB_ROLE_ORG_UNIT", "HRM_JOB_ROLES", "sqlite_sequence" };
            tables.Sort();
            string[] ts = tables.ToArray();

            List<string> li = DbEntry.Provider.GetTableNames();
            li.Sort();
            Assert.AreEqual(ts, li.ToArray());
        }

        [Test]
        public void TestAutoCreateTable()
        {
			var dt = DateTime.Now;
            var o = new MyTestTable {Name = "Tom", Gender = true, Age = 18, Birthday = dt};
            DbEntry.Save(o);
            List<MyTestTable> ls = DbEntry.From<MyTestTable>().Where(Condition.Empty).Select();
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("Tom", ls[0].Name);
            Assert.AreEqual(true, ls[0].Gender);
            Assert.AreEqual(18, ls[0].Age);
			Console.WriteLine (dt - dt.Date);
			Console.WriteLine (ls[0].Birthday - dt.Date);
			Console.WriteLine (dt - ls[0].Birthday);
			Assert.IsTrue((dt - ls[0].Birthday).TotalMilliseconds < 1);
        }

        [Test]
        public void TestHasOne()
        {
            var u = new ctUser {Name = "Tom", info = {Value = new ctInfo {iMsg = "ok"}}};
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
            var u = new asUser2 { Name = "Tom", Age = 18 };
            u.Save();
            var u1 = asUser2.FindById(u.Id);
			Assert.AreEqual(0, u1.GetUpdateColumnCount());
            u1.Name = "Jerry";
			Assert.AreEqual(1, u1.GetUpdateColumnCount());
			Assert.IsTrue(u1.GetUpdateColumns()[0].Key == "theName");
            u1.Save();
            var u2 = asUser2.FindById(u.Id);
            Assert.AreEqual("Jerry", u2.Name);
        }

        [Test]
        public void TestEnum()
        {
            var u = new EnumTest {Name = "test", MyType = MyEnum.Manager, MyDate = new DateTime(2000, 1, 1)};
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
            new SampleData{Name = "angel", Role = UserRole.Worker, JoinDate = new DateTime(2004, 2, 27, 15, 10, 23),Enabled = true,NullInt = null}.Save();
            new SampleData{Name = "tom", Role = UserRole.Manager, JoinDate = new DateTime(2001, 3, 17, 7, 12, 4), Enabled = false, NullInt = null}.Save();
            new SampleData{Name = "jerry", Role = UserRole.Client, JoinDate = new DateTime(1999, 1, 31, 21, 22, 55), Enabled = true, NullInt = 10}.Save();
            List<SampleData> ls1 = SampleData.Find(CK.K["Id"] > 1, new OrderBy("Id"));
            Assert.AreEqual(2, ls1.Count);
        }

        [Test]
        public void TestJoinTableNoCreate()
        {
            DbEntry.From<JoinTableNoCreate>().Where(Condition.Empty).Select();
        }

        [Test]
        public void TestPartOf()
        {
            DbEntry.From<DtPart>().Where(Condition.Empty).Select();
            AssertSql(@"SELECT [Id],[dtValue] FROM [DateAndTime];
<Text><60>()");
        }

        [Test]
        public void TestAutoAlterTableTest()
        {
            var o = new AutoAlterTableTest { Name = "tom", Age = 19, Gender = true, Salary = 2653.23 };
            o.Save();
            var o1 = AutoAlterTableTest.FindById(o.Id);
            Assert.IsNotNull(o1);
            Assert.AreEqual("tom", o1.Name);
            Assert.AreEqual(19, o1.Age);
            Assert.AreEqual(true, o1.Gender);
            Assert.AreEqual(2653.23, o1.Salary);
        }
    }
}
