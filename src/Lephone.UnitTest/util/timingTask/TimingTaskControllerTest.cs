using System;
using Lephone.Util.TimingTask;
using Lephone.Util.TimingTask.Timings;
using NUnit.Framework;

namespace Lephone.UnitTest.util.timingTask
{
	[TestFixture]
	public class TimingTaskControllerTest
	{
		[Test]
		public void TestIt()
		{
			MockTask t1 = new MockTask();
			MockTask t2 = new MockTask();
			MockNowTimeProvider Now = new MockNowTimeProvider(new DateTime(2005,1,21,0,0,0));
			ITiming ti1 = new DayTiming(new TimeOfDayStructure(1,2,3), Now);
			ITiming ti2 = new DayTiming(new TimeOfDayStructure(1,3,5), Now);
			TimingTaskCollection ttc = new TimingTaskCollection(new TimingTask(t1, ti1), new TimingTask(t2, ti2));

			MockTimer mt = new MockTimer();
			TimingTaskController Controller = new TimingTaskController(ttc, mt);
			Controller.Start();

			Now.SetNow(new DateTime(2005,1,21,1,2,2));
			mt.RaiseElapsed();

			Now.SetNow(new DateTime(2005,1,21,1,2,2,900));
			mt.RaiseElapsed();
			Assert.AreEqual(0, t1.Times);

			Now.SetNow(new DateTime(2005,1,21,1,2,3));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t1.Times);

			Now.SetNow(new DateTime(2005,1,21,1,2,4));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t1.Times);

			Now.SetNow(new DateTime(2005,1,21,1,2,6));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t1.Times);


			Now.SetNow(new DateTime(2005,1,21,1,3,4));
			mt.RaiseElapsed();

			Now.SetNow(new DateTime(2005,1,21,1,3,5));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t2.Times);

			Now.SetNow(new DateTime(2005,1,21,1,3,6));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t2.Times);

			Now.SetNow(new DateTime(2005,1,21,1,3,7));
			mt.RaiseElapsed();
			Assert.AreEqual(1, t2.Times);


			Assert.AreEqual(1, t1.Times);
		}
	}
}
