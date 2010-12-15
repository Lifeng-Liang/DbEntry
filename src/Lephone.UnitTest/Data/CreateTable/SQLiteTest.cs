using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.CreateTable
{
    #region objects

    public class CCC1 : DbObjectModel<IndexTestClass> {}

    public class IndexTestClass : DbObjectModel<IndexTestClass>
    {
        [BelongsTo, DbColumn("CCCId"), Index(IndexName = "xxx1", UNIQUE = true), Index(IndexName = "ccc1", UNIQUE = true)]
        public CCC1 CCC { get; set; }

        [Index(IndexName = "xxx1", UNIQUE = true)]
        public int QQQId { get; set; }

        [Length(50), Index(IndexName = "ccc1", UNIQUE = true)]
        public string UUUs { get; set; }
    }

    public class TableWithNonDbGenId : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public int Id;

        public string Name;
    }

    public class MyTest1 : IDbObject
    {
        [DbKey]
        public long Id;

        [Index]
        public string Name;
    }

    public class MyTest2 : IDbObject
    {
        [DbKey]
        public long Id;

        [Index(UNIQUE = true)]
        public string Name;
    }

    public class MyTest3 : IDbObject
    {
        [DbKey]
        public long Id;

        [Index("Name_Age", ASC = false, UNIQUE = true)]
        public string Name;

        [Index("Name_Age")]
        public int Age;
    }

    [DbTable("MyTest")]
    public class MyTest8 : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public long Id;

        [DbKey(IsDbGenerate = false), Length(50)]
        public string Name;

        public int Age;
    }

    public class UnsignedTestTable : IDbObject
    {
        public string Name;
        public uint Age;
    }

    public class GuidMultiKey : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public Guid RoleId;
        [DbKey(IsDbGenerate = false)]
        public Guid UserId;
    }

    public class BinaryAndBLOB : IDbObject
    {
        [Length(5, 5)]
        public byte[] password;

        public byte[] image;
    }

    [DbTable("Books")]
    public class crxBook : IDbObject
    {
        [DbKey] public int Id;

        [Length(20)]
        public string Name;

        [CrossTableName(Name = "book_and_category")]
        public HasAndBelongsToMany<crxCategory> Categories;

        public crxBook()
        {
            Categories = new HasAndBelongsToMany<crxCategory>(this, "Id", "Books_Id");
        }
    }

    [DbTable("Categories")]
    public class crxCategory : IDbObject
    {
        [DbKey] public int Id;

        [Length(20)]
        public string Name;

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

    [DbTable("tom:test_table")]
    public class compTableName : DbObjectModel<compTableName>
    {
        public string Name { get; set; }
    }

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
        [Test]
        public void TestGuidMultiKey()
        {
            Sqlite.Create(typeof(GuidMultiKey));
            Assert.AreEqual("CREATE TABLE [Guid_Multi_Key] (\n\t[UserId] UNIQUEIDENTIFIER NOT NULL ,\n\t[RoleId] UNIQUEIDENTIFIER NOT NULL ,\n\tPRIMARY KEY([UserId], [RoleId])\n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test1()
        {
            Sqlite.Create(typeof(MyTest1));
            Assert.AreEqual("CREATE TABLE [My_Test1] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\nCREATE INDEX [IX_My_Test1_Name] ON [My_Test1] ([Name] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
            Sqlite.Create(typeof(MyTest2));
            Assert.AreEqual("CREATE TABLE [My_Test2] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\nCREATE UNIQUE INDEX [IX_My_Test2_Name] ON [My_Test2] ([Name] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test3()
        {
            Sqlite.Create(typeof(MyTest3));
            Assert.AreEqual("CREATE TABLE [My_Test3] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[Age] INT NOT NULL \n);\nCREATE UNIQUE INDEX [IX_My_Test3_Name_Age] ON [My_Test3] ([Name] DESC, [Age] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test4()
        {
            Sqlite.Create(typeof(Person));
            Assert.AreEqual("CREATE TABLE [People] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test5()
        {
            Sqlite.Create(typeof(Category));
            Assert.AreEqual("CREATE TABLE [Categories] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test6()
        {
            Sqlite.Create(typeof(PersonalComputer));
            Assert.AreEqual("CREATE TABLE [PCs] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[Person_Id] BIGINT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test7()
        {
            Sqlite.Create(typeof(Book));
            Assert.AreEqual("CREATE TABLE [Books] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[Category_Id] BIGINT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test8()
        {
            Sqlite.Create(typeof(MyTest8));
            Assert.AreEqual("CREATE TABLE [MyTest] (\n\t[Name] NVARCHAR (50) NOT NULL ,\n\t[Id] BIGINT NOT NULL ,\n\t[Age] INT NOT NULL ,\n\tPRIMARY KEY([Name], [Id])\n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test9()
        {
            Sqlite.Create(typeof(Article));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Article] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test10()
        {
            Sqlite.Create(typeof(Reader));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Reader] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test11()
        {
            Sqlite.CreateCrossTable(typeof(Reader), typeof(Article));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [R_Article_Reader] (\n\t[Article_Id] BIGINT NOT NULL ,\n\t[Reader_Id] BIGINT NOT NULL \n);\n" +
                "CREATE INDEX [IX_R_Article_Reader_Reader_Id] ON [R_Article_Reader] ([Reader_Id] ASC);\n" +
                "CREATE INDEX [IX_R_Article_Reader_Article_Id] ON [R_Article_Reader] ([Article_Id] ASC);\n<Text><30>()",
                StaticRecorder.Messages[0]);
        }

        [Test]
        public void TestGuidKey()
        {
            Sqlite.Create(typeof(GuidKey));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Guid_Key] (\n\t[Id] UNIQUEIDENTIFIER NOT NULL  PRIMARY KEY,\n\t[Name] NTEXT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestManyMore()
        {
            Sqlite.CreateCrossTable(typeof(ManyMore), typeof(ManyMore1));
            Assert.AreEqual("CREATE TABLE [R_ManyMore_ManyMore1] (\n\t[ManyMore_Id] BIGINT NOT NULL ,\n\t[ManyMore1_Id] BIGINT NOT NULL \n);\nCREATE INDEX [IX_R_ManyMore_ManyMore1_ManyMore_Id] ON [R_ManyMore_ManyMore1] ([ManyMore_Id] ASC);\nCREATE INDEX [IX_R_ManyMore_ManyMore1_ManyMore1_Id] ON [R_ManyMore_ManyMore1] ([ManyMore1_Id] ASC);\n<Text><30>()", StaticRecorder.LastMessage);

            Sqlite.CreateCrossTable(typeof(ManyMore), typeof(ManyMore2));
            Assert.AreEqual("CREATE TABLE [R_ManyMore_ManyMore2] (\n\t[ManyMore_Id] BIGINT NOT NULL ,\n\t[ManyMore2_Id] BIGINT NOT NULL \n);\nCREATE INDEX [IX_R_ManyMore_ManyMore2_ManyMore_Id] ON [R_ManyMore_ManyMore2] ([ManyMore_Id] ASC);\nCREATE INDEX [IX_R_ManyMore_ManyMore2_ManyMore2_Id] ON [R_ManyMore_ManyMore2] ([ManyMore2_Id] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUnsigned()
        {
            Sqlite.Create(typeof(UnsignedTestTable));
            Assert.AreEqual("CREATE TABLE [Unsigned_Test_Table] (\n\t[Name] NTEXT NOT NULL ,\n\t[Age] INT NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreateIndex()
        {
            Sqlite.Create(typeof(IndexTestClass));
            AssertSql(
@"CREATE TABLE [Index_Test_Class] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
	[QQQId] INT NOT NULL ,
	[UUUs] NVARCHAR (50) NOT NULL ,
	[CCCId] BIGINT NOT NULL 
);
CREATE UNIQUE INDEX [IX_Index_Test_Class_xxx1] ON [Index_Test_Class] ([QQQId] ASC, [CCCId] ASC);
CREATE UNIQUE INDEX [IX_Index_Test_Class_ccc1] ON [Index_Test_Class] ([UUUs] ASC, [CCCId] ASC);
<Text><30>()");
        }

        [Test]
        public void TestBinaryAndBlob()
        {
            Sqlite.Create(typeof(BinaryAndBLOB));
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
            Sqlite.Create(typeof(crxBook));
            AssertSql(
@"CREATE TABLE [Books] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NVARCHAR (20) NOT NULL 
);
<Text><30>()");

            Sqlite.CreateCrossTable(typeof(crxBook), typeof(crxCategory));
            AssertSql(
@"CREATE TABLE [R_book_and_category] (
    [Books_Id] INT NOT NULL ,
    [Categories_Id] INT NOT NULL 
);
CREATE INDEX [IX_R_book_and_category_Books_Id] ON [R_book_and_category] ([Books_Id] ASC);
CREATE INDEX [IX_R_book_and_category_Categories_Id] ON [R_book_and_category] ([Categories_Id] ASC);
<Text><30>()");
        }

        [Test]
        public void TestDefineCrossTableName2()
        {
            Sqlite.Create(typeof(crxBook1));
            AssertSql(
@"CREATE TABLE [crx_Book1] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NVARCHAR (20) NOT NULL 
);
<Text><30>()");

            Sqlite.CreateCrossTable(typeof(crxBook1), typeof(crxCategory1));
            AssertSql(
@"CREATE TABLE [R_book_and_category] (
    [crxBook1_Id] BIGINT NOT NULL ,
    [crxCategory1_Id] BIGINT NOT NULL 
);
CREATE INDEX [IX_R_book_and_category_crxBook1_Id] ON [R_book_and_category] ([crxBook1_Id] ASC);
CREATE INDEX [IX_R_book_and_category_crxCategory1_Id] ON [R_book_and_category] ([crxCategory1_Id] ASC);
<Text><30>()");
        }

        [Test]
        public void TestTableName()
        {
            Sqlite.Create(typeof(compTableName));
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
            Sqlite.From<compTableName>().Where(p => p.Name == "tom").Select();
            AssertSql(@"SELECT [Id],[Name] FROM [tom].[test_table] WHERE [Name] = @Name_0;
<Text><60>(@Name_0=tom:String)");

            var c = new compTableName {Name = "tom"};
            Sqlite.Insert(c);
            AssertSql(@"INSERT INTO [tom].[test_table] ([Name]) VALUES (@Name_0);
SELECT LAST_INSERT_ROWID();
<Text><30>(@Name_0=tom:String)");

            c.Id = 2;
            c.Name = "jerry";
            Sqlite.Update(c);
            AssertSql(@"UPDATE [tom].[test_table] SET [Name]=@Name_0  WHERE [Id] = @Id_1;
<Text><30>(@Name_0=jerry:String,@Id_1=2:Int64)");

            Sqlite.Delete(c);
            AssertSql(@"DELETE FROM [tom].[test_table] WHERE [Id] = @Id_0;
<Text><30>(@Id_0=2:Int64)");
        }

        [Test]
        public void TestTableWithNonDbGenId()
        {
            Sqlite.Create(typeof(TableWithNonDbGenId));
            AssertSql(@"CREATE TABLE [Table_With_Non_Db_Gen_Id] (
    [Id] INT NOT NULL  PRIMARY KEY,
    [Name] NTEXT NOT NULL 
);
<Text><30>()");
        }

        [Test]
        public void TestTableNameForBelongsToColumn()
        {
            Sqlite.Create(typeof(ForTableName));
            AssertSql(@"CREATE TABLE [For_Table_Name] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL ,
    [For_TableName2_Id] BIGINT NOT NULL 
);
<Text><30>()");
        }

        [Test]
        public void TestDefineContext()
        {
            var context = DbEntry.GetContext(typeof(ForDefineContext));
            context.Create(typeof(ForDefineContext));
            AssertSql(@"CREATE TABLE [For_Define_Context] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL 
);
<Text><30>()");
        }

        [Test]
        public void TestDecimal()
        {
            Sqlite.Create(typeof(PrDecimal));
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
