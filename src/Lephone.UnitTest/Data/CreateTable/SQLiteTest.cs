using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.Driver;
using Lephone.MockSql.Recorder;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.CreateTable
{
    #region objects

    public class CCC1 : DbObjectModel<CCC1>
    {
        [HasMany]
        public IList<IndexTestClass> Indeies { get; set; }
    }

    [DbContext("SQLite")]
    public class IndexTestClass : DbObjectModel<IndexTestClass>
    {
        [BelongsTo, DbColumn("CCCId"), Index(IndexName = "xxx1", UNIQUE = true), Index(IndexName = "ccc1", UNIQUE = true)]
        public CCC1 CCC { get; set; }

        [Index(IndexName = "xxx1", UNIQUE = true)]
        public int QQQId { get; set; }

        [Length(50), Index(IndexName = "ccc1", UNIQUE = true)]
        public string UUUs { get; set; }
    }

    [DbContext("SQLite")]
    public class TableWithNonDbGenId : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public int Id;

        public string Name;
    }

    [DbContext("SQLite")]
    public class MyTest1 : IDbObject
    {
        [DbKey]
        public long Id;

        [Index]
        public string Name;
    }

    [DbContext("SQLite")]
    public class MyTest2 : IDbObject
    {
        [DbKey]
        public long Id;

        [Index(UNIQUE = true)]
        public string Name;
    }

    [DbContext("SQLite")]
    public class MyTest3 : IDbObject
    {
        [DbKey]
        public long Id;

        [Index("Name_Age", ASC = false, UNIQUE = true)]
        public string Name;

        [Index("Name_Age")]
        public int Age;
    }

    [DbTable("MyTest"), DbContext("SQLite")]
    public class MyTest8 : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public long Id;

        [DbKey(IsDbGenerate = false), Length(50)]
        public string Name;

        public int Age;
    }

    [DbContext("SQLite")]
    public class UnsignedTestTable : IDbObject
    {
        public string Name;
        public uint Age;
    }

    [DbContext("SQLite")]
    public class GuidMultiKey : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public Guid RoleId;
        [DbKey(IsDbGenerate = false)]
        public Guid UserId;
    }

    [DbContext("SQLite")]
    public class BinaryAndBLOB : IDbObject
    {
        [Length(5, 5)]
        public byte[] password;

        public byte[] image;
    }

    [DbTable("Books"), DbContext("SQLite")]
    public class crxBook : DbObjectModel<crxBook, int>
    {
        [Length(20)]
        public string Name { get; set; }

        [CrossTableName(Name = "book_and_category")]
        public HasAndBelongsToMany<crxCategory> Categories;

        public crxBook()
        {
            Categories = new HasAndBelongsToMany<crxCategory>(this, "Id", "Books_Id");
        }
    }

    [DbTable("Categories"), DbContext("SQLite")]
    public class crxCategory : DbObjectModel<crxCategory, int>
    {
        [Length(20)]
        public string Name { get; set; }

        [CrossTableName(Name = "book_and_category")]
        public HasAndBelongsToMany<crxBook> Books;

        public crxCategory()
        {
            Books = new HasAndBelongsToMany<crxBook>(this, "Id", "Categories_Id");
        }
    }

    public class crxBook1 : DbObjectModel<crxBook1>
    {
        [Length(20)]
        public string Name { get; set; }

        [HasAndBelongsToMany(CrossTableName = "book_and_category")]
        public IList<crxCategory1> Categories { get; set; }
    }

    public class crxCategory1 : DbObjectModel<crxCategory1>
    {
        [Length(20)]
        public string Name { get; set; }

        [HasAndBelongsToMany(CrossTableName = "book_and_category")]
        public IList<crxBook1> Books { get; set; }
    }

    [DbContext("SQLite")]
    public class crxBook1Sqlite : DbObjectModel<crxBook1Sqlite>
    {
        [Length(20)]
        public string Name { get; set; }

        [HasAndBelongsToMany(CrossTableName = "book_and_category")]
        public IList<crxCategory1Sqlite> Categories { get; set; }
    }

    [DbContext("SQLite")]
    public class crxCategory1Sqlite : DbObjectModel<crxCategory1Sqlite>
    {
        [Length(20)]
        public string Name { get; set; }

        [HasAndBelongsToMany(CrossTableName = "book_and_category")]
        public IList<crxBook1Sqlite> Books { get; set; }
    }

    [DbTable("tom:test_table"), DbContext("SQLite")]
    public class compTableName : DbObjectModel<compTableName>
    {
        public string Name { get; set; }
    }

    [DbContext("SQLite")]
    public class ForTableName : DbObjectModel<ForTableName>
    {
        public string Name { get; set; }

        [BelongsTo]
        public For_TableName2 Table2 { get; set; }
    }

    public class For_TableName2 : DbObjectModel<For_TableName2>
    {
        public string Name { get; set; }

        [HasMany]
        public IList<ForTableName> Tables { get; set; }
    }

    [DbContext("SQLite")]
    public class ForDefineContext : DbObjectModel<ForDefineContext>
    {
        public string Name { get; set; }
    }

    [DbContext("SQLite")]
    public class PrDecimal : DbObjectModel<PrDecimal>
    {
        public decimal Price { get; set; }

        [Precision(10, 4)]
        public decimal TotalFee { get; set; }

        public decimal? Price2 { get; set; }

        [Precision(10, 4)]
        public decimal? TotalFee2 { get; set; }
    }

    #endregion

    [TestFixture]
    public class SqliteTest : SqlTestBase
    {
        private static readonly DbDriver Driver = DbDriverFactory.Instance.GetInstance("SQLite");

        protected override void OnSetUp()
        {
            base.OnSetUp();
            Driver.TableNames = null;
        }

        [Test]
        public void TestGuidMultiKey()
        {
            DbEntry.Create(typeof(GuidMultiKey));
            Assert.AreEqual("CREATE TABLE [Guid_Multi_Key] (\n\t[RoleId] UNIQUEIDENTIFIER NOT NULL ,\n\t[UserId] UNIQUEIDENTIFIER NOT NULL ,\n\tPRIMARY KEY([RoleId], [UserId])\n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test1()
        {
            DbEntry.Create(typeof(MyTest1));
            Assert.AreEqual("CREATE TABLE [My_Test1] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\nCREATE INDEX [IX_My_Test1_Name] ON [My_Test1] ([Name] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
            DbEntry.Create(typeof(MyTest2));
            Assert.AreEqual("CREATE TABLE [My_Test2] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\nCREATE UNIQUE INDEX [IX_My_Test2_Name] ON [My_Test2] ([Name] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test3()
        {
            DbEntry.Create(typeof(MyTest3));
            Assert.AreEqual("CREATE TABLE [My_Test3] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[Age] INT NOT NULL \n);\nCREATE UNIQUE INDEX [IX_My_Test3_Name_Age] ON [My_Test3] ([Name] DESC, [Age] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test4()
        {

            DbEntry.Create(typeof(PersonSqlite));
            Assert.AreEqual("CREATE TABLE [People] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test5()
        {
            DbEntry.Create(typeof(CategorySqlite));
            Assert.AreEqual("CREATE TABLE [Categories] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test6()
        {
            DbEntry.Create(typeof(PersonalComputerSqlite));
            AssertSql(
@"CREATE TABLE [PCs] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL ,
    [Person_Id] BIGINT NULL ,
    FOREIGN KEY([Person_Id]) REFERENCES [People] ([Id]) 
);
<Text><30>()");
        }

        [Test]
        public void Test7()
        {
            DbEntry.Create(typeof(BookSqlite));
            AssertSql(
@"CREATE TABLE [Books] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL ,
    [Category_Id] BIGINT NULL ,
    FOREIGN KEY([Category_Id]) REFERENCES [Categories] ([Id]) 
);
<Text><30>()");
        }

        [Test]
        public void Test8()
        {
            DbEntry.Create(typeof(MyTest8));
            Assert.AreEqual("CREATE TABLE [MyTest] (\n\t[Id] BIGINT NOT NULL ,\n\t[Name] NVARCHAR (50) NOT NULL ,\n\t[Age] INT NOT NULL ,\n\tPRIMARY KEY([Id], [Name])\n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test9()
        {
            DbEntry.Create(typeof(ArticleSqlite));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Article] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test10()
        {
            DbEntry.Create(typeof(ReaderSqlite));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Reader] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test11()
        {
            DbEntry.CreateCrossTable(typeof(ReaderSqlite), typeof(ArticleSqlite));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            AssertSql(0,
@"CREATE TABLE [R_Article_Reader] (
    [Article_Id] BIGINT NOT NULL ,
    [Reader_Id] BIGINT NOT NULL ,
    FOREIGN KEY([Article_Id]) REFERENCES [Article] ([Id]) ,
    FOREIGN KEY([Reader_Id]) REFERENCES [Reader] ([Id]) 
);
CREATE INDEX [IX_R_Article_Reader_Article_Id] ON [R_Article_Reader] ([Article_Id] ASC);
CREATE INDEX [IX_R_Article_Reader_Reader_Id] ON [R_Article_Reader] ([Reader_Id] ASC);
<Text><30>()");
        }

        [Test]
        public void TestGuidKey()
        {
            DbEntry.Create(typeof(GuidKeySqlite));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Guid_Key_Sqlite] (\n\t[Id] UNIQUEIDENTIFIER NOT NULL  PRIMARY KEY,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestManyMore()
        {
            DbEntry.CreateCrossTable(typeof(ManyMoreSqlite), typeof(ManyMore1Sqlite));
            AssertSql(
@"CREATE TABLE [R_ManyMore_ManyMore1] (
    [ManyMore_Id] BIGINT NOT NULL ,
    [ManyMore1_Id] BIGINT NOT NULL ,
    FOREIGN KEY([ManyMore_Id]) REFERENCES [ManyMore] ([Id]) ,
    FOREIGN KEY([ManyMore1_Id]) REFERENCES [ManyMore1] ([Id]) 
);
CREATE INDEX [IX_R_ManyMore_ManyMore1_ManyMore_Id] ON [R_ManyMore_ManyMore1] ([ManyMore_Id] ASC);
CREATE INDEX [IX_R_ManyMore_ManyMore1_ManyMore1_Id] ON [R_ManyMore_ManyMore1] ([ManyMore1_Id] ASC);
<Text><30>()");

            DbEntry.CreateCrossTable(typeof(ManyMoreSqlite), typeof(ManyMore2Sqlite));
            AssertSql(
@"CREATE TABLE [R_ManyMore_ManyMore2] (
    [ManyMore_Id] BIGINT NOT NULL ,
    [ManyMore2_Id] BIGINT NOT NULL ,
    FOREIGN KEY([ManyMore_Id]) REFERENCES [ManyMore] ([Id]) ,
    FOREIGN KEY([ManyMore2_Id]) REFERENCES [ManyMore2] ([Id]) 
);
CREATE INDEX [IX_R_ManyMore_ManyMore2_ManyMore_Id] ON [R_ManyMore_ManyMore2] ([ManyMore_Id] ASC);
CREATE INDEX [IX_R_ManyMore_ManyMore2_ManyMore2_Id] ON [R_ManyMore_ManyMore2] ([ManyMore2_Id] ASC);
<Text><30>()");
        }

        [Test]
        public void TestUnsigned()
        {
            DbEntry.Create(typeof(UnsignedTestTable));
            Assert.AreEqual("CREATE TABLE [Unsigned_Test_Table] (\n\t[Name] NTEXT NOT NULL ,\n\t[Age] INT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreateIndex()
        {
            DbEntry.Create(typeof(IndexTestClass));
            AssertSql(
@"CREATE TABLE [Index_Test_Class] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
	[QQQId] INT NOT NULL ,
	[UUUs] NVARCHAR (50) NOT NULL ,
	[CCCId] BIGINT NULL ,
    FOREIGN KEY([CCCId]) REFERENCES [CCC1] ([Id]) 
);
CREATE UNIQUE INDEX [IX_Index_Test_Class_xxx1] ON [Index_Test_Class] ([QQQId] ASC, [CCCId] ASC);
CREATE UNIQUE INDEX [IX_Index_Test_Class_ccc1] ON [Index_Test_Class] ([UUUs] ASC, [CCCId] ASC);
<Text><30>()");
        }

        [Test]
        public void TestBinaryAndBlob()
        {
            DbEntry.Create(typeof(BinaryAndBLOB));
            AssertSql(
@"CREATE TABLE [Binary_And_BLOB] (
    [password] BINARY (5) NOT NULL ,
    [image] BLOB NOT NULL 
);
<Text><30>()");
        }

        [Test]
        public void TestValidationOfBinaryAndBlob()
        {
            var o = new BinaryAndBLOB {password = new byte[] {1}, image = new byte[] {}};
            var vh = new ValidateHandler();
            var isValid = vh.ValidateObject(o);
            Assert.IsFalse(isValid);

            o.password = new byte[]{1,2,3,4,5, 6};
            isValid = vh.ValidateObject(o);
            Assert.IsFalse(isValid);

            o.password = new byte[] {1, 2, 3, 4, 5};
            isValid = vh.ValidateObject(o);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void TestDefineCrossTableName()
        {
            DbEntry.Create(typeof(crxBook));
            AssertSql(
@"CREATE TABLE [Books] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NVARCHAR (20) NOT NULL 
);
<Text><30>()");

            DbEntry.CreateCrossTable(typeof(crxBook), typeof(crxCategory));
            AssertSql(
@"CREATE TABLE [R_book_and_category] (
    [Books_Id] INT NOT NULL ,
    [Categories_Id] INT NOT NULL ,
    FOREIGN KEY([Books_Id]) REFERENCES [Books] ([Id]) ,
    FOREIGN KEY([Categories_Id]) REFERENCES [Categories] ([Id]) 
);
CREATE INDEX [IX_R_book_and_category_Books_Id] ON [R_book_and_category] ([Books_Id] ASC);
CREATE INDEX [IX_R_book_and_category_Categories_Id] ON [R_book_and_category] ([Categories_Id] ASC);
<Text><30>()");
        }

        [Test]
        public void TestDefineCrossTableName2()
        {
            DbEntry.Create(typeof(crxBook1Sqlite));
            AssertSql(
@"CREATE TABLE [crx_Book1Sqlite] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NVARCHAR (20) NOT NULL 
);
<Text><30>()");

            DbEntry.CreateCrossTable(typeof(crxBook1Sqlite), typeof(crxCategory1Sqlite));
            AssertSql(
@"CREATE TABLE [R_book_and_category] (
    [crxBook1Sqlite_Id] BIGINT NOT NULL ,
    [crxCategory1Sqlite_Id] BIGINT NOT NULL ,
    FOREIGN KEY([crxBook1Sqlite_Id]) REFERENCES [crx_Book1Sqlite] ([Id]) ,
    FOREIGN KEY([crxCategory1Sqlite_Id]) REFERENCES [crx_Category1Sqlite] ([Id]) 
);
CREATE INDEX [IX_R_book_and_category_crxBook1Sqlite_Id] ON [R_book_and_category] ([crxBook1Sqlite_Id] ASC);
CREATE INDEX [IX_R_book_and_category_crxCategory1Sqlite_Id] ON [R_book_and_category] ([crxCategory1Sqlite_Id] ASC);
<Text><30>()");
        }

        [Test]
        public void TestTableName()
        {
            DbEntry.Create(typeof(compTableName));
            AssertSql(
@"CREATE TABLE [tom].[test_table] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL 
);
<Text><30>()");
        }

        [Test]
        public void TestTableNameForCrud()
        {
            DbEntry.From<compTableName>().Where(p => p.Name == "tom").Select();
            AssertSql(@"SELECT [Id],[Name] FROM [tom].[test_table] WHERE [Name] = @Name_0;
<Text><60>(@Name_0=tom:String)");

            var c = new compTableName {Name = "tom"};
            DbEntry.Insert(c);
            AssertSql(@"INSERT INTO [tom].[test_table] ([Name]) VALUES (@Name_0);
SELECT LAST_INSERT_ROWID();
<Text><30>(@Name_0=tom:String)");

            c.Id = 2;
            c.Name = "jerry";
            DbEntry.Update(c);
            AssertSql(@"UPDATE [tom].[test_table] SET [Name]=@Name_0  WHERE [Id] = @Id_1;
<Text><30>(@Name_0=jerry:String,@Id_1=2:Int64)");

            DbEntry.Delete(c);
            AssertSql(@"DELETE FROM [tom].[test_table] WHERE [Id] = @Id_0;
<Text><30>(@Id_0=2:Int64)");
        }

        [Test]
        public void TestTableWithNonDbGenId()
        {
            DbEntry.Create(typeof(TableWithNonDbGenId));
            AssertSql(@"CREATE TABLE [Table_With_Non_Db_Gen_Id] (
    [Id] INT NOT NULL  PRIMARY KEY,
    [Name] NTEXT NOT NULL 
);
<Text><30>()");
        }

        [Test]
        public void TestTableNameForBelongsToColumn()
        {
            DbEntry.Create(typeof(ForTableName));
            AssertSql(
@"CREATE TABLE [For_Table_Name] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL ,
    [For_TableName2_Id] BIGINT NULL ,
    FOREIGN KEY([For_TableName2_Id]) REFERENCES [For_Table_Name2] ([Id]) 
);
<Text><30>()");
        }

        [Test]
        public void TestDefineContext()
        {
            DbEntry.Create(typeof(ForDefineContext));
            AssertSql(@"CREATE TABLE [For_Define_Context] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL 
);
<Text><30>()");
        }

        [Test]
        public void TestDecimal()
        {
            DbEntry.Create(typeof(PrDecimal));
            AssertSql(@"CREATE TABLE [Pr_Decimal] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Price] DECIMAL (16,2) NOT NULL ,
    [TotalFee] DECIMAL (10,4) NOT NULL ,
    [Price2] DECIMAL (16,2) NULL ,
    [TotalFee2] DECIMAL (10,4) NULL 
);
<Text><30>()");
        }
    }
}
