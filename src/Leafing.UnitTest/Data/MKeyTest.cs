using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data
{
    public class Mkey : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public string FirstName;

        [DbKey(IsDbGenerate = false)]
        public string LastName;

        public int Age;
    }

    [DbContext("SQLite")]
    public class Mkey2 : IDbObject
    {
        [DbKey(IsDbGenerate = false)]
        public string FirstName;

        [DbKey(IsDbGenerate = false)]
        public string LastName;

        public int Age;
    }

    [DbContext("Firebird")]
    public class Mkey3 : IDbObject
    {
        [DbKey(IsDbGenerate = false), Length(50)]
        public string Name;

        [DbKey(IsDbGenerate = false)]
        public int Age;

        public bool Gender;
    }

    [TestFixture]
    public class MKeyTest : DataTestBase
    {
        [Test]
        public void TestForMkey()
        {
            DbEntry.Create(typeof(Mkey));

            var p1 = new Mkey { FirstName = "test", LastName = "next", Age = 11 };
            DbEntry.Insert(p1);

            var p2 = DbEntry.From<Mkey>().Where(p => p.FirstName == "test" && p.LastName == "next").Select()[0];
            Assert.AreEqual(11, p2.Age);

            p2.Age = 18;
            DbEntry.Update(p2);

            var p3 = DbEntry.From<Mkey>().Where(p => p.FirstName == "test" && p.LastName == "next").Select()[0];
            Assert.AreEqual(18, p3.Age);
        }

        [Test]
        public void TestMkeyForUpdate()
        {
            var p = new Mkey2 { FirstName = "test", LastName = "next", Age = 11 };
            DbEntry.Update(p);
            AssertSql(@"UPDATE [Mkey2] SET [Age]=@Age_0  WHERE ([FirstName] = @FirstName_1) AND ([LastName] = @LastName_2);
<Text><30>(@Age_0=11:Int32,@FirstName_1=test:String,@LastName_2=next:String)");
        }

        [Test]
        public void TestForFirebird()
        {
            // try create table first.
            var ctx = ModelContext.GetInstance(typeof(Mkey3));
            ctx.Operator.TryCreateTable();
            StaticRecorder.ClearMessages();
            // real test
            var o = new Mkey3 {Name = "test", Age = 18, Gender = true};
            DbEntry.Insert(o);
            Assert.AreEqual(1, StaticRecorder.Messages.Count);
            AssertSql(@"INSERT INTO ""MKEY3"" (""NAME"",""AGE"",""GENDER"") VALUES (@Name_0,@Age_1,@Gender_2);<Text><30>(@Name_0=test:String,@Age_1=18:Int32,@Gender_2=True:Boolean)");
        }
    }
}
