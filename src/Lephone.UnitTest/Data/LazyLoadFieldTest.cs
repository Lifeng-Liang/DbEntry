using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using Lephone.Util;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    public class lzUser1 : DbObject
    {
        public string Name;
        public LazyLoadField<string> Profile;

        public lzUser1()
        {
            Profile = new LazyLoadField<string>(this);
        }
    }

    public abstract class lzUser : DbObjectModel<lzUser>
    {
        public abstract string Name { get; set; }

        [DbColumn("Profile")]
        protected internal LazyLoadField<string> _Profile;

        [LazyLoad]
        public string Profile
        {
            get
            {
                return _Profile.Value;
            }
            set
            {
                _Profile.Value = value;
                m_ColumnUpdated("Profile");
            }
        }

        protected lzUser()
        {
            _Profile = new LazyLoadField<string>(this);
        }
    }

    public abstract class lzpUser : DbObjectModel<lzpUser>
    {
        public abstract string Name { get; set; }

        [LazyLoad]
        public abstract string Profile { get; set; }
    }

    [DbTable("User")]
    public abstract class lzpUser1 : DbObjectModel<lzpUser1>
    {
        public abstract string Name { get; set; }

        [LazyLoad, AllowNull, DbColumn("MyTest"), Length(10)]
        [StringColumn(IsUnicode=false, Regular=CommonRegular.EmailRegular)]
        [Index(ASC=true, IndexName="test", UNIQUE=true)]
        public abstract string Profile { get; set; }
    }

    [TestFixture]
    public class LazyLoadFieldTest : DataTestBase
    {
        #region Init

        protected override void OnSetUp()
        {
            base.OnSetUp();
            ClassHelper.SetValue(DbEntry.Context, "_tableNames", null);
        }

        #endregion

        [Test]
        public void TestCreate()
        {
            sqlite.Create(typeof(lzUser));
            Assert.AreEqual("CREATE TABLE [lz_User] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[Profile] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate1()
        {
            sqlite.Create(typeof(lzpUser1));
            Assert.AreEqual("CREATE TABLE [User] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[MyTest] VARCHAR (10) NULL \n);\nCREATE UNIQUE INDEX [IX_User_test] ON [User] ([MyTest] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestValidate()
        {
            lzpUser1 u = lzpUser1.New();
            u.Name = "tom";
            u.Profile = "xxx";
            Assert.IsFalse(u.IsValid());

            u.Profile = "a@b.c";
            Assert.IsTrue(u.IsValid());
        }

        [Test]
        public void TestRead()
        {
            sqlite.GetObject<lzUser>(1);
            Assert.AreEqual("SELECT [Id],[Name] FROM [lz_User] WHERE [Id] = @Id_0;\n<Text><60>(@Id_0=1:Int32)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestInsert()
        {
            lzUser u = lzUser.New();
            u.Name = "tom";
            u.Profile = "test";
            sqlite.Insert(u);
            Assert.AreEqual("INSERT INTO [lz_User] ([Name],[Profile]) VALUES (@Name_0,@Profile_1);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@Name_0=tom:String,@Profile_1=test:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCRUD()
        {
            lzUser u = lzUser.New();
            u.Name = "tom";
            u.Profile = "test";
            u.Save();
            Assert.AreEqual(1, u.Id);

            lzUser u1 = lzUser.FindById(1);
            Assert.AreEqual("tom", u1.Name);
            Assert.AreEqual("test", u1.Profile);

            u1.Profile = "test 2";
            u1.Save();

            lzUser u2 = lzUser.FindById(1);
            Assert.AreEqual("tom", u2.Name);
            Assert.AreEqual("test 2", u2.Profile);

            u2.Delete();

            lzUser u3 = lzUser.FindById(1);
            Assert.IsNull(u3);
        }

        [Test]
        public void TestCRUDforDynamicObject()
        {
            lzpUser u = lzpUser.New();
            u.Name = "tom";
            u.Profile = "test";
            u.Save();
            Assert.AreEqual(1, u.Id);

            lzpUser u1 = lzpUser.FindById(1);
            Assert.AreEqual("tom", u1.Name);
            Assert.AreEqual("test", u1.Profile);

            u1.Profile = "test 2";
            u1.Save();

            lzpUser u2 = lzpUser.FindById(1);
            Assert.AreEqual("tom", u2.Name);
            Assert.AreEqual("test 2", u2.Profile);

            u2.Delete();

            lzpUser u3 = lzpUser.FindById(1);
            Assert.IsNull(u3);
        }

        [Test]
        public void TestForNotUpdateWithDynamicObject()
        {
            lzpUser u = lzpUser.New();
            u.Name = "tom";
            u.Profile = "test";
            u.Save();
            Assert.AreEqual(1, u.Id);

            lzpUser u1 = lzpUser.FindById(1);
            u1.Name = "jerry";
            u1.Save();

            lzpUser u2 = lzpUser.FindById(1);
            Assert.AreEqual("jerry", u2.Name);
            Assert.AreEqual("test", u2.Profile);
        }

        [Test]
        public void TestForNotUpdate()
        {
            var u = new lzUser1();
            u.Name = "tom";
            u.Profile.Value = "test";
            DbEntry.Save(u);
            Assert.AreEqual(1, u.Id);

            var u1 = DbEntry.GetObject<lzUser1>(1);
            DbEntry.Save(u1);

            var u2 = DbEntry.GetObject<lzUser1>(1);
            Assert.AreEqual("tom", u2.Name);
            Assert.AreEqual("test", u2.Profile.Value);
        }
    }
}
