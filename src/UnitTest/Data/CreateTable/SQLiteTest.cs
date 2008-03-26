
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.MockSql;
using Lephone.MockSql.Recorder;

using Lephone.UnitTest.Data.Objects;

#endregion

namespace Lephone.UnitTest.Data.CreateTable
{
    #region objects

    class MyTest1 : IDbObject
    {
        [DbKey]
        public long Id = 0;

        [Index]
        public string Name = null;
    }

    class MyTest2 : IDbObject
    {
        [DbKey]
        public long Id = 0;

        [Index(UNIQUE = true)]
        public string Name = null;
    }

    class MyTest3 : IDbObject
    {
        [DbKey]
        public long Id = 0;

        [Index("Name_Age", ASC = false, UNIQUE = true)]
        public string Name = null;

        [Index("Name_Age")]
        public int Age = 0;
    }

    [DbTable("MyTest")]
    class MyTest8 : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public long Id = 0;

        [DbKey(IsDbGenerate = false), Length(50)]
        public string Name = null;

        public int Age = 0;
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

    #endregion

    [TestFixture]
    public class SQLiteTest
    {
        private DbContext de = new DbContext(EntryConfig.GetDriver("SQLite"));

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        [Test]
        public void TestGuidMultiKey()
        {
            de.Create(typeof(GuidMultiKey));
            Assert.AreEqual("CREATE TABLE [Guid_Multi_Key] (\n\t[UserId] uniqueidentifier NOT NULL ,\n\t[RoleId] uniqueidentifier NOT NULL ,\n\tPRIMARY KEY([UserId], [RoleId])\n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test1()
        {
            de.Create(typeof(MyTest1));
            Assert.AreEqual("CREATE TABLE [My_Test1] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\nCREATE INDEX [IX_My_Test1_Name] ON [My_Test1] ([Name] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
            de.Create(typeof(MyTest2));
            Assert.AreEqual("CREATE TABLE [My_Test2] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\nCREATE UNIQUE INDEX [IX_My_Test2_Name] ON [My_Test2] ([Name] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test3()
        {
            de.Create(typeof(MyTest3));
            Assert.AreEqual("CREATE TABLE [My_Test3] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL ,\n\t[Age] int NOT NULL \n);\nCREATE UNIQUE INDEX [IX_My_Test3_Name_Age] ON [My_Test3] ([Name] DESC, [Age] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test4()
        {
            de.Create(typeof(Person));
            Assert.AreEqual("CREATE TABLE [People] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test5()
        {
            de.Create(typeof(Category));
            Assert.AreEqual("CREATE TABLE [Categories] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test6()
        {
            de.Create(typeof(PersonalComputer));
            Assert.AreEqual("CREATE TABLE [PCs] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL ,\n\t[Person_Id] bigint NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test7()
        {
            de.Create(typeof(Book));
            Assert.AreEqual("CREATE TABLE [Books] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL ,\n\t[Category_Id] bigint NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test8()
        {
            de.Create(typeof(MyTest8));
            Assert.AreEqual("CREATE TABLE [MyTest] (\n\t[Name] nvarchar (50) NOT NULL ,\n\t[Id] bigint NOT NULL ,\n\t[Age] int NOT NULL ,\n\tPRIMARY KEY([Name], [Id])\n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test9()
        {
            de.Create(typeof(Article));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Article] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test10()
        {
            de.Create(typeof(Reader));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Reader] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test11()
        {
            de.Create_ManyToManyMediTable(typeof(Reader), typeof(Article));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [R_Article_Reader] (\n\t[Article_Id] bigint NOT NULL ,\n\t[Reader_Id] bigint NOT NULL \n);\n" +
                "CREATE INDEX [IX_R_Article_Reader_Reader_Id] ON [R_Article_Reader] ([Reader_Id] ASC);\n" +
                "CREATE INDEX [IX_R_Article_Reader_Article_Id] ON [R_Article_Reader] ([Article_Id] ASC);\n<Text><30>()",
                StaticRecorder.Messages[0]);
        }

        [Test]
        public void TestGuidKey()
        {
            de.Create(typeof(GuidKey));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Guid_Key] (\n\t[Id] uniqueidentifier PRIMARY KEY,\n\t[Name] ntext NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestManyMore()
        {
            de.Create_ManyToManyMediTable(typeof(ManyMore), typeof(ManyMore1));
            Assert.AreEqual("CREATE TABLE [R_ManyMore_ManyMore1] (\n\t[ManyMore_Id] bigint NOT NULL ,\n\t[ManyMore1_Id] bigint NOT NULL \n);\nCREATE INDEX [IX_R_ManyMore_ManyMore1_ManyMore_Id] ON [R_ManyMore_ManyMore1] ([ManyMore_Id] ASC);\nCREATE INDEX [IX_R_ManyMore_ManyMore1_ManyMore1_Id] ON [R_ManyMore_ManyMore1] ([ManyMore1_Id] ASC);\n<Text><30>()", StaticRecorder.LastMessage);

            de.Create_ManyToManyMediTable(typeof(ManyMore), typeof(ManyMore2));
            Assert.AreEqual("CREATE TABLE [R_ManyMore_ManyMore2] (\n\t[ManyMore_Id] bigint NOT NULL ,\n\t[ManyMore2_Id] bigint NOT NULL \n);\nCREATE INDEX [IX_R_ManyMore_ManyMore2_ManyMore_Id] ON [R_ManyMore_ManyMore2] ([ManyMore_Id] ASC);\nCREATE INDEX [IX_R_ManyMore_ManyMore2_ManyMore2_Id] ON [R_ManyMore_ManyMore2] ([ManyMore2_Id] ASC);\n<Text><30>()", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestUnsigned()
        {
            de.Create(typeof(UnsignedTestTable));
            Assert.AreEqual("CREATE TABLE [Unsigned_Test_Table] (\n\t[Name] ntext NOT NULL ,\n\t[Age] int NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }
    }
}
