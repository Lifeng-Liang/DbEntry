using Leafing.Data;
using Leafing.Data.SqlEntry;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    [TestFixture]
    public class ConnectionTest {
        [Test]
        public void Test1() {
            StaticRecorder.ConnectionOpendTimes = 0;
            DbEntry.UsingTransaction(delegate {
            });
            Assert.AreEqual(0, StaticRecorder.ConnectionOpendTimes);
        }

        [Test]
        public void Test2() {
            StaticRecorder.ConnectionOpendTimes = 0;
            var ctx = new DataProvider("SQLite");
            DbEntry.UsingTransaction(() => ctx.ExecuteNonQuery("select * from test"));
            Assert.AreEqual(1, StaticRecorder.ConnectionOpendTimes);
        }

        [Test]
        public void Test3() {
            StaticRecorder.ConnectionOpendTimes = 0;
            var ctx = new DataProvider("SQLite");
            DbEntry.UsingTransaction(delegate {
                ctx.ExecuteNonQuery("select * from test");
                ctx.ExecuteNonQuery("select * from test");
            });
            Assert.AreEqual(1, StaticRecorder.ConnectionOpendTimes);
        }

        [Test]
        public void Test4() {
            StaticRecorder.ConnectionOpendTimes = 0;
            var ctx = new DataProvider("SQLite");
            ctx.ExecuteNonQuery("select * from test");
            ctx.ExecuteNonQuery("select * from test");
            Assert.AreEqual(2, StaticRecorder.ConnectionOpendTimes);
        }
    }
}