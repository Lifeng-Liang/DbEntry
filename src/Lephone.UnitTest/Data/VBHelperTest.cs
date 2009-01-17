using Lephone.Data;
using Lephone.Data.Common;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class VBHelperTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            using (VBHelper.NewTransaction())
            {
                var p = new Person {Name = "uuu"};
                DbEntry.Save(p);
                var p1 = new Person {Name = "iii"};
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
                    var p = new Person {Name = "uuu"};
                    DbEntry.Save(p);
                    var p1 = new Person {Name = "iii"};
                    DbEntry.Save(p1);
                    DbEntry.Context.ExecuteNonQuery("select form form"); // raise exception for test.
                    VBHelper.Commit();
                }
            }
            catch { }
            var p2 = DbEntry.GetObject<Person>(4);
            Assert.IsNull(p2);
            Assert.IsNull(DbEntry.GetObject<Person>(5));
        }
    }
}
