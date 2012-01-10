using System;
using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest.Data.CreateTable
{
    [TestFixture]
    public class SqlServer2008Test
    {
        [DbContext("SqlServer2008")]
        public class Ss2008 : DbObjectModel<Ss2008>
        {
            public string Name { get; set; }
            [SpecialName]
            public DateTime CreatedOn { get; set; }
        }

        protected static void AssertSql(string sql)
        {
            Assert.AreEqual(sql.Replace("\r\n", "\n").Replace("    ", "\t"), StaticRecorder.LastMessage);
        }


        [Test]
        public void TestForCreatedOn()
        {
            ModelContext.GetInstance(typeof(Ss2008)).Operator.Create();
            AssertSql(@"CREATE TABLE [Ss2008] (
    [Id] BIGINT IDENTITY NOT FOR REPLICATION NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(MAX) NOT NULL ,
    [CreatedOn] DATETIME NOT NULL 
);
<Text><30>()");
        }
    }
}
