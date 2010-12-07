using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.MockSql.Recorder;
using NUnit.Framework;
using DescriptionAttribute = Lephone.Data.Definition.DescriptionAttribute;

namespace Lephone.UnitTest.Data.CreateTable
{
    [DbContext("SqlServerMock"), DbTable("MyGod")]
    public class User : DbObjectModel<User>
    {
        [Description("This's the name")]
        public string Name { get; set; }

        [Description("Age of user")]
        public int Age { get; set; }
    }

    [TestFixture]
    public class SqlServer2005Test
    {
        protected static void AssertSql(string sql)
        {
            Assert.AreEqual(sql.Replace("\r\n", "\n").Replace("    ", "\t"), StaticRecorder.LastMessage);
        }

        [Test]
        public void TestAddDescription()
        {
            var ctx = DbEntry.GetContext(typeof(User));
            ctx.Create(typeof(User));
            AssertSql(
@"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'This''s the name' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'MyGod', @level2type=N'COLUMN', @level2name=N'Name';
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Age of user' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'MyGod', @level2type=N'COLUMN', @level2name=N'Age';
<Text><30>()");
        }
    }
}
