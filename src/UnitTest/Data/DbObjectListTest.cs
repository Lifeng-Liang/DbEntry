
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;

namespace Lephone.UnitTest.Data
{
    [DbTable("Reader")]
    public class MyReader : IDbObject
    {
        public long Id;
        public string Name;
    }

    [TestFixture]
    public class DbObjectListTest
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
        public void TestToDataTable()
        {
            DataTable dt = DbEntry.From<MyReader>().Where(WhereCondition.EmptyCondition).OrderBy("Id").Select().ToDataTable();
            Assert.AreEqual("Reader", dt.TableName);
            Assert.AreEqual(3, dt.Rows.Count);
            Assert.AreEqual(1, dt.Rows[0]["Id"]);
            Assert.AreEqual("tom", dt.Rows[0]["Name"]);
            Assert.AreEqual(2, dt.Rows[1]["Id"]);
            Assert.AreEqual("jerry", dt.Rows[1]["Name"]);
            Assert.AreEqual(3, dt.Rows[2]["Id"]);
            Assert.AreEqual("mike", dt.Rows[2]["Name"]);
        }

        [Test]
        public void TestToDataTable2()
        {
            DataTable dt = DbEntry.From<NullableTable>().Where(WhereCondition.EmptyCondition).OrderBy("Id").Select().ToDataTable();
            Assert.AreEqual("NullTest", dt.TableName);
            Assert.AreEqual(4, dt.Rows.Count);

            Assert.AreEqual(1, dt.Rows[0]["Id"]);
            Assert.AreEqual("tom", dt.Rows[0]["Name"]);
            Assert.AreEqual(DBNull.Value, dt.Rows[0]["MyInt"]);
            Assert.AreEqual(true, dt.Rows[0]["MyBool"]);

            Assert.AreEqual(2, dt.Rows[1]["Id"]);
            Assert.AreEqual(DBNull.Value, dt.Rows[1]["Name"]);
            Assert.AreEqual(1, dt.Rows[1]["MyInt"]);
            Assert.AreEqual(false, dt.Rows[1]["MyBool"]);

            Assert.AreEqual(3, dt.Rows[2]["Id"]);
            Assert.AreEqual(DBNull.Value, dt.Rows[2]["Name"]);
            Assert.AreEqual(DBNull.Value, dt.Rows[2]["MyInt"]);
            Assert.AreEqual(DBNull.Value, dt.Rows[2]["MyBool"]);

            Assert.AreEqual(4, dt.Rows[3]["Id"]);
            Assert.AreEqual("tom", dt.Rows[3]["Name"]);
            Assert.AreEqual(1, dt.Rows[3]["MyInt"]);
            Assert.AreEqual(true, dt.Rows[3]["MyBool"]);
        }
    }
}
