using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using Lephone.Util;
using NUnit.Framework;

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
        private static readonly DbContext sqlite = new DbContext("SQLite");

        [TearDown]
        public void TearDown()
        {
            ClassHelper.SetValue(sqlite, "TableNames", null);
        }

        [Test]
        public void Test1()
        {
            DeleteToUser o = DeleteToUser.New();
            o.Id = 2;
            o.Name = "tom";
            sqlite.Delete(o);
            // TODO: why all is _0 ?
            Assert.AreEqual("DELETE FROM [Delete_To_User] WHERE [Id] = @Id_0;\nINSERT INTO [UnregUser] ([Name],[DeletedOn]) VALUES (@Name_0,DATETIME(CURRENT_TIMESTAMP, 'localtime'));\n<Text><30>(@Id_0=2:Int64,@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate()
        {
            sqlite.CreateDeleteToTable(typeof(DeleteToUser));
            Assert.AreEqual("CREATE TABLE [UnregUser] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[DeletedOn] DATETIME NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }
    }
}
