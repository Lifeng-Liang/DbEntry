using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class PartialUpdateTest
    {
        [DbContext("SQLite")]
        public class User : DbObjectModel<User>
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public static readonly DbContext Sqlite = EntryConfig.NewContext("SQLite");

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        [Test]
        public void Test1()
        {
            var u = new User {Id = 1, Name = "tom"};
            Sqlite.Save(u);
            Assert.AreEqual("UPDATE [User] SET [Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
            var u = new User {Id = 1, Age = 19};
            Sqlite.Save(u);
            Assert.AreEqual("UPDATE [User] SET [Age]=@Age_0  WHERE [Id] = @Id_1;\n<Text><30>(@Age_0=19:Int32,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }
    }
}
