
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Lephone.Data;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;

namespace Lephone.UnitTest.Data
{
    public abstract class DateTable : DbObjectModel<DateTable>
    {
        [SpecialName]
        public abstract DateTime CreatedOn { get; set; }

        [SpecialName]
        public abstract DateTime? UpdatedOn { get; set; }

        public abstract string Name { get; set; }
    }

    [DbTable("DateTable")]
    public class DateTable2 : DbObject
    {
        [SpecialName]
        public DateTime CreatedOn;

        [SpecialName]
        public DateTime? UpdatedOn;

        public string Name;
    }

    [TestFixture]
    public class DbNowTest
    {
        private DbContext de = new DbContext("SQLite");

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        [Test]
        public void TestCreatedOn()
        {
            InsertStatementBuilder sb = new InsertStatementBuilder("user");
            sb.Values.Add(new KeyValue("CreatedOn", DbNow.Value));
            SqlStatement sql = sb.ToSqlStatement(de.Dialect);
            Assert.AreEqual("Insert Into [user] ([CreatedOn]) Values (datetime(current_timestamp, 'localtime'));\n", sql.SqlCommandText);
        }

        [Test]
        public void TestUpdatedOn()
        {
            UpdateStatementBuilder sb = new UpdateStatementBuilder("user");
            sb.Values.Add(new KeyValue("UpdatedOn", DbNow.Value));
            SqlStatement sql = sb.ToSqlStatement(de.Dialect);
            Assert.AreEqual("Update [user] Set [UpdatedOn]=datetime(current_timestamp, 'localtime') ;\n", sql.SqlCommandText);
        }

        [Test]
        public void TestCreatedOnWithTable()
        {
            DateTable o = DateTable.New();
            o.Name = "tom";
            de.Insert(o);
            Assert.AreEqual("Insert Into [DateTable] ([CreatedOn],[Name]) Values (datetime(current_timestamp, 'localtime'),@Name_0);\nSELECT last_insert_rowid();\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUpdatedOnWithTable()
        {
            DateTable o = DateTable.New();
            o.Name = "tom";
            o.Id = 1;
            de.Update(o);
            Assert.AreEqual("Update [DateTable] Set [UpdatedOn]=datetime(current_timestamp, 'localtime'),[Name]=@Name_0  Where [Id] = @Id_1;\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUpdatedOnWithoutPartialUpdate()
        {
            DateTable2 o = new DateTable2();
            o.Name = "tom";
            o.Id = 1;
            de.Update(o);
            Assert.AreEqual("Update [DateTable] Set [UpdatedOn]=datetime(current_timestamp, 'localtime'),[Name]=@Name_0  Where [Id] = @Id_1;\n", StaticRecorder.LastMessage);
        }
    }
}
