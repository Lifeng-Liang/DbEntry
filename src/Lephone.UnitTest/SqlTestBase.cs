using Lephone.MockSql.Recorder;
using NUnit.Framework;

namespace Lephone.UnitTest
{
    public class SqlTestBase
    {
        [SetUp]
        public void SetUp()
        {
            StaticRecorder.ClearMessages();
            OnSetUp();
        }

        protected virtual void OnSetUp() { }

        [TearDown]
        public void TearDown()
        {
            OnTearDown();
        }

        protected virtual void OnTearDown() { }

        protected static void AssertSql(string sql)
        {
            Assert.AreEqual(sql.Replace("\r\n", "\n").Replace("    ", "\t"), StaticRecorder.LastMessage);
            StaticRecorder.ClearMessages();
        }
    }
}
