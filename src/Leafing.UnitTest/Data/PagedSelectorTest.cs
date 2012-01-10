using Leafing.Data;
using Leafing.Data.Common;
using Leafing.Data.Definition;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [DbContext("SQLite")]
    public class PagedForOtherDb : DbObjectModel<PagedForOtherDb>
    {
        public string Name { get; set; }
    }

    [TestFixture]
    public class PagedSelectorTest : DataTestBase
    {
        [Test]
        public void TestPagedSelecor()
        {
            var ps = new PagedSelector<SinglePerson>(null, new OrderBy((DESC)"Id"), 2);
            Assert.AreEqual(3, ps.GetResultCount());
            var ls = ps.GetCurrentPage(0);
            Assert.AreEqual(2, ls.Count);
            Assert.AreEqual("Mike", ls[0].Name);
            Assert.AreEqual("Jerry", ls[1].Name);
            ls = ps.GetCurrentPage(1);
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("Tom", ls[0].Name);
        }

        [Test]
        public void TestPagedSelecor2()
        {
            var ps = new PagedSelector<SinglePerson>(null, new OrderBy((DESC)"Id"), 3);
            Assert.AreEqual(3, ps.GetResultCount());
            var ls = ps.GetCurrentPage(0);
            Assert.AreEqual(3, ls.Count);
            Assert.AreEqual("Mike", ls[0].Name);
            Assert.AreEqual("Jerry", ls[1].Name);
            Assert.AreEqual("Tom", ls[2].Name);
        }

        [Test]
        public void TestStaticPagedSelecor()
        {
            var ps = new StaticPagedSelector<SinglePerson>(null, new OrderBy((DESC)"Id"), 2);
            Assert.AreEqual(3, ps.GetResultCount());
            var ls = ps.GetCurrentPage(1);
            Assert.AreEqual(1, ls.Count);
            Assert.AreEqual("Mike", ls[0].Name);
            ls = ps.GetCurrentPage(0);
            Assert.AreEqual(2, ls.Count);
            Assert.AreEqual("Jerry", ls[0].Name);
            Assert.AreEqual("Tom", ls[1].Name);
        }

        [Test]
        public void TestStaticPagedSelecor2()
        {
            var ps = DbEntry.From<SinglePerson>().Where(Condition.Empty).OrderBy((DESC)"Id").PageSize(3).GetStaticPagedSelector();
            Assert.AreEqual(3, ps.GetResultCount());
            var ls = ps.GetCurrentPage(0);
            Assert.AreEqual(3, ls.Count);
            Assert.AreEqual("Mike", ls[0].Name);
            Assert.AreEqual("Jerry", ls[1].Name);
            Assert.AreEqual("Tom", ls[2].Name);
        }

        [Test]
        public void TestForOtherDb()
        {
            StaticRecorder.CurRow.Add(new RowInfo("__count__", 1L));
            var selector = PagedForOtherDb.Where(Condition.Empty).OrderBy("Id").PageSize(10).GetPagedSelector();
            var count = selector.GetResultCount();
            Assert.AreEqual(1, count);
            StaticRecorder.CurRow.Add(new RowInfo("Id", 1L));
            StaticRecorder.CurRow.Add(new RowInfo("Name", "tom"));
            var list = selector.GetCurrentPage(0);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual("tom", list[0].Name);
        }

        [Test]
        public void TestForCountIsZero()
        {
            var selector = DbEntry.From<SinglePerson>().Where(p => p.Id > 10000).OrderBy("Id").PageSize(10).GetPagedSelector();
            var count = (int)selector.GetResultCount();
            Assert.AreEqual(0, count);
        }
    }
}
