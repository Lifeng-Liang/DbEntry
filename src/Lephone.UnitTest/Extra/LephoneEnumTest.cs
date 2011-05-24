using Lephone.Data;
using Lephone.Extra;
using NUnit.Framework;

namespace Lephone.UnitTest.Extra
{
    [TestFixture]
    public class LephoneEnumTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            SqlRecorder.Start();
            var o = new LephoneEnum {Name = "test", Type = 10, Value = null};
            DbEntry.Insert(o);
            AssertSql(@"INSERT INTO [Lephone_Enum] ([Type],[Name],[Value]) VALUES (@Type_0,@Name_1,@Value_2);
SELECT LAST_INSERT_ROWID();
<Text><30>(@Type_0=10:Int32,@Name_1=test:String,@Value_2=<NULL>:Int32)", SqlRecorder.LastMessage);
            SqlRecorder.Stop();
        }
    }
}
