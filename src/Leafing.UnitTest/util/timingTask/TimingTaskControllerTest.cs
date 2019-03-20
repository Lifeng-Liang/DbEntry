using System;
using Leafing.Core.TimingTask;
using Leafing.Core.TimingTask.Timings;
using Leafing.UnitTest.Mocks;
using NUnit.Framework;

namespace Leafing.UnitTest.util.timingTask {
    [TestFixture]
    public class TimingTaskControllerTest {
        [Test]
        public void TestIt() {
            var t1 = new MockTask();
            var t2 = new MockTask();
            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 0, 0, 0));
            ITiming ti1 = new DayTiming(new TimeOfDayStructure(1, 2, 3));
            ITiming ti2 = new DayTiming(new TimeOfDayStructure(1, 3, 5));
            var ttc = new TimingTaskCollection(new TimingTask(t1, ti1), new TimingTask(t2, ti2));

            var mt = new MockTimer();
            var controller = new TimingTaskController(ttc, mt);
            controller.Start();

            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 1, 2, 2));
            mt.RaiseElapsed();

            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 1, 2, 2, 900));
            mt.RaiseElapsed();
            Assert.AreEqual(0, t1.Times);

            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 1, 2, 3));
            mt.RaiseElapsed();
            Assert.AreEqual(1, t1.Times);

            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 1, 2, 4));
            mt.RaiseElapsed();
            Assert.AreEqual(1, t1.Times);

            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 1, 2, 6));
            mt.RaiseElapsed();
            Assert.AreEqual(1, t1.Times);


            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 1, 3, 4));
            mt.RaiseElapsed();

            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 1, 3, 5));
            mt.RaiseElapsed();
            Assert.AreEqual(1, t2.Times);

            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 1, 3, 6));
            mt.RaiseElapsed();
            Assert.AreEqual(1, t2.Times);

            MockMiscProvider.MockNow = (new DateTime(2005, 1, 21, 1, 3, 7));
            mt.RaiseElapsed();
            Assert.AreEqual(1, t2.Times);


            Assert.AreEqual(1, t1.Times);
        }
    }
}