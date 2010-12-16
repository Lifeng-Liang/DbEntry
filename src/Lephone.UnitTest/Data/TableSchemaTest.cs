using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.SqlEntry;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class TableSchemaTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            List<DbColumnInfo> ls = DbEntry.Context.GetDbColumnInfoList("File");
            Assert.AreEqual(3, ls.Count);

            Assert.AreEqual("Id", ls[0].ColumnName);
            Assert.AreEqual(true, ls[0].IsKey);
            Assert.AreEqual(typeof(long), ls[0].DataType);

            Assert.AreEqual("Name", ls[1].ColumnName);
            Assert.AreEqual(false, ls[1].IsKey);
            Assert.AreEqual(typeof(string), ls[1].DataType);

            Assert.AreEqual("BelongsTo_Id", ls[2].ColumnName);
            Assert.AreEqual(false, ls[1].IsKey);
            Assert.AreEqual(typeof(long), ls[2].DataType);
        }
    }
}
