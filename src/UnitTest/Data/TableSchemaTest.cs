
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.SqlEntry;

namespace org.hanzify.llf.UnitTest.Data
{
    [TestFixture]
    public class TableSchemaTest
    {
        #region Init

        [SetUp]
        public void SetUp()
        {
            InitHelper.Init();
        }

        [TearDown]
        public void TearDown()
        {
            InitHelper.Clear();
        }

        #endregion

        [Test]
        public void Test1()
        {
            List<DbColumnInfo> ls = DbEntry.Context.GetDbColumnInfos("File");
            Assert.AreEqual(3, ls.Count);

            Assert.AreEqual("Id", ls[0].ColumnName);
            Assert.AreEqual(true, ls[0].IsKey);
            Assert.AreEqual(typeof(long), ls[0].DataType);

            Assert.AreEqual("Name", ls[1].ColumnName);
            Assert.AreEqual(false, ls[1].IsKey);
            Assert.AreEqual(typeof(string), ls[1].DataType);

            Assert.AreEqual("BelongsTo_Id", ls[2].ColumnName);
            Assert.AreEqual(false, ls[1].IsKey);
            Assert.AreEqual(typeof(int), ls[2].DataType);
        }
    }
}
