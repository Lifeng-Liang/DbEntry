using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data {
    [DeleteTo("UnregUser"), DbContext("SQLite")]
    public class DeleteToUser : DbObjectModel<DeleteToUser> {
        public string Name { get; set; }
    }

    [TestFixture]
    public class DeleteToTest {
        [TearDown]
        public void TearDown() {
            //var ctx = ModelContext.GetInstance(typeof(DeleteToUser));
            //ctx.Provider.Driver.TableNames = null;
        }

        [Test]
        public void Test1() {
            var o = new DeleteToUser { Id = 2, Name = "tom" };
            o.Delete();
            // TODO: why all is _0 ?
            Assert.AreEqual("DELETE FROM [Delete_To_User] WHERE [Id] = @Id_0;\nINSERT INTO [UnregUser] ([Name],[DeletedOn]) VALUES (@Name_0,DATETIME(CURRENT_TIMESTAMP, 'localtime'));\n<Text><30>(@Id_0=2:Int64,@Name_0=tom:String)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate() {
            DbEntry.CreateDeleteToTable(typeof(DeleteToUser));
            Assert.AreEqual("CREATE TABLE [UnregUser] (\n\t[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,\n\t[Name] NTEXT NOT NULL ,\n\t[DeletedOn] DATETIME NOT NULL \n);\n<Text><30>()", StaticRecorder.LastMessage);
        }
    }
}