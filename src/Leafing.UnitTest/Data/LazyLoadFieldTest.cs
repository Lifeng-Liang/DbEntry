using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    public class lzUser1 : DbObjectModel<lzUser1>
    {
        public string Name { get; set; }
        public LazyLoad<string> Profile;

        public lzUser1()
        {
            Profile = new LazyLoad<string>(this, "Profile");
        }
    }

    public class lzUser : DbObjectModel<lzUser>
    {
        public string Name { get; set; }

        [DbColumn("Profile")]
		public LazyLoad<string> Profile {get;set;}

        public lzUser()
        {
            Profile = new LazyLoad<string>(this, "Profile");
        }
    }

    [DbContext("SQLite")]
    public class lzUser2 : DbObjectModel<lzUser2>
    {
        public string Name { get; set; }

		public LazyLoad<string> Profile { get; set; }

		public lzUser2 ()
		{
			Profile = new LazyLoad<string> (this, "Profile");
		}
    }

    public class lzpUser : DbObjectModel<lzpUser>
    {
        public string Name { get; set; }

		public LazyLoad<string> Profile { get; set; }

		public lzpUser ()
		{
			Profile = new LazyLoad<string> (this, "Profile");
		}
    }

    [DbContext("SQLite")]
    public class lzpUserSqlite : DbObjectModel<lzpUserSqlite>
    {
        public string Name { get; set; }

		public LazyLoad<string> Profile { get; set; }

		public lzpUserSqlite ()
		{
			Profile = new LazyLoad<string> (this, "Profile");
		}
    }

    [DbContext("SQLite")]
    public class lzpUserSqlite2 : DbObjectModel<lzpUserSqlite2>
    {
        public string Name { get; set; }

        [AllowNull]
		public LazyLoad<string> Profile { get; set; }

		public lzpUserSqlite2 ()
		{
			Profile = new LazyLoad<string> (this, "Profile");
		}
    }

    [DbContext("SQLite")]
    public class lzpUserSqlite3 : DbObjectModel<lzpUserSqlite3>
    {
        public string Name { get; set; }

        [AllowNull]
		public LazyLoad<int?> Age { get; set; }

		public lzpUserSqlite3 ()
		{
			Age = new LazyLoad<int?> (this, "Age");
		}
    }

    public class lzpUserSqlite4 : DbObjectModel<lzpUserSqlite4>
    {
        public string Name { get; set; }

        [AllowNull]
		public LazyLoad<int?> Age { get; set; }

		public lzpUserSqlite4 ()
		{
			Age = new LazyLoad<int?> (this, "Age");
		}
    }

    [DbTable("User"), DbContext("SQLite")]
    public class lzpUser1 : DbObjectModel<lzpUser1>
    {
        public string Name { get; set; }

        [AllowNull, DbColumn("MyTest"), Length(10)]
        [StringColumn(IsUnicode=false, Regular=CommonRegular.EmailRegular)]
        [Index(ASC=true, IndexName="test", UNIQUE=true)]
		public LazyLoad<string> Profile { get; set; }

		public lzpUser1 ()
		{
			Profile = new LazyLoad<string> (this, "Profile");
		}
    }

    [TestFixture]
    public class LazyLoadFieldTest : DataTestBase
    {
        #region Init

        protected override void OnSetUp()
        {
            base.OnSetUp();
            //DbEntry.Provider.Driver.TableNames = null;
        }

        #endregion

        [Test]
        public void TestCreate()
        {
            DbEntry.Create(typeof(lzUser2));
            Assert.AreEqual("CREATE TABLE [lz_User2] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[Profile] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate1()
        {
            DbEntry.Create(typeof(lzpUser1));
            Assert.AreEqual("CREATE TABLE [User] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[MyTest] VARCHAR (10) NULL \n);\nCREATE UNIQUE INDEX [IX_User_test] ON [User] ([MyTest] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestValidate()
        {
            StaticRecorder.CurRow.Add(new RowInfo(0));
            var u = new lzpUser1 {Name = "tom"};
			u.Profile.Value = "xxx";
            Assert.IsFalse(u.IsValid());

            StaticRecorder.CurRow.Add(new RowInfo(0));
			u.Profile.Value = "a@b.c";
            Assert.IsTrue(u.IsValid());
        }

        [Test]
        public void TestRead()
        {
            DbEntry.GetObject<lzUser2>(1);
            Assert.AreEqual("SELECT [Id],[Name] FROM [lz_User2] WHERE [Id] = @Id_0;\n<Text><60>(@Id_0=1:Int32)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestInsert()
        {
            var u = new lzUser2 {Name = "tom"};
			u.Profile.Value = "test";
            DbEntry.Insert(u);
            Assert.AreEqual("INSERT INTO [lz_User2] ([Name],[Profile]) VALUES (@Name_0,@Profile_1);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@Name_0=tom:String,@Profile_1=test:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCrud()
        {
            var u = new lzUser {Name = "tom"};
			u.Profile.Value = "test";
            u.Save();
            Assert.AreEqual(1, u.Id);

            lzUser u1 = lzUser.FindById(1);
            Assert.AreEqual("tom", u1.Name);
			Assert.AreEqual("test", u1.Profile.Value);

			u1.Profile.Value = "test 2";
            u1.Save();

            lzUser u2 = lzUser.FindById(1);
            Assert.AreEqual("tom", u2.Name);
			Assert.AreEqual("test 2", u2.Profile.Value);

            u2.Delete();

            lzUser u3 = lzUser.FindById(1);
            Assert.IsNull(u3);
        }

        [Test]
        public void TestCrudForDynamicObject()
        {
            DbEntry.Create(typeof(lzpUser));

            var u = new lzpUser { Name = "tom" };
			u.Profile.Value = "test";
            u.Save();
            Assert.AreEqual(1, u.Id);

            lzpUser u1 = lzpUser.FindById(1);
            Assert.AreEqual("tom", u1.Name);
            Assert.AreEqual("test", u1.Profile.Value);

			u1.Profile.Value = "test 2";
            u1.Save();

            lzpUser u2 = lzpUser.FindById(1);
            Assert.AreEqual("tom", u2.Name);
            Assert.AreEqual("test 2", u2.Profile.Value);

            u2.Delete();

            lzpUser u3 = lzpUser.FindById(1);
            Assert.IsNull(u3);
        }

        [Test]
        public void TestForNotUpdateWithDynamicObject()
        {
            DbEntry.Create(typeof(lzpUser));

            var u = new lzpUser {Name = "tom"};
			u.Profile.Value = "test";
            u.Save();
            Assert.AreEqual(1, u.Id);

            lzpUser u1 = lzpUser.FindById(1);
            u1.Name = "jerry";
            u1.Save();

            lzpUser u2 = lzpUser.FindById(1);
            Assert.AreEqual("jerry", u2.Name);
			Assert.AreEqual("test", u2.Profile.Value);
        }

        [Test]
        public void TestForNotUpdate()
        {
            var u = new lzUser1 {Name = "tom", Profile = {Value = "test"}};
            DbEntry.Save(u);
            Assert.AreEqual(1, u.Id);

            var u1 = DbEntry.GetObject<lzUser1>(1);
            DbEntry.Save(u1);

            var u2 = DbEntry.GetObject<lzUser1>(1);
            Assert.AreEqual("tom", u2.Name);
            Assert.AreEqual("test", u2.Profile.Value);
        }

        //[Test]
        //public void TestLazyLoadFieldForCondition()
        //{
        //    Condition c = CK<lzpUser>.Field["Profile"] == "test";
        //    var dpc = new DataParameterCollection();
        //    Assert.AreEqual("[Profile] = @Profile_0", c.ToSqlText(dpc, DbEntry.Provider.Dialect));
        //    Assert.AreEqual(1, dpc.Count);
        //    Assert.AreEqual("test", dpc[0].Value);
        //}

        [Test]
        public void TestLazyLoadFieldForCondition2()
        {
			DbEntry.From<lzpUserSqlite>().Where(p => p.Profile.Value == "test").Select();
            AssertSql("SELECT [Id],[Name] FROM [lzp_User_Sqlite] WHERE [Profile] = @Profile_0;\n<Text><60>(@Profile_0=test:String)");
        }

        [Test]
        public void TestLazyloadNullable()
        {
            lzpUserSqlite2.Where(p => p.Profile == null).Select();
            AssertSql("SELECT [Id],[Name] FROM [lzp_User_Sqlite2] WHERE [Profile] IS NULL;\n<Text><60>()");
        }

        [Test]
        public void TestLazyloadNullable2()
        {
            lzpUserSqlite3.Where(p => p.Age == null).Select();
            AssertSql("SELECT [Id],[Name] FROM [lzp_User_Sqlite3] WHERE [Age] IS NULL;\n<Text><60>()");
        }

        [Test]
        public void TestLazyloadNullable3()
        {
            var u = new lzpUserSqlite4 {Name = "tom"};
			u.Age.Value = 12;
            u.Save();
            var u1 = lzpUserSqlite4.FindById(u.Id);
            Assert.AreEqual("tom", u1.Name);
			Assert.AreEqual(12, u1.Age.Value);
			u1.Age.Value = null;
            u1.Save();
            var u2 = lzpUserSqlite4.FindById(u.Id);
            Assert.AreEqual("tom", u2.Name);
			Assert.AreEqual(null, u2.Age.Value);
            var u3 = lzpUserSqlite4.FindById(u.Id);
			u3.Age.Value = 19;
            u3.Save();
            var u4 = lzpUserSqlite4.FindById(u.Id);
			Assert.AreEqual(19, u4.Age.Value);
        }
    }
}
