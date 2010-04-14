using System;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.CreateTable
{
    [TestFixture]
    public class SqlServer2008Test
    {
        [DbContext("SqlServer2008")]
        public abstract class Ss2008 : DbObjectModel<Ss2008>
        {
            public abstract string Name { get; set; }
            [SpecialName]
            public abstract DateTime CreatedOn { get; set; }
        }

        protected static void AssertSql(string sql)
        {
            Assert.AreEqual(sql.Replace("\r\n", "\n").Replace("    ", "\t"), StaticRecorder.LastMessage);
        }


        [Test]
        public void TestForCreatedOn()
        {
            var context = DbEntry.GetContext("SqlServer2008");
            context.Create(typeof(Ss2008));
            AssertSql(@"CREATE TABLE [Ss2008] (
    [Id] BIGINT IDENTITY NOT FOR REPLICATION NOT NULL PRIMARY KEY,
    [Name] NTEXT NOT NULL ,
    [CreatedOn] DATETIME NOT NULL 
);
<Text><30>()");
        }
    }
}
