using System;
using Leafing.Data.Common;
using Leafing.Data.SqlEntry;
using Leafing.MockSql.Recorder;
using Leafing.UnitTest.Mocks;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class DbTimeProviderTest
    {
        [Test]
        public void Test1()
        {
            MockMiscProvider.MockNow = new DateTime(2010, 12, 10, 10, 9 , 8);
            var provider = new DbTimeProvider(new DataProvider("SQLite"));

            StaticRecorder.CurRow.Add(new RowInfo("now", new DateTime(2010, 12, 10, 9, 8, 7)));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 7), provider.Now);

            MockMiscProvider.Add(new TimeSpan(0, 0, 50));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 57), provider.Now);

            StaticRecorder.CurRow.Add(new RowInfo("now", new DateTime(2010, 12, 10, 9, 8, 7)));
            MockMiscProvider.Add(new TimeSpan(0, 10, 0));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 7), provider.Now);
        }

        [Test]
        public void Test2()
        {
            MockMiscProvider.MockNow = new DateTime(2010, 12, 10, 10, 9, 8);
            var provider = new DbTimeProvider(new DataProvider("SQLite"));

            StaticRecorder.CurRow.Add(new RowInfo("now", new DateTime(2010, 12, 10, 9, 8, 7)));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 7), provider.Now);

            MockMiscProvider.Add(new TimeSpan(0, 0, 50));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 57), provider.Now);

            StaticRecorder.CurRow.Add(new RowInfo("now", new DateTime(2010, 12, 10, 9, 18, 7)));
            MockMiscProvider.Add(new TimeSpan(0, 10, 0));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 18, 7), provider.Now);

            MockMiscProvider.Add(new TimeSpan(0, 3, 8));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 21, 15), provider.Now);
        }
    }
}
