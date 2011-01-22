using System;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;
using Lephone.MockSql.Recorder;
using Lephone.UnitTest.util;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class DbTimeProviderTest
    {
        [Test]
        public void Test1()
        {
            MockMiscProvider.Me.SetNow(new DateTime(2010, 12, 10, 10, 9 , 8));
            var provider = new DbTimeProvider(new DataProvider("SQLite"));

            StaticRecorder.CurRow.Add(new RowInfo("now", new DateTime(2010, 12, 10, 9, 8, 7)));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 7), provider.Now);

            MockMiscProvider.Me.Add(new TimeSpan(0, 0, 50));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 57), provider.Now);

            StaticRecorder.CurRow.Add(new RowInfo("now", new DateTime(2010, 12, 10, 9, 8, 7)));
            MockMiscProvider.Me.Add(new TimeSpan(0, 10, 0));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 7), provider.Now);
        }

        [Test]
        public void Test2()
        {
            MockMiscProvider.Me.SetNow(new DateTime(2010, 12, 10, 10, 9, 8));
            var provider = new DbTimeProvider(new DataProvider("SQLite"));

            StaticRecorder.CurRow.Add(new RowInfo("now", new DateTime(2010, 12, 10, 9, 8, 7)));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 7), provider.Now);

            MockMiscProvider.Me.Add(new TimeSpan(0, 0, 50));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 8, 57), provider.Now);

            StaticRecorder.CurRow.Add(new RowInfo("now", new DateTime(2010, 12, 10, 9, 18, 7)));
            MockMiscProvider.Me.Add(new TimeSpan(0, 10, 0));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 18, 7), provider.Now);

            MockMiscProvider.Me.Add(new TimeSpan(0, 3, 8));
            Assert.AreEqual(new DateTime(2010, 12, 10, 9, 21, 15), provider.Now);
        }
    }
}
