using Lephone.Data.SqlEntry;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class SqlLogTest
    {
        [Test]
        public void Test1()
        {
            SqlRecorder.Start();

            var de = new DataProvider("SQLite");

            var sql = new SqlStatement("test log") {NeedLog = false};
            de.ExecuteNonQuery(sql);

            Assert.AreEqual(0, SqlRecorder.List.Count);

            sql.NeedLog = true;
            de.ExecuteNonQuery(sql);

            Assert.AreEqual(1, SqlRecorder.List.Count);
            Assert.AreEqual("test log<Text><30>()", SqlRecorder.LastMessage);

            SqlRecorder.Stop();
        }
    }
}
