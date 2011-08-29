using Lephone.Data.Definition;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class ExcludeModelTest : DataTestBase
    {
        [DbTable("Categories")]
        public class ExludeCatagory : DbObjectModel<ExludeCatagory>
        {
            public string Name { get; set; }

            [Exclude]
            public ExcludeBook Book { get; set; }
        }

        [DbTable("Books")]
        public class ExcludeBook : DbObjectModel<ExcludeBook>
        {
            public string Name { get; set; }
            [DbColumn("Category_Id")]
            public long Cid { get; set; }
        }

        [Test]
        public void Test1()
        {
            var c = new ExludeCatagory {Name = "ttt"};
            var b = new ExcludeBook {Name = "bbb", Cid = 1};
            c.Book = b;
            c.Save();
            Assert.AreEqual(0, b.Id);
        }
    }
}
