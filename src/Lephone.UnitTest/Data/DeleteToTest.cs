using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using Lephone.Core;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [DeleteTo("UnregUser"), DbContext("SQLite")]
    public class DeleteToUser : DbObjectModel<DeleteToUser>
    {
        public string Name { get; set; }
    }

    [TestFixture]
    public class DeleteToTest
    {
        private static readonly DbContext Sqlite = EntryConfig.NewContext("SQLite");

        [TearDown]
        public void TearDown()
        {
            ClassHelper.SetValue(Sqlite, "_tableNames", null);
        }

        [Test]
        public void Test1()
        {
            var o = new DeleteToUser {Id = 2, Name = "tom"};
            Sqlite.Delete(o);
            // TODO: why all is _0 ?
            Assert.AreEqual("DELETE FROM [Delete_To_User] WHERE [Id] = @Id_0;\nINSERT INTO [UnregUser] ([Name],[DeletedOn]) VALUES (@Name_0,DATETIME(CURRENT_TIMESTAMP, 'localtime'));\n<Text><30>(@Id_0=2:Int64,@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate()
        {
            Sqlite.CreateDeleteToTable(typeof(DeleteToUser));
            Assert.AreEqual("CREATE TABLE [UnregUser] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[DeletedOn] DATETIME NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }
    }
}
