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
            // TODO: why all is _0 ?
            Assert.AreEqual("Delete From [Delete_To_User] Where [Id] = @Id_0;\nInsert Into [UnregUser] ([Name],[DeletedOn]) Values (@Name_0,datetime(current_timestamp, 'localtime'));\n<Text><30>(@Id_0=2:Int64,@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate()
        {
            de.Create_DeleteToTable(typeof(DeleteToUser));
            Assert.AreEqual("CREATE TABLE [UnregUser] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] ntext NOT NULL ,\n\t[DeletedOn] datetime NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }
    }
}
