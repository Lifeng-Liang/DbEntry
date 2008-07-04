using Lephone.Data;
using Lephone.Data.Common;
using Lephone.MockSql.Recorder;
using Lephone.UnitTest.Data.Objects;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class FirebirdTest
    {
        private DbContext de = new DbContext("Firebird");

        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
        }

        [Test]
        public void TestCreateAndInsert()
        {
            PeopleModel o = PeopleModel.New();
            o.Name = "tom";
            de.Save(o);

            Assert.AreEqual(4, StaticRecorder.Messages.Count);
            Assert.AreEqual("CREATE TABLE \"PEOPLE\" (\"ID\" bigint NOT NULL PRIMARY KEY,\"NAME\" varchar (5) NOT NULL);<Text><30>()", StaticRecorder.Messages[0]);
            Assert.AreEqual("CREATE GENERATOR GEN_PEOPLE_ID;<Text><30>()", StaticRecorder.Messages[1]);
            Assert.AreEqual("select gen_id(GEN_PEOPLE_ID, 1) from RDB$DATABASE<Text><30>()", StaticRecorder.Messages[2]);
            // TODO: why the ID is int32 ?
            Assert.AreEqual(string.Format("Insert Into \"PEOPLE\" (\"NAME\",\"ID\") Values (@Name_0,@Id_1);<Text><30>(@Name_0=tom:String,@Id_1={0}:Int32)", o.Id), StaticRecorder.Messages[3]);
        }

        [Test]
        public void TestSelect()
        {
            de.From<PeopleModel>().Where(CK.K["Name"] == "tom" && CK.K["Age"] > 18).Select();
            Assert.AreEqual("Select \"ID\",\"NAME\" From \"PEOPLE\" Where (\"NAME\" = @Name_0) And (\"AGE\" > @Age_1);<Text><60>(@Name_0=tom:String,@Age_1=18:Int32)", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate()
        {
            de.Create(typeof(LephoneEnum));
            string Exp = "CREATE TABLE \"LEPHONE_ENUM\" (" +
                "\"ID\" bigint NOT NULL PRIMARY KEY," +
                "\"TYPE\" int NOT NULL ," +
                "\"NAME\" varchar (50) NOT NULL ," +
                "\"VALUE\" int" +
                ");<Text><30>()";
            Assert.AreEqual(Exp, StaticRecorder.Messages[0]);
            Assert.AreEqual("CREATE GENERATOR GEN_LEPHONE_ENUM_ID;<Text><30>()", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestDrop()
        {
            de.DropTable(typeof(LephoneEnum));
            Assert.AreEqual("Drop Table \"LEPHONE_ENUM\"<Text><30>()", StaticRecorder.Messages[0]);
            Assert.AreEqual("DROP GENERATOR GEN_LEPHONE_ENUM_ID;<Text><30>()", StaticRecorder.Messages[1]);
        }
    }
}
