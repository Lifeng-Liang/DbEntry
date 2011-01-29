using Lephone.Data;
using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [Cacheable, DbTable("Lock_Book")]
    public class CachedLockBook : DbObjectModel<CachedLockBook>
    {
        public string Name { get; set; }

        [SpecialName]
        public int LockVersion { get; set; }
    }

    public class LockBook : DbObjectModel<LockBook>
    {
        public string Name { get; set; }

        [SpecialName]
        public int LockVersion { get; set; }
    }

    [DbTable("Lock_Book")]
    public class LBook : DbObject
    {
        public string Name;

        [SpecialName]
        public int LockVersion;
    }

    [TestFixture]
    public class OptimisticLockingTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            var b = new LockBook {Name = "locker"};
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
            var b = new LockBook {Name = "locker"};
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
            var b = new LBook {Name = "l"};
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

        [Test]
        public void TestResave()
        {
            var b1 = new LockBook {Name = "test"};
            b1.Save();
            var b = LockBook.FindById(b1.Id);
            b.Name = "aa";
            b.Save();
            b.Name = "bb";
            b.Save(); // should not throw exception
        }

        [Test]
        public void TestUpdateAfterInsert()
        {
            var b = new LockBook {Name = "test"};
            b.Save();
            b.Name = "bb";
            b.Save(); // should not throw exception
        }

        [Test]
        public void TestCachedLockVersion()
        {
            var b = new CachedLockBook {Name = "abc"};
            b.Save();
            b.Name = "aaa";
            b.Save();
            var b1 = CachedLockBook.FindById(b.Id);
            Assert.AreEqual(1, b1.LockVersion);
        }
    }
}
