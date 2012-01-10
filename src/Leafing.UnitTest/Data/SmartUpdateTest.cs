using System;
using Leafing.Core;
using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    #region objects

    [DbContext("SQLite")]
    public class sUser : DbObjectModel<sUser>
    {
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                m_ColumnUpdated("Name");
            }
        }

        private int _age;

        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                _age = value;
                m_ColumnUpdated("Age");
            }
        }

        public sUser()
        {
            m_InitUpdateColumns();
        }

        public sUser(string name, int age)
        {
            this.Name = name;
            this.Age = age;
            m_InitUpdateColumns();
        }
    }

    [DbContext("SQLite")]
    public class rUser : DbObjectModel<rUser>
    {
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                m_ColumnUpdated("Name");
            }
        }

        private int _age;

        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                _age = value;
                m_ColumnUpdated("Age");
            }
        }

        public HasMany<rArticle> Articles;

        public rUser()
        {
            Articles = new HasMany<rArticle>(this, null, "Reader_Id");
            m_InitUpdateColumns();
        }

        public rUser(string name, int age)
        {
            Articles = new HasMany<rArticle>(this, null, "Reader_Id");
            this.Name = name;
            this.Age = age;
            m_InitUpdateColumns();
        }
    }

    [DbContext("SQLite")]
    public class rArticle : DbObjectModel<rArticle>
    {
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                m_ColumnUpdated("Name");
            }
        }

        private int _price;

        [DbColumn("thePrice")]
        public int Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                m_ColumnUpdated("thePrice");
            }
        }

        [DbColumn("Reader_Id")]
        public BelongsTo<rUser, long> Reader;

        public rArticle()
        {
            Reader = new BelongsTo<rUser, long>(this, "Reader_Id");
            m_InitUpdateColumns();
        }

        public rArticle(string name, int age)
        {
            Reader = new BelongsTo<rUser, long>(this, "Reader_Id");
            this.Name = name;
            this.Price = age;
            m_InitUpdateColumns();
        }
    }

    [DbContext("SQLite")]
    public class asUser : DbObjectModel<asUser>
    {
        [DbColumn("theName")] public string Name { get; set; }
        public int Age { get; set; }
    }

    #endregion

    [TestFixture]
    public class SmartUpdateTest
    {
        #region init

        public SmartUpdateTest()
        {
            // raise AutoCreateTable once.
            DbEntry.From<sUser>().Where(Condition.Empty).Select();
            DbEntry.From<rUser>().Where(Condition.Empty).Select();
            DbEntry.From<rArticle>().Where(Condition.Empty).Select();
            DbEntry.From<asUser>().Where(Condition.Empty).Select();
        }

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        #endregion

        private asUser LoadAsUser(string name, int age)
        {
            StaticRecorder.CurRow.Add(new RowInfo("Id", 1L));
            StaticRecorder.CurRow.Add(new RowInfo("theName", name));
            StaticRecorder.CurRow.Add(new RowInfo("Age", age));
            var u = asUser.FindById(1);
            StaticRecorder.ClearMessages();
            return u;
        }

        [Test]
        public void TestDropManyToManyMedi()
        {
            DbEntry.DropTable(typeof(Objects.DArticleSqlite), true);
            Assert.AreEqual(2, StaticRecorder.Messages.Count);
            Assert.AreEqual("DROP TABLE [Article]<Text><30>()", StaticRecorder.Messages[0]);
            Assert.AreEqual("DROP TABLE [R_Article_Reader]<Text><30>()", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestDontUpdateIfNotSetValue()
        {
            var u = new sUser("Tom", 18) {Id = 1};
            DbEntry.Save(u);
            Assert.AreEqual(0, StaticRecorder.Messages.Count);
            Assert.AreEqual("", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestPartialUpdateThatSetValue()
        {
            var u = new sUser("Tom", 18) {Id = 1, Name = "Tom"};
            DbEntry.Save(u);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [s_User] SET [Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=Tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestPartialUpdateThatSetValueByTransaction()
        {
            DbEntry.NewTransaction(delegate
            {
                var u = new sUser("Tom", 18) {Id = 1, Name = "Tom"};
                DbEntry.Save(u);
            });
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [s_User] SET [Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=Tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestPartialUpdateThatSetedValueByTransactionWithException()
        {
            Util.CatchAll(() =>
                DbEntry.NewTransaction(delegate
                {
                    var u = new sUser("Tom", 18) {Id = 1, Name = "Tom"};
                    DbEntry.Save(u);
                    throw new Exception(); // emulate exception
                }));
            Assert.AreEqual(0, StaticRecorder.Messages.Count);
            Assert.AreEqual("", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForComplexObject()
        {
            // relationship objects, one not update, one insert, one partial update.
            var u = new rUser("tom", 18) {Id = 1};
            u.Articles.Add(new rArticle("sos", 199));
            var a = new rArticle("haha", 299) {Id = 1, Price = 180};
            u.Articles.Add(a);
            DbEntry.Save(u);
            Assert.AreEqual(2, StaticRecorder.Messages.Count);
            Assert.AreEqual("INSERT INTO [r_Article] ([Name],[thePrice],[Reader_Id]) VALUES (@Name_0,@thePrice_1,@Reader_Id_2);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@Name_0=sos:String,@thePrice_1=199:Int32,@Reader_Id_2=1:Int64)", StaticRecorder.Messages[0]);
            Assert.AreEqual("UPDATE [r_Article] SET [thePrice]=@thePrice_0,[Reader_Id]=@Reader_Id_1  WHERE [Id] = @Id_2;\n<Text><30>(@thePrice_0=180:Int32,@Reader_Id_1=1:Int64,@Id_2=1:Int64)", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject()
        {
            var u = LoadAsUser("Tom", 18);
            DbEntry.Save(u);
            Assert.AreEqual(0, StaticRecorder.Messages.Count);
            Assert.AreEqual("", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject2()
        {
            var u = LoadAsUser("jerry", 18);
            u.Name = "Tom";
            DbEntry.Save(u);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [as_User] SET [theName]=@theName_0  WHERE [Id] = @Id_1;\n<Text><30>(@theName_0=Tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject3()
        {
            var u = LoadAsUser("Tom", 18);
            u.Name = "Jerry";
            u.Age = 25;
            DbEntry.Save(u);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [as_User] SET [theName]=@theName_0,[Age]=@Age_1  WHERE [Id] = @Id_2;\n<Text><30>(@theName_0=Jerry:String,@Age_1=25:Int32,@Id_2=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject4()
        {
            var u = LoadAsUser("jerry", 18);
            DbEntry.NewTransaction(delegate
            {
                DbEntry.NewTransaction(delegate
                {
                    u.Name = "Tom";
                    DbEntry.Save(u);
                });
            });
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [as_User] SET [theName]=@theName_0  WHERE [Id] = @Id_1;\n<Text><30>(@theName_0=Tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUpdateFieldAfterSave()
        {
            var u = LoadAsUser("Tom", 18);
            u.Name = "jerry";
            DbEntry.Save(u);
            u.Age = 25;
            DbEntry.Save(u);
            Assert.AreEqual(2, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [as_User] SET [theName]=@theName_0  WHERE [Id] = @Id_1;\n<Text><30>(@theName_0=jerry:String,@Id_1=1:Int64)", StaticRecorder.Messages[0]);
            Assert.AreEqual("UPDATE [as_User] SET [Age]=@Age_0  WHERE [Id] = @Id_1;\n<Text><30>(@Age_0=25:Int32,@Id_1=1:Int64)", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestUpdateFieldAfterInsert()
        {
            var u = new asUser {Name = "Tom", Age = 18};
            //u.GetUpdateColumns().Clear();
            DbEntry.Save(u);
            u.Age = 25;
            DbEntry.Save(u);
            Assert.AreEqual(2, StaticRecorder.Messages.Count);
            Assert.AreEqual("INSERT INTO [as_User] ([theName],[Age]) VALUES (@theName_0,@Age_1);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@theName_0=Tom:String,@Age_1=18:Int32)", StaticRecorder.Messages[0]);
            string exp =
                string.Format(
                    "UPDATE [as_User] SET [Age]=@Age_0  WHERE [Id] = @Id_1;\n<Text><30>(@Age_0=25:Int32,@Id_1={0}:Int64)",
                    u.Id);
            Assert.AreEqual(exp, StaticRecorder.LastMessage);
        }
    }
}
