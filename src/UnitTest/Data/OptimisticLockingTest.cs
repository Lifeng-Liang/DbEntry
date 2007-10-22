
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.MockSql.Recorder;
using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [DbTable("Books")]
    public abstract class LockBook : DbObjectModel<LockBook>
    {
        public abstract string Name { get; set; }

        [DbColumn("Category_Id"), SpecialName]
        public abstract int LockVersion { get; set; }

        public LockBook Init(string Name)
        {
            this.Name = Name;
            return this;
        }
    }

    [DbTable("Books")]
    public class LBook : DbObject
    {
        public string Name;

        [DbColumn("Category_Id"), SpecialName]
        public int LockVersion;
    }

    [TestFixture]
    public class OptimisticLockingTest
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
        public void Test1()
        {
            LockBook b = LockBook.New().Init("locker");
            b.Save();
            long id = b.Id;

            b = LockBook.FindById(id);
            Assert.AreEqual(0, b.LockVersion);
            b.Name = "1";
            b.Save();

            b = LockBook.FindById(id);
            Assert.AreEqual(1, b.LockVersion);
            b.Name = "2";
            b.Save();

            b = LockBook.FindById(id);
            Assert.AreEqual(2, b.LockVersion);
        }

        [Test, ExpectedException(typeof(DataException))]
        public void Test2()
        {
            LockBook b = LockBook.New().Init("locker");
            b.Save();
            long id = b.Id;

            b = LockBook.FindById(id);
            LockBook b1 = LockBook.FindById(id);
            Assert.AreEqual(0, b.LockVersion);
            b.Name = "1";
            b.Save();

            b1.Name = "0";
            b1.Save();
        }

        [Test]
        public void Test3()
        {
            LBook b = new LBook();
            b.Name = "l";
            DbEntry.Save(b);
            long n = b.Id;

            b = DbEntry.GetObject<LBook>(n);
            Assert.AreEqual(0, b.LockVersion);
            DbEntry.Save(b);
            b = DbEntry.GetObject<LBook>(n);
            Assert.AreEqual(1, b.LockVersion);
            DbEntry.Save(b);
            b = DbEntry.GetObject<LBook>(n);
            Assert.AreEqual(2, b.LockVersion);
        }
    }
}
