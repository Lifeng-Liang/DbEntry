
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

    class MyTest1
    {
        [DbKey]
        public long Id = 0;

        [Index]
        public string Name = null;
    }

    class MyTest2
    {
        [DbKey]
        public long Id = 0;

        [Index(UNIQUE=true)]
        public string Name = null;
    }

    class MyTest3
    {
        [DbKey]
        public long Id = 0;

        [Index("Name_Age", ASC = false, UNIQUE = true)]
        public string Name = null;

        [Index("Name_Age")]
        public int Age = 0;
    }

    [DbTable("MyTest")]
    class MyTest8
    {
        [DbKey(IsDbGenerate=false)]
        public long Id = 0;

        [DbKey(IsDbGenerate=false), MaxLength(50)]
        public string Name = null;

        public int Age = 0;
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
        public void Test1()
        {
            de.Create(typeof(MyTest1));
            Assert.AreEqual("CREATE TABLE [MyTest1] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT  ,\n\t[Name] ntext NOT NULL \n);\nCREATE INDEX [IX_MyTest1_Name] ON [MyTest1] ([Name] ASC);\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
            de.Create(typeof(MyTest2));
            Assert.AreEqual("CREATE TABLE [MyTest2] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT  ,\n\t[Name] ntext NOT NULL \n);\nCREATE UNIQUE INDEX [IX_MyTest2_Name] ON [MyTest2] ([Name] ASC);\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test3()
        {
            de.Create(typeof(MyTest3));
            Assert.AreEqual("CREATE TABLE [MyTest3] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT  ,\n\t[Name] ntext NOT NULL ,\n\t[Age] int NOT NULL \n);\nCREATE UNIQUE INDEX [IX_MyTest3_Name_Age] ON [MyTest3] ([Name] DESC, [Age] ASC);\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test4()
        {
            de.Create(typeof(Person));
            Assert.AreEqual("CREATE TABLE [People] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT  ,\n\t[Name] ntext NOT NULL \n);\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test5()
        {
            de.Create(typeof(Category));
            Assert.AreEqual("CREATE TABLE [Categories] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT  ,\n\t[Name] ntext NOT NULL \n);\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test6()
        {
            de.Create(typeof(PersonalComputer));
            Assert.AreEqual("CREATE TABLE [PCs] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT  ,\n\t[Name] ntext NOT NULL ,\n\t[Person_Id] bigint NOT NULL \n);\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test7()
        {
            de.Create(typeof(Book));
            Assert.AreEqual("CREATE TABLE [Books] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT  ,\n\t[Name] ntext NOT NULL ,\n\t[Category_Id] bigint NOT NULL \n);\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test8()
        {
            de.Create(typeof(MyTest8));
            Assert.AreEqual("CREATE TABLE [MyTest] (\n\t[Name] nvarchar (50) NOT NULL ,\n\t[Id] bigint NOT NULL ,\n\t[Age] int NOT NULL ,\n\tPRIMARY KEY([Name], [Id])\n);\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test9()
        {
            de.Create(typeof(Article));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Article] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT  ,\n\t[Name] ntext NOT NULL \n);\n", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test10()
        {
            de.Create(typeof(Reader));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Reader] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT  ,\n\t[Name] ntext NOT NULL \n);\n", StaticRecorder.Messages[0]);
        }

        [Test]
        public void Test11()
        {
            de.CreateManyToManyMediTable(typeof(Reader), typeof(Article));
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE [Article_Reader] (\n\t[Article_Id] bigint NOT NULL ,\n\t[Reader_Id] bigint NOT NULL \n);\n" +
                "CREATE INDEX [IX_Article_Reader_Reader_Id] ON [Article_Reader] ([Reader_Id] ASC);\n" +
                "CREATE INDEX [IX_Article_Reader_Article_Id] ON [Article_Reader] ([Article_Id] ASC);\n",
                StaticRecorder.Messages[0]);
        }
    }
}
