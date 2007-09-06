
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.UnitTest.Data.Objects;
using org.hanzify.llf.MockSql.Recorder;

namespace org.hanzify.llf.UnitTest.Data
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
            Assert.AreEqual("CREATE TABLE \"PEOPLE\" (\"ID\" bigint NOT NULL PRIMARY KEY,\"NAME\" varchar (5) NOT NULL);", StaticRecorder.Messages[0]);
            Assert.AreEqual("CREATE GENERATOR GEN_PEOPLE_ID;", StaticRecorder.Messages[1]);
            Assert.AreEqual("select gen_id(GEN_PEOPLE_ID, 1) from RDB$DATABASE", StaticRecorder.Messages[2]);
            Assert.AreEqual("Insert Into \"PEOPLE\" (\"NAME\",\"ID\") Values (@Name_0,@Id_1);", StaticRecorder.Messages[3]);
        }

        [Test]
        public void TestSelect()
        {
            de.From<PeopleModel>().Where(CK.K["Name"] == "tom" && CK.K["Age"] > 18).Select();
            Assert.AreEqual("Select \"ID\",\"NAME\" From \"PEOPLE\" Where (\"NAME\" = @Name_0) And (\"AGE\" > @Age_1);\n", StaticRecorder.LastMessage);
        }

        [Test]
        public void TestCreate()
        {
            de.Create(typeof(EnumTable));
            string Exp = "CREATE TABLE \"ENUMTABLE\" (" +
                "\"ID\" bigint NOT NULL PRIMARY KEY," +
                "\"TYPE\" int NOT NULL ," +
                "\"NAME\" varchar (50) NOT NULL ," +
                "\"VALUE\" int" +
                ");";
            Assert.AreEqual(Exp, StaticRecorder.Messages[0]);
            Assert.AreEqual("CREATE GENERATOR GEN_ENUMTABLE_ID;", StaticRecorder.Messages[1]);
        }

        [Test]
        public void TestDrop()
        {
            de.DropTable(typeof(EnumTable));
            Assert.AreEqual("Drop Table \"ENUMTABLE\"", StaticRecorder.Messages[0]);
            Assert.AreEqual("DROP GENERATOR GEN_ENUMTABLE_ID;", StaticRecorder.Messages[1]);
        }
    }
}
