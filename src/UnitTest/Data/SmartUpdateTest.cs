
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.Common;
using Lephone.MockSql;
using Lephone.MockSql.Recorder;

#endregion

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

        public asUser()
        {
        }

        public asUser(string Name, int Age)
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

        private DbContext de = new DbContext(EntryConfig.GetDriver("SQLite"));

        public SmartUpdateTest()
        {
            // raise AutoCreateTable once.
            de.From<sUser>().Where(null).Select();
            de.From<rUser>().Where(null).Select();
            de.From<rArticle>().Where(null).Select();
            de.From<asUser>().Where(null).Select();
        }

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

        [Test]
        public void TestDropManyToManyMedi()
        {
            de.DropTable(typeof(Lephone.UnitTest.Data.Objects.DArticle));
            Assert.AreEqual(2, StaticRecorder.Messages.Count);
            Assert.AreEqual("Drop Table [Article]", StaticRecorder.Messages[0]);
            Assert.AreEqual("Drop Table [R_Article_Reader]", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestDontUpdateIfNotSetValue()
        {
            sUser u = new sUser("Tom", 18);
            u.Id = 1; // Make it looks like read from database
            de.Save(u);
            Assert.AreEqual(0, StaticRecorder.Messages.Count);
            Assert.AreEqual("", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestPartialUpdateThatSetValue()
        {
            sUser u = new sUser("Tom", 18);
            u.Id = 1; // Make it looks like read from database
            u.Name = "Tom";
            de.Save(u);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("Update [s_User] Set [Name]=@Name_0  Where [Id] = @Id_1;\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestPartialUpdateThatSetValueByTransaction()
        {
            de.UsingTransaction(delegate()
            {
                sUser u = new sUser("Tom", 18);
                u.Id = 1; // Make it looks like read from database
                u.Name = "Tom";
                de.Save(u);
            });
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("Update [s_User] Set [Name]=@Name_0  Where [Id] = @Id_1;\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestPartialUpdateThatSetedValueByTransactionWithException()
        {
            try
            {
                de.UsingTransaction(delegate()
                {
                    sUser u = new sUser("Tom", 18);
                    u.Id = 1; // Make it looks like read from database
                    u.Name = "Tom";
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
            rUser u = new rUser("tom", 18);
            u.Id = 1; // Make it looks like read from database
            u.Articles.Add(new rArticle("sos", 199));
            rArticle a = new rArticle("haha", 299);
            a.Id = 1;
            a.Price = 180;
            u.Articles.Add(a);
            de.Save(u);
            Assert.AreEqual(2, StaticRecorder.Messages.Count);
            Assert.AreEqual("Insert Into [r_Article] ([Name],[thePrice],[Reader_Id]) Values (@Name_0,@thePrice_1,@Reader_Id_2);\nSELECT last_insert_rowid();\n", StaticRecorder.Messages[0]);
            Assert.AreEqual("Update [r_Article] Set [thePrice]=@thePrice_0,[Reader_Id]=@Reader_Id_1  Where [Id] = @Id_2;\n", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject()
        {
            asUser u = asUser.New("Tom", 18);
            u.Id = 1; // Make it looks like read from database
            de.Save(u);
            Assert.AreEqual(0, StaticRecorder.Messages.Count);
            Assert.AreEqual("", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject2()
        {
            asUser u = asUser.New("Tom", 18);
            u.Id = 1; // Make it looks like read from database
            u.Name = "Tom";
            de.Save(u);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("Update [as_User] Set [theName]=@theName_0  Where [Id] = @Id_1;\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject3()
        {
            asUser u = asUser.New("Tom", 18);
            u.Id = 1; // Make it looks like read from database
            u.Name = "Jerry";
            u.Age = 25;
            de.Save(u);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("Update [as_User] Set [theName]=@theName_0,[Age]=@Age_1  Where [Id] = @Id_2;\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSmartUpdateForDynamicObject4()
        {
            DbEntry.UsingTransaction(delegate()
            {
                de.UsingTransaction(delegate()
                {
                    asUser u = asUser.New("Tom", 18);
                    u.Id = 1; // Make it looks like read from database
                    u.Name = "Tom";
                    de.Save(u);
                });
            });
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("Update [as_User] Set [theName]=@theName_0  Where [Id] = @Id_1;\n", StaticRecorder.LastMessage);
        }
    }
}
