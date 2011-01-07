using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [DbContext("Firebird")]
    public class LephoneEnum2 : DbObject
    {
        public int Type;

        [Length(1, 50)]
        public string Name;

        public int? Value;
    }


    [TestFixture]
    public class FirebirdTest
    {
        private readonly DbContext _firebird = EntryConfig.NewContext("Firebird");

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        [Test]
        public void TestCreateAndInsert()
        {
            var o = new PeopleModel2 {Name = "tom"};
            _firebird.Save(o);

            Assert.AreEqual(4, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE \"PEOPLE\" (\"ID\" BIGINT NOT NULL PRIMARY KEY,\"NAME\" VARCHAR (5) CHARACTER SET UNICODE_FSS NOT NULL);<Text><30>()", StaticRecorder.Messages[0]);
            Assert.AreEqual("CREATE GENERATOR GEN_PEOPLE_ID;<Text><30>()", StaticRecorder.Messages[1]);
            Assert.AreEqual("SELECT GEN_ID(GEN_PEOPLE_ID, 1) FROM RDB$DATABASE<Text><30>()", StaticRecorder.Messages[2]);
            // TODO: why the ID is int32 ?
            Assert.AreEqual(string.Format("INSERT INTO \"PEOPLE\" (\"NAME\",\"ID\") VALUES (@Name_0,@Id_1);<Text><30>(@Name_0=tom:String,@Id_1={0}:Int32)", o.Id), StaticRecorder.Messages[3]);
        }

        [Test]
        public void TestSelect()
        {
            _firebird.From<PeopleModel2>().Where(CK.K["Name"] == "tom" && CK.K["Age"] > 18).Select();
            Assert.AreEqual("SELECT \"ID\",\"NAME\" FROM \"PEOPLE\" WHERE (\"NAME\" = @Name_0) AND (\"AGE\" > @Age_1);<Text><60>(@Name_0=tom:String,@Age_1=18:Int32)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate()
        {
            _firebird.Create(typeof(LephoneEnum2));
            const string exp = "CREATE TABLE \"LEPHONE_ENUM2\" (" +
                               "\"ID\" BIGINT NOT NULL PRIMARY KEY," +
                               "\"TYPE\" INT NOT NULL ," +
                               "\"NAME\" VARCHAR (50) CHARACTER SET UNICODE_FSS NOT NULL ," +
                               "\"VALUE\" INT" +
                               ");<Text><30>()";
            Assert.AreEqual(exp, StaticRecorder.Messages[0]);
            Assert.AreEqual("CREATE GENERATOR GEN_LEPHONE_ENUM2_ID;<Text><30>()", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestDrop()
        {
            _firebird.DropTable(typeof(LephoneEnum2));
            Assert.AreEqual("DROP TABLE \"LEPHONE_ENUM2\"<Text><30>()", StaticRecorder.Messages[0]);
            Assert.AreEqual("DROP GENERATOR GEN_LEPHONE_ENUM2_ID;<Text><30>()", StaticRecorder.Messages[1]);
        }
    }
}
