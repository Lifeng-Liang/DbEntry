using Leafing.Data.Definition;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    [TestFixture]
    public class PartialUpdateTest
    {
        [DbContext("SQLite")]
        public class User : DbObjectModel<User>
        {
            public string Name { get; set; }
            public int Age { get; set; }

			public static User AsLoad(long id)
			{
				var o = new User{ Id = 1 };
				o.InitLoadedColumns ();
				return o;
			}
        }

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        [Test]
        public void Test1()
        {
			var u = User.AsLoad(1);
			u.Name = "tom";
            u.Save();
            Assert.AreEqual("UPDATE [User] SET [Name]=@Name_0  WHERE [Id] = @Id_1;\n<Text><30>(@Name_0=tom:String,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }

        [Test]
        public void Test2()
        {
			var u = User.AsLoad(1);
			u.Age = 19;
            u.Save();
            Assert.AreEqual("UPDATE [User] SET [Age]=@Age_0  WHERE [Id] = @Id_1;\n<Text><30>(@Age_0=19:Int32,@Id_1=1:Int64)", StaticRecorder.LastMessage);
        }
    }
}
