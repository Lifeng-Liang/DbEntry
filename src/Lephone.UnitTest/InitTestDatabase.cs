using Lephone.Core;
using Lephone.Data;
using NUnit.Framework;

namespace Lephone.UnitTest
{
    [TestFixture]
    public class InitTestDatabase
    {
        [Test, Ignore("Initialize database")]
        public void TestGenerateData()
        {
            ClassHelper.SetValue(typeof(DbEntry), "Context", DbEntry.GetContext("Init"));
            var sql = ResourceHelper.ReadToEnd(typeof(InitTestDatabase), "TestTable.sql");
            DbEntry.Context.ExecuteNonQuery(sql);
        }
    }
}
