
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using Lephone.Util;

namespace Lephone.UnitTest.Data
{
    [DeleteTo("UnregUser")]
    public abstract class DeleteToUser : DbObjectModel<DeleteToUser>
    {
        public abstract string Name { get; set; }
    }

    [TestFixture]
    public class DeleteToTest
    {
        private static DbContext de = new DbContext("SQLite");

        [TearDown]
        public void TearDown()
        {
            ClassHelper.SetValue(de, "TableNames", null);
        }

        [Test]
        public void Test1()
        {
            DeleteToUser o = DeleteToUser.New();
            o.Id = 2;
            o.Name = "tom";
            de.Delete(o);
            Assert.AreEqual("Delete From [DeleteToUser] Where [Id] = @Id_0;\nInsert Into [UnregUser] ([Name],[DeletedOn]) Values (@Name_0,datetime(current_timestamp, 'localtime'));\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate()
        {
            de.Create_DeleteToTable(typeof(DeleteToUser));
            Assert.AreEqual("CREATE TABLE [UnregUser] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL ,\n\t[DeletedOn] datetime NOT NULL \n);\n", StaticRecorder.LastMessage);
        }
    }
}
