using Lephone.Core;
using Lephone.Data;
using Lephone.Data.SqlEntry;
using NUnit.Framework;

namespace Lephone.UnitTest
{
    [TestFixture]
    public class InitTestDatabase
    {
        [Test, Ignore("Initialize database")]
        public void TestGenerateData()
        {
            ClassHelper.SetValue(typeof(DbEntry), "Provider", new DataProvider("Init"));
            var sql = ResourceHelper.ReadToEnd(typeof(InitTestDatabase), "TestTable.sql");
            DbEntry.Provider.ExecuteNonQuery(sql);
        }
    }
}
