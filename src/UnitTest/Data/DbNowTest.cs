
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Lephone.Data;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class DbNowTest
    {
        private DbContext de = new DbContext("SQLite");

        [Test]
        public void Test1()
        {
            InsertStatementBuilder sb = new InsertStatementBuilder("user");
            sb.Values.Add(new KeyValue("CreatedOn", DbNow.Value));
            SqlStatement sql = sb.ToSqlStatement(de.Dialect);
            Assert.AreEqual("Insert Into [user] ([CreatedOn]) Values (CURRENT_TIMESTAMP);\n", sql.SqlCommandText);
        }

        [Test]
        public void Test2()
        {
            UpdateStatementBuilder sb = new UpdateStatementBuilder("user");
            sb.Values.Add(new KeyValue("UpdatedOn", DbNow.Value));
            SqlStatement sql = sb.ToSqlStatement(de.Dialect);
            Assert.AreEqual("Update [user] Set [UpdatedOn]=CURRENT_TIMESTAMP ;\n", sql.SqlCommandText);
        }
    }
}
