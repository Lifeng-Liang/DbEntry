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

    class CCC1 : DbObjectModel<IndexTestClass> {}

    abstract class IndexTestClass : DbObjectModel<IndexTestClass>
    {
        [BelongsTo, DbColumn("CCCId"), Index(IndexName = "xxx1", UNIQUE = true), Index(IndexName = "ccc1", UNIQUE = true)]
        public abstract CCC1 CCC { get; set; }

        [Index(IndexName = "xxx1", UNIQUE = true)]
        public abstract int QQQId { get; set; }

        [Length(50), Index(IndexName = "ccc1", UNIQUE = true)]
        public abstract string UUUs { get; set; }
    }

    class MyTest1 : IDbObject
    {
        [DbKey]
        public long Id;

        [Index]
        public string Name;
    }

    class MyTest2 : IDbObject
    {
        [DbKey]
        public long Id;

        [Index(UNIQUE = true)]
        public string Name;
    }

    class MyTest3 : IDbObject
    {
        [DbKey]
        public long Id;

        [Index("Name_Age", ASC = false, UNIQUE = true)]
        public string Name;

        [Index("Name_Age")]
        public int Age;
    }

    [DbTable("MyTest")]
    class MyTest8 : IDbObject
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
            Categories = new HasAndBelongsToMany<crxCategory>(this, "Id");
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
            Books = new HasAndBelongsToMany<crxBook>(this, "Id");
        }
    }

    public abstract class crxBook1 : DbObjectModel<crxBook1>
    {
        [Length(20)]
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(CrossTableName = "book_and_category")]
        public abstract IList<crxCategory1> Categories { get; set; }
    }

    public abstract class crxCategory1 : DbObjectModel<crxCategory1>
    {
        [Length(20)]
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(CrossTableName = "book_and_category")]
        public abstract IList<crxBook1> Books { get; set; }
    }

    [DbTable("tom:test_table")]
    public abstract class compTableName : DbObjectModel<compTableName>
    {
        public abstract string Name { get; set; }
    }


    #endregion

    [TestFixture]
    public class SQLiteTest : SqlTestBase
    {
        [Test]
        public void TestGuidMultiKey()
        {
            sqlite.Create(typeof(GuidMultiKey));
            Assert.AreEqual("CREATE TABLE [Guid_Multi_Key] (\n\t[UserId] uniqueidentifier NOT NULL ,\n\t[RoleId] uniqueidentifier NOT NULL ,\n\tPRIMARY KEY([UserId], [RoleId])\n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test1()
        {
            sqlite.Create(typeof(MyTest1));
            Assert.AreEqual("CREATE TABLE [My_Test1] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\nCREATE INDEX [IX_My_Test1_Name] ON [My_Test1] ([Name] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
            sqlite.Create(typeof(MyTest2));
            Assert.AreEqual("CREATE TABLE [My_Test2] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\nCREATE UNIQUE INDEX [IX_My_Test2_Name] ON [My_Test2] ([Name] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test3()
        {
            sqlite.Create(typeof(MyTest3));
            Assert.AreEqual("CREATE TABLE [My_Test3] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL ,\n\t[Age] int NOT NULL \n);\nCREATE UNIQUE INDEX [IX_My_Test3_Name_Age] ON [My_Test3] ([Name] DESC, [Age] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test4()
        {
            sqlite.Create(typeof(Person));
            Assert.AreEqual("CREATE TABLE [People] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test5()
        {
            sqlite.Create(typeof(Category));
            Assert.AreEqual("CREATE TABLE [Categories] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test6()
        {
            sqlite.Create(typeof(PersonalComputer));
            Assert.AreEqual("CREATE TABLE [PCs] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL ,\n\t[Person_Id] bigint NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test7()
        {
            sqlite.Create(typeof(Book));
            Assert.AreEqual("CREATE TABLE [Books] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL ,\n\t[Category_Id] bigint NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test8()
        {
            sqlite.Create(typeof(MyTest8));
            Assert.AreEqual("CREATE TABLE [MyTest] (\n\t[Name] nvarchar (50) NOT NULL ,\n\t[Id] bigint NOT NULL ,\n\t[Age] int NOT NULL ,\n\tPRIMARY KEY([Name], [Id])\n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test9()
        {
            sqlite.Create(typeof(Article));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Article] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test10()
        {
            sqlite.Create(typeof(Reader));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Reader] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test11()
        {
            sqlite.CreateCrossTable(typeof(Reader), typeof(Article));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [R_Article_Reader] (\n\t[Article_Id] bigint NOT NULL ,\n\t[Reader_Id] bigint NOT NULL \n);\n" +
                "CREATE INDEX [IX_R_Article_Reader_Reader_Id] ON [R_Article_Reader] ([Reader_Id] ASC);\n" +
                "CREATE INDEX [IX_R_Article_Reader_Article_Id] ON [R_Article_Reader] ([Article_Id] ASC);\n<Text><30>()",
                StaticRecorder.Messages[0]);
        }

        [Test]
        public void TestGuidKey()
        {
            sqlite.Create(typeof(GuidKey));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Guid_Key] (\n\t[Id] uniqueidentifier NOT NULL  PRIMARY KEY,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestManyMore()
        {
            sqlite.CreateCrossTable(typeof(ManyMore), typeof(ManyMore1));
            Assert.AreEqual("CREATE TABLE [R_ManyMore_ManyMore1] (\n\t[ManyMore_Id] bigint NOT NULL ,\n\t[ManyMore1_Id] bigint NOT NULL \n);\nCREATE INDEX [IX_R_ManyMore_ManyMore1_ManyMore_Id] ON [R_ManyMore_ManyMore1] ([ManyMore_Id] ASC);\nCREATE INDEX [IX_R_ManyMore_ManyMore1_ManyMore1_Id] ON [R_ManyMore_ManyMore1] ([ManyMore1_Id] ASC);\n<Text><30>()", StaticRecorder.LastMessage);

            sqlite.CreateCrossTable(typeof(ManyMore), typeof(ManyMore2));
            Assert.AreEqual("CREATE TABLE [R_ManyMore_ManyMore2] (\n\t[ManyMore_Id] bigint NOT NULL ,\n\t[ManyMore2_Id] bigint NOT NULL \n);\nCREATE INDEX [IX_R_ManyMore_ManyMore2_ManyMore_Id] ON [R_ManyMore_ManyMore2] ([ManyMore_Id] ASC);\nCREATE INDEX [IX_R_ManyMore_ManyMore2_ManyMore2_Id] ON [R_ManyMore_ManyMore2] ([ManyMore2_Id] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUnsigned()
        {
            sqlite.Create(typeof(UnsignedTestTable));
            Assert.AreEqual("CREATE TABLE [Unsigned_Test_Table] (\n\t[Name] ntext NOT NULL ,\n\t[Age] int NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreateIndex()
        {
            sqlite.Create(typeof(IndexTestClass));
            AssertSql(
@"CREATE TABLE [Index_Test_Class] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
	[QQQId] int NOT NULL ,
	[UUUs] nvarchar (50) NOT NULL ,
	[CCCId] bigint NOT NULL 
);
CREATE UNIQUE INDEX [IX_Index_Test_Class_xxx1] ON [Index_Test_Class] ([QQQId] ASC, [CCCId] ASC);
CREATE UNIQUE INDEX [IX_Index_Test_Class_ccc1] ON [Index_Test_Class] ([UUUs] ASC, [CCCId] ASC);
<Text><30>()");
        }

        [Test]
        public void TestBinaryAndBLOB()
        {
            sqlite.Create(typeof(BinaryAndBLOB));
            AssertSql(
@"CREATE TABLE [Binary_And_BLOB] (
    [password] binary (5) NOT NULL ,
    [image] blob NOT NULL 
);
<Text><30>()");
        }

        [Test]
        public void TestValidationOfBinaryAndBLOB()
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
            sqlite.Create(typeof(crxBook));
            AssertSql(
@"CREATE TABLE [Books] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] nvarchar (20) NOT NULL 
);
<Text><30>()");

            sqlite.CreateCrossTable(typeof(crxBook), typeof(crxCategory));
            AssertSql(
@"CREATE TABLE [R_book_and_category] (
    [Books_Id] int NOT NULL ,
    [Categories_Id] int NOT NULL 
);
CREATE INDEX [IX_R_book_and_category_Books_Id] ON [R_book_and_category] ([Books_Id] ASC);
CREATE INDEX [IX_R_book_and_category_Categories_Id] ON [R_book_and_category] ([Categories_Id] ASC);
<Text><30>()");
        }

        [Test]
        public void TestDefineCrossTableName2()
        {
            sqlite.Create(typeof(crxBook1));
            AssertSql(
@"CREATE TABLE [crx_Book1] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] nvarchar (20) NOT NULL 
);
<Text><30>()");

            sqlite.CreateCrossTable(typeof(crxBook1), typeof(crxCategory1));
            AssertSql(
@"CREATE TABLE [R_book_and_category] (
    [crx_Book1_Id] bigint NOT NULL ,
    [crx_Category1_Id] bigint NOT NULL 
);
CREATE INDEX [IX_R_book_and_category_crx_Book1_Id] ON [R_book_and_category] ([crx_Book1_Id] ASC);
CREATE INDEX [IX_R_book_and_category_crx_Category1_Id] ON [R_book_and_category] ([crx_Category1_Id] ASC);
<Text><30>()");
        }

        [Test]
        public void TestTableName()
        {
            sqlite.Create(typeof(compTableName));
            AssertSql(
@"CREATE TABLE [tom].[test_table] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] ntext NOT NULL 
);
<Text><30>()");
        }

        [Test]
        public void TestTableNameForCRUD()
        {
            sqlite.From<compTableName>().Where(p => p.Name == "tom").Select();
            AssertSql(@"Select [Id],[Name] From [tom].[test_table] Where [Name] = @Name_0;
<Text><60>(@Name_0=tom:String)");

            var c = compTableName.New();
            c.Name = "tom";
            sqlite.Insert(c);
            AssertSql(@"Insert Into [tom].[test_table] ([Name]) Values (@Name_0);
SELECT last_insert_rowid();
<Text><30>(@Name_0=tom:String)");

            c.Id = 2;
            sqlite.Update(c);
            AssertSql(@"Update [tom].[test_table] Set [Name]=@Name_0  Where [Id] = @Id_1;
<Text><30>(@Name_0=tom:String,@Id_1=2:Int64)");

            sqlite.Delete(c);
            AssertSql(@"Delete From [tom].[test_table] Where [Id] = @Id_0;
<Text><30>(@Id_0=2:Int64)");
        }
    }
}
