using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    #region objects

    public class sUser : DbObjectModel<sUser>
    {
        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                m_ColumnUpdated("Name");
            }
        }

        private int _Age;

        public int Age
        {
            get
            {
                return _Age;
            }
            set
            {
                _Age = value;
                m_ColumnUpdated("Age");
            }
        }

        public sUser()
        {
            m_InitUpdateColumns();
        }

        public sUser(string Name, int Age)
        {
            this.Name = Name;
            this.Age = Age;
            m_InitUpdateColumns();
        }
    }

    public class rUser : DbObjectModel<rUser>
    {
        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                m_ColumnUpdated("Name");
            }
        }

        private int _Age;

        public int Age
        {
            get
            {
                return _Age;
            }
            set
            {
                _Age = value;
                m_ColumnUpdated("Age");
            }
        }

        public HasMany<rArticle> Articles;

        public rUser()
        {
            Articles = new HasMany<rArticle>(this, "");
            m_InitUpdateColumns();
        }

        public rUser(string Name, int Age)
        {
            Articles = new HasMany<rArticle>(this, "");
            this.Name = Name;
            this.Age = Age;
            m_InitUpdateColumns();
        }
    }

    public class rArticle : DbObjectModel<rArticle>
    {
        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                m_ColumnUpdated("Name");
            }
        }

        private int _Price;

        [DbColumn("thePrice")]
        public int Price
        {
            get
            {
                return _Price;
            }
            set
            {
                _Price = value;
                m_ColumnUpdated("thePrice");
            }
        }

        [DbColumn("Reader_Id")]
        public BelongsTo<rUser> _Reader;

        public rArticle()
        {
            _Reader = new BelongsTo<rUser>(this);
            m_InitUpdateColumns();
        }

        public rArticle(string Name, int Age)
        {
            _Reader = new BelongsTo<rUser>(this);
            this.Name = Name;
            this.Price = Age;
            m_InitUpdateColumns();
        }
    }

    public abstract class asUser : DbObjectModel<asUser>
    {
        [DbColumn("theName")]
        public abstract string Name { get; set; }
        public abstract int Age { get; set; }

        public Dictionary<string, object> GetUpdateColumns()
        {
            return this.m_UpdateColumns;
        }

        protected asUser()
        {
        }

        protected asUser(string Name, int Age)
        {
            this.Name = Name;
            this.Age = Age;
        }
    }

    #endregion

    [TestFixture]
    public class SmartUpdateTest
    {
        #region init

        private readonly DbContext de = EntryConfig.NewContext("SQLite");

        public SmartUpdateTest()
        {
            // raise AutoCreateTable once.
            de.From<sUser>().Where(Condition.Empty).Select();
            de.From<rUser>().Where(Condition.Empty).Select();
            de.From<rArticle>().Where(Condition.Empty).Select();
            de.From<asUser>().Where(Condition.Empty).Select();
        }

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        #endregion

        [Test]
        public void TestDropManyToManyMedi()
        {
            de.DropTable(typeof(Objects.DArticle));
            Assert.AreEqual(2, StaticRecorder.Messages.Count);
            Assert.AreEqual("DROP TABLE [Article]<Text><30>()", StaticRecorder.Messages[0]);
            Assert.AreEqual("DROP TABLE [R_Article_Reader]<Text><30>()", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestDontUpdateIfNotSetValue()
        {
            var u = new sUser("Tom", 18) {Id = 1};
            de.Save(u);
            Assert.AreEqual(0, StaticRecorder.Messages.Count);
            Assert.AreEqual("", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestPartialUpdateThatSetValue()
        {
            var u = new sUser("Tom", 18) {Id = 1, Name = "Tom"};
            de.Save(u);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [s_User] SET [Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=Tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestPartialUpdateThatSetValueByTransaction()
        {
            de.NewTransaction(delegate
            {
                var u = new sUser("Tom", 18) {Id = 1, Name = "Tom"};
                de.Save(u);
            });
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [s_User] SET [Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=Tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestPartialUpdateThatSetedValueByTransactionWithException()
        {
            try
            {
                de.NewTransaction(delegate
                {
                    var u = new sUser("Tom", 18) {Id = 1, Name = "Tom"};
                    de.Save(u);
                    throw new Exception(); // emulate exception
                });
            }
            catch { }
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
            de.Save(u);
            Assert.AreEqual(2, StaticRecorder.Messages.Count);
            Assert.AreEqual("INSERT INTO [r_Article] ([Name],[thePrice],[Reader_Id]) VALUES (@Name_0,@thePrice_1,@Reader_Id_2);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@Name_0=sos:String,@thePrice_1=199:Int32,@Reader_Id_2=1:Int64)", StaticRecorder.Messages[0]);
            Assert.AreEqual("UPDATE [r_Article] SET [thePrice]=@thePrice_0,[Reader_Id]=@Reader_Id_1  WHERE [Id] = @Id_2;\n<Text><30>(@thePrice_0=180:Int32,@Reader_Id_1=1:Int64,@Id_2=1:Int64)", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject()
        {
            var u = DynamicObjectBuilder.Instance.NewObject<asUser>("Tom", 18);
            u.Id = 1; // Make it looks like read from database
            de.Save(u);
            Assert.AreEqual(0, StaticRecorder.Messages.Count);
            Assert.AreEqual("", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject2()
        {
            var u = DynamicObjectBuilder.Instance.NewObject<asUser>("jerry", 18);
            u.Id = 1; // Make it looks like read from database
            u.Name = "Tom";
            de.Save(u);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [as_User] SET [theName]=@theName_0  WHERE [Id] = @Id_1;\n<Text><30>(@theName_0=Tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject3()
        {
            var u = DynamicObjectBuilder.Instance.NewObject<asUser>("Tom", 18);
            u.Id = 1; // Make it looks like read from database
            u.Name = "Jerry";
            u.Age = 25;
            de.Save(u);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [as_User] SET [theName]=@theName_0,[Age]=@Age_1  WHERE [Id] = @Id_2;\n<Text><30>(@theName_0=Jerry:String,@Age_1=25:Int32,@Id_2=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject4()
        {
            DbEntry.NewTransaction(delegate
            {
                de.NewTransaction(delegate
                {
                    var u = DynamicObjectBuilder.Instance.NewObject<asUser>("jerry", 18);
                    u.Id = 1; // Make it looks like read from database
                    u.Name = "Tom";
                    de.Save(u);
                });
            });
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [as_User] SET [theName]=@theName_0  WHERE [Id] = @Id_1;\n<Text><30>(@theName_0=Tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUpdateFieldAfterSave()
        {
            var u = DynamicObjectBuilder.Instance.NewObject<asUser>("Tom", 18);
            u.Id = 1;
            u.Name = "jerry";
            de.Save(u);
            u.Age = 25;
            de.Save(u);
            Assert.AreEqual(2, StaticRecorder.Messages.Count);
            Assert.AreEqual("UPDATE [as_User] SET [theName]=@theName_0  WHERE [Id] = @Id_1;\n<Text><30>(@theName_0=jerry:String,@Id_1=1:Int64)", StaticRecorder.Messages[0]);
            Assert.AreEqual("UPDATE [as_User] SET [Age]=@Age_0  WHERE [Id] = @Id_1;\n<Text><30>(@Age_0=25:Int32,@Id_1=1:Int64)", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestUpdateFieldAfterInsert()
        {
            var u = DynamicObjectBuilder.Instance.NewObject<asUser>("Tom", 18);
            de.Save(u);
            u.Age = 25;
            de.Save(u);
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
