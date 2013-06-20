using Leafing.Core.Logging;
using Leafing.Extra.Logging;
using NUnit.Framework;

namespace Leafing.UnitTest.util
{
    [TestFixture]
    public class LoggerTest : DataTestBase
    {
        [Test]
        public void Test1()
        {
            var logger = new Logger("Unit");
            var ss = logger.LogRecorders.ToArray();
            Assert.AreEqual(ss.Length, 2);
            Assert.AreEqual(ss[0].GetType(), typeof(DtsFileLogRecorder));
            Assert.AreEqual(ss[1].GetType(), typeof(ConsoleLogRecorder));
        }

        [Test]
        public void Test2()
        {
            var logger = new Logger("Unit2");
            var ss = logger.LogRecorders.ToArray();
            Assert.AreEqual(ss.Length, 1);
            Assert.AreEqual(ss[0].GetType(), typeof(ConsoleMessageLogRecorder));
        }

        [Test]
        public void Test3()
        {
            Logger.System.Trace("test");
            var list = LeafingLog.FindRecent(1);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("test", list[0].Message);
        }
    }
}
