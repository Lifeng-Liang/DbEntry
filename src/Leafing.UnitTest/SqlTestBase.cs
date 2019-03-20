using Leafing.MockSql.Recorder;
using NUnit.Framework;

namespace Leafing.UnitTest {
    public class SqlTestBase {
        [SetUp]
        public void SetUp() {
            StaticRecorder.ClearMessages();
            OnSetUp();
        }

        protected virtual void OnSetUp() { }

        [TearDown]
        public void TearDown() {
            OnTearDown();
        }

        protected virtual void OnTearDown() { }

        protected static void AssertSql(string sql) {
            AssertSql(-1, sql);
            StaticRecorder.ClearMessages();
        }

        protected static void AssertSql(int n, string sql) {
            var act = n < 0 ? StaticRecorder.LastMessage : StaticRecorder.Messages[n];
            AssertSql(act, sql);
        }

        protected static void AssertSql(string act, string exp) {
            Assert.AreEqual(exp.Replace("\r\n", "\n").Replace("    ", "\t"),
                act.Replace("\r\n", "\n").Replace("    ", "\t"));
        }
    }
}