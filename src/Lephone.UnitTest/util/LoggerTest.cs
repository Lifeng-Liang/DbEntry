using Lephone.Core.Logging;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
    [TestFixture]
    public class LoggerTest
    {
        [Test]
        public void Test1()
        {
            var logger = new Logger("Unit");
            var ss = logger.LogEvent.GetInvocationList();
            Assert.AreEqual(ss.Length, 2);
            Assert.AreEqual(ss[0].Target.GetType(), typeof(DtsFileLogRecorder));
            Assert.AreEqual(ss[1].Target.GetType(), typeof(ConsoleLogRecorder));
        }

        [Test]
        public void Test2()
        {
            var logger = new Logger("Unit2");
            var ss = logger.LogEvent.GetInvocationList();
            Assert.AreEqual(ss.Length, 1);
            Assert.AreEqual(ss[0].Target.GetType(), typeof(ConsoleMessageRecorder));
        }
    }
}
