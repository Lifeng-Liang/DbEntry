using System;
using Lephone.Core.TimingTask;
using Lephone.Core.TimingTask.Timings;
using NUnit.Framework;

namespace Lephone.UnitTest.util.timingTask
{
	[TestFixture]
	public class TimingTaskControllerTest
	{
		[Test]
		public void TestIt()
		{
			var t1 = new MockTask();
			var t2 = new MockTask();
			var now = new MockMiscProvider(new DateTime(2005,1,21,0,0,0));
			ITiming ti1 = new DayTiming(new TimeOfDayStructure(1,2,3), now);
			ITiming ti2 = new DayTiming(new TimeOfDayStructure(1,3,5), now);
			var ttc = new TimingTaskCollection(new TimingTask(t1, ti1), new TimingTask(t2, ti2));

			var mt = new MockTimer();
			var controller = new TimingTaskController(ttc, mt);
			controller.Start();

			now.SetNow(new DateTime(2005,1,21,1,2,2));
			mt.RaiseElapsed();

			now.SetNow(new DateTime(2005,1,21,1,2,2,900));
			mt.RaiseElapsed();
			Assert.AreEqual(0, t1.Times);

			now.SetNow(new DateTime(2005,1,21,1,2,3));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t1.Times);

			now.SetNow(new DateTime(2005,1,21,1,2,4));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t1.Times);

			now.SetNow(new DateTime(2005,1,21,1,2,6));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t1.Times);


			now.SetNow(new DateTime(2005,1,21,1,3,4));
			mt.RaiseElapsed();

			now.SetNow(new DateTime(2005,1,21,1,3,5));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t2.Times);

			now.SetNow(new DateTime(2005,1,21,1,3,6));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t2.Times);

			now.SetNow(new DateTime(2005,1,21,1,3,7));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t2.Times);


			Assert.AreEqual(1, t1.Times);
		}
	}
}
