
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;

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

        [Test]
        public void Test1()
        {
            DeleteToUser o = DeleteToUser.New();
            o.Id = 2;
            o.Name = "tom";
            de.Delete(o);
            Assert.AreEqual("Delete From [DeleteToUser] Where [Id] = @Id_0;\nInsert Into [UnregUser] ([Name],[DeletedOn]) Values (@Name_0,CURRENT_TIMESTAMP);\n", StaticRecorder.LastMessage);
        }
    }
}
