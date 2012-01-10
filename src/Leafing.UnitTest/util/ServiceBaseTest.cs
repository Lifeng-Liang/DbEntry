using System.Threading;
using Leafing.Core;
using Leafing.Core.Setting;
using Leafing.UnitTest.Mocks;
using NUnit.Framework;

namespace Leafing.UnitTest.util
{
    [TestFixture]
    public class ServiceBaseTest
    {
        public class MyService : ServiceBase
        {
            public bool RunningTask;

            protected override void OnStarting()
            {
            }

            protected override void OnStopping()
            {
                RunningTask = false;
            }

            protected override void RunControllerTask()
            {
                RunningTask = true;
            }
        }

        [Test]
        public void Test1()
        {
            MockMiscProvider.SleepMilliSecends = 0;
            MockMiscProvider.MockSystemRunningMillisecends = CoreSettings.MinStartTicks - 1;
            var svc = new MyService();
            svc.Start();
            while(!svc.RunningTask)
            {
                Thread.Sleep(100);
            }
            svc.Stop();
            Assert.AreEqual(CoreSettings.DelayToStart, MockMiscProvider.SleepMilliSecends);
        }

        [Test]
        public void Test2()
        {
            MockMiscProvider.SleepMilliSecends = 0;
            MockMiscProvider.MockSystemRunningMillisecends = CoreSettings.MinStartTicks + 1;
            var svc = new MyService();
            svc.Start();
            while (!svc.RunningTask)
            {
                Thread.Sleep(100);
            }
            svc.Stop();
            Assert.AreEqual(0, MockMiscProvider.SleepMilliSecends);
        }
    }
}
