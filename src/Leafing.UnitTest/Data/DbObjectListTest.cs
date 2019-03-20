using System;
using System.Data;
using Leafing.Data;
using Leafing.Data.Definition;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    [DbTable("Reader")]
    public class MyReader : IDbObject {
        public long Id;
        public string Name;
    }

    [TestFixture]
    public class DbObjectListTest : DataTestBase {
        [Test]
        public void TestToDataTable() {
            DataTable dt = DbEntry.From<MyReader>().Where(Condition.Empty).OrderBy("Id").Select().ToDataTable();
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
        public void TestToDataTable2() {
            DataTable dt = DbEntry.From<NullableTable>().Where(Condition.Empty).OrderBy("Id").Select().ToDataTable();
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