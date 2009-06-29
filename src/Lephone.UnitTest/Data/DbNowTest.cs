using System;
using Lephone.Data;
using Lephone.Data.Builder;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class DbNowTest
    {
        #region Init

        [DbTable("DateTable")]
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

        [DbTable("DateTable")]
        public class DateTable3 : DbObject
        {
            [SpecialName]
            public DateTime SavedOn;

            public string Name;
        }

        [DbTable("DateTable")]
        public abstract class DateTable4 : DbObjectModel<DateTable4>
        {
            [SpecialName]
            public abstract DateTime SavedOn { get; set; }

            public abstract string Name { get; set; }
        }

        private readonly DbContext sqlite = new DbContext("SQLite");

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        #endregion

        [Test]
        public void TestCreatedOn()
        {
            var sb = new InsertStatementBuilder("user");
            sb.Values.Add(new KeyValue("CreatedOn", AutoValue.DbNow));
            SqlStatement sql = sb.ToSqlStatement(sqlite.Dialect);
            Assert.AreEqual("INSERT INTO [user] ([CreatedOn]) VALUES (DATETIME(CURRENT_TIMESTAMP, 'localtime'));\n", sql.SqlCommandText);
        }

        [Test]
        public void TestUpdatedOn()
        {
            var sb = new UpdateStatementBuilder("user");
            sb.Values.Add(new KeyValue("UpdatedOn", AutoValue.DbNow));
            SqlStatement sql = sb.ToSqlStatement(sqlite.Dialect);
            Assert.AreEqual("UPDATE [user] SET [UpdatedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime') ;\n", sql.SqlCommandText);
        }

        [Test]
        public void TestCreatedOnWithTable()
        {
            DateTable o = DateTable.New;
            o.Name = "tom";
            sqlite.Insert(o);
            Assert.AreEqual("INSERT INTO [DateTable] ([CreatedOn],[Name]) VALUES (DATETIME(CURRENT_TIMESTAMP, 'localtime'),@Name_0);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUpdatedOnWithTable()
        {
            DateTable o = DateTable.New;
            o.Name = "tom";
            o.Id = 1;
            sqlite.Update(o);
            Assert.AreEqual("UPDATE [DateTable] SET [UpdatedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime'),[Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUpdatedOnWithoutPartialUpdate()
        {
            var o = new DateTable2 {Name = "tom", Id = 1};
            sqlite.Update(o);
            Assert.AreEqual("UPDATE [DateTable] SET [UpdatedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime'),[Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSelectDatabaseTime()
        {
            DateTime dt = DbEntry.Context.GetDatabaseTime();
            TimeSpan ts = DateTime.Now.Subtract(dt);
            Assert.IsTrue(ts.TotalSeconds < 10);
        }

        [Test]
        public void TestSavedOn()
        {
            var o = new DateTable3 {Name = "tom"};
            sqlite.Insert(o);
            Assert.AreEqual("INSERT INTO [DateTable] ([SavedOn],[Name]) VALUES (DATETIME(CURRENT_TIMESTAMP, 'localtime'),@Name_0);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSavedOnForUpdate()
        {
            var o = new DateTable3 {Name = "tom", Id = 1};
            sqlite.Update(o);
            Assert.AreEqual("UPDATE [DateTable] SET [SavedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime'),[Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSavedOnForPartialUpdate()
        {
            DateTable4 o = DateTable4.New;
            o.Name = "tom";
            o.Id = 1;
            sqlite.Update(o);
            Assert.AreEqual("UPDATE [DateTable] SET [SavedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime'),[Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }
    }
}
