using System;
using Leafing.Core.TimingTask;
using Leafing.Core.TimingTask.Timings;
using Leafing.UnitTest.Mocks;
using NUnit.Framework;

namespace Leafing.UnitTest.util.timingTask
{
	[TestFixture]
	public class TimingTaskTest
	{
		[Test]
		public void TestIt()
		{
			var task = new MockTask();
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 10, 5, 10, 0));
			ITiming timing = new TimeSpanTiming(new TimeSpan(0, 5, 0));

			var tt = new TimingTask(task, timing);
			tt.RunIfTimingUp();
			Assert.AreEqual(true, task.DoTaskOk);
			task.DoTaskOk = false;

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

            MockMiscProvider.Add(new TimeSpan(0, 1, 1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

            MockMiscProvider.Add(new TimeSpan(0, 3, 1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

            MockMiscProvider.Add(new TimeSpan(0, 0, 56));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
			tt.RunIfTimingUp();
			Assert.AreEqual(true, task.DoTaskOk);
			task.DoTaskOk = false;

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

            MockMiscProvider.Add(new TimeSpan(0, 5, 8));
			tt.RunIfTimingUp();
			Assert.AreEqual(true, task.DoTaskOk);
		}
	}
}
