using System;
using Leafing.Data;
using Leafing.Data.Builder;
using Leafing.Data.Builder.Clause;
using Leafing.Data.Definition;
using Leafing.Data.SqlEntry;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class DbNowTest
    {
        #region Init

        [DbTable("DateTable"), DbContext("SQLite")]
        public class DateTable : DbObjectModel<DateTable>
        {
            [SpecialName]
            public DateTime CreatedOn { get; set; }

            [SpecialName]
            public DateTime? UpdatedOn { get; set; }

            public string Name { get; set; }

			public static DateTable AsLoad(long id, string name)
			{
				var o = new DateTable{ Id = 1 };
				o.InitLoadedColumns ();
				o.Name = name;
				return o;
			}
        }

        [DbTable("DateTable"), DbContext("SQLite")]
        public class DateTable2 : DbObject
        {
            [SpecialName]
            public DateTime CreatedOn;

            [SpecialName]
            public DateTime? UpdatedOn;

            public string Name;
        }

        [DbTable("DateTable"), DbContext("SQLite")]
        public class DateTable3 : DbObject
        {
            [SpecialName]
            public DateTime SavedOn;

            public string Name;
        }

        [DbTable("DateTable"), DbContext("SQLite")]
        public class DateTable4 : DbObjectModel<DateTable4>
        {
            [SpecialName]
            public DateTime SavedOn { get; set; }

            public string Name { get; set; }

			public static DateTable4 AsLoad(long id, string name)
			{
				var o = new DateTable4{ Id = 1 };
				o.InitLoadedColumns ();
				o.Name = name;
				return o;
			}
        }

        [DbTable("DateTable"), DbContext("SQLite")]
        public class DateTable5 : DbObjectModel<DateTable5>
        {
            public string Name { get; set; }

            public int Count { get; set; }
        }

        [DbTable("DateTable"), DbContext("SQLite")]
        public class DateTable6 : DbObjectModel<DateTable6>
        {
            public long ShopId { get; set; }

            [SpecialName]
            public DateTime SavedOn { get; set; }

            public string Name { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        #endregion

        [Test]
        public void TestCreatedOn()
        {
            var dc = new DataProvider("SQLite");
            var sb = new InsertStatementBuilder(new FromClause("user"));
            sb.Values.Add(new KeyOpValue("CreatedOn", null, KvOpertation.Now));
            SqlStatement sql = sb.ToSqlStatement(dc.Dialect, null);
            Assert.AreEqual("INSERT INTO [user] ([CreatedOn]) VALUES (DATETIME(CURRENT_TIMESTAMP, 'localtime'));\n", sql.SqlCommandText);
        }

        [Test]
        public void TestUpdatedOn()
        {
            var dc = new DataProvider("SQLite");
            var sb = new UpdateStatementBuilder(new FromClause("user"));
            sb.Values.Add(new KeyOpValue("UpdatedOn", null, KvOpertation.Now));
            SqlStatement sql = sb.ToSqlStatement(dc.Dialect, null);
            Assert.AreEqual("UPDATE [user] SET [UpdatedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime') ;\n", sql.SqlCommandText);
        }

        [Test]
        public void TestCreatedOnWithTable()
        {
            var o = new DateTable {Name = "tom"};
            DbEntry.Insert(o);
            Assert.AreEqual("INSERT INTO [DateTable] ([CreatedOn],[Name]) VALUES (DATETIME(CURRENT_TIMESTAMP, 'localtime'),@Name_0);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUpdatedOnWithTable()
        {
			var o = DateTable.AsLoad(1, "tom");
            DbEntry.Update(o);
			Assert.AreEqual("UPDATE [DateTable] SET [Name]=@Name_0,[UpdatedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime')  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUpdatedOnWithoutPartialUpdate()
        {
            var o = new DateTable2 {Name = "tom", Id = 1};
            DbEntry.Update(o);
            Assert.AreEqual("UPDATE [DateTable] SET [UpdatedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime'),[Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSavedOn()
        {
            var o = new DateTable3 {Name = "tom"};
            DbEntry.Insert(o);
            Assert.AreEqual("INSERT INTO [DateTable] ([SavedOn],[Name]) VALUES (DATETIME(CURRENT_TIMESTAMP, 'localtime'),@Name_0);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSavedOnForUpdate()
        {
            var o = new DateTable3 {Name = "tom", Id = 1};
            DbEntry.Update(o);
            Assert.AreEqual("UPDATE [DateTable] SET [SavedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime'),[Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSavedOnForPartialUpdate()
        {
			var o = DateTable4.AsLoad(1, "tom");
            DbEntry.Update(o);
			Assert.AreEqual("UPDATE [DateTable] SET [Name]=@Name_0,[SavedOn]=DATETIME(CURRENT_TIMESTAMP, 'localtime')  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestSavedOnWithSetValue()
        {
            var o = new DateTable6 {Name = "tom", SavedOn = DateTime.Now, ShopId = 100};
            DbEntry.Save(o);
            Assert.AreEqual("INSERT INTO [DateTable] ([ShopId],[SavedOn],[Name]) VALUES (@ShopId_0,DATETIME(CURRENT_TIMESTAMP, 'localtime'),@Name_1);\nSELECT LAST_INSERT_ROWID();\n<Text><30>(@ShopId_0=100:Int64,@Name_1=tom:String)", StaticRecorder.LastMessage);
        }
    }
}
