using System.Collections.Generic;
using Leafing.Data;
using Leafing.Data.SqlEntry;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    [TestFixture]
    public class TableSchemaTest : DataTestBase {
        [Test, Ignore("NSQLite's bug.")]
        public void Test1() {
            List<DbColumnInfo> ls = DbEntry.Provider.GetDbColumnInfoList("File");
            Assert.AreEqual(3, ls.Count);

            Assert.AreEqual("Id", ls[0].ColumnName);
            //Assert.AreEqual(true, ls[0].IsKey);
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