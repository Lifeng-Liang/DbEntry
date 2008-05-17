using Lephone.Data;
using Lephone.Data.Common;
using NUnit.Framework;
using Lephone.UnitTest.Data.Objects;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class VBHelperTest
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
        public void Test1()
        {
            using (VBHelper.NewTransaction())
            {
                Person p = new Person();
                p.Name = "uuu";
                DbEntry.Save(p);
                Person p1 = new Person();
                p1.Name = "iii";
                DbEntry.Save(p1);
                VBHelper.Commit();
            }
            Assert.AreEqual("uuu", DbEntry.GetObject<Person>(4).Name);
            Assert.AreEqual("iii", DbEntry.GetObject<Person>(5).Name);
        }

        [Test]
        public void Test2()
        {
            try
            {
                using (VBHelper.NewTransaction())
                {
                    Person p = new Person();
                    p.Name = "uuu";
                    DbEntry.Save(p);
                    Person p1 = new Person();
                    p1.Name = "iii";
                    DbEntry.Save(p1);
                    DbEntry.Context.ExecuteNonQuery("select form form"); // raise exception for test.
                    VBHelper.Commit();
                }
            }
            catch { }
            Person p2 = DbEntry.GetObject<Person>(4);
            Assert.IsNull(p2);
            Assert.IsNull(DbEntry.GetObject<Person>(5));
        }
    }
}
