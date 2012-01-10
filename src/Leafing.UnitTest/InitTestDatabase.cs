using Leafing.Core;
using Leafing.Data.SqlEntry;
using NUnit.Framework;

namespace Leafing.UnitTest
{
    [TestFixture]
    public class InitTestDatabase
    {
        [Test, Ignore("Initialize database")]
        public void TestGenerateData()
        {
            var sql = ResourceHelper.ReadToEnd(typeof(InitTestDatabase), "TestTable.sql");
            var init = new DataProvider("Init");
            init.ExecuteNonQuery(sql);
        }
    }
}
