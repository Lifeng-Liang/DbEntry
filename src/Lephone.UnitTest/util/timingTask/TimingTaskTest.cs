using System;
using Lephone.Core.TimingTask;
using Lephone.Core.TimingTask.Timings;
using NUnit.Framework;

namespace Lephone.UnitTest.util.timingTask
{
	[TestFixture]
	public class TimingTaskTest
	{
		[Test]
		public void TestIt()
		{
			var task = new MockTask();
			var ntp = new MockMiscProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming timing = new TimeSpanTiming(new TimeSpan(0, 5, 0), ntp);

			var tt = new TimingTask(task, timing);
			tt.RunIfTimingUp();
			Assert.AreEqual(true, task.DoTaskOk);
			task.DoTaskOk = false;

			ntp.Add(new TimeSpan(0,0,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

			ntp.Add(new TimeSpan(0,1,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

			ntp.Add(new TimeSpan(0,3,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

			ntp.Add(new TimeSpan(0,0,56));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

			ntp.Add(new TimeSpan(0,0,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(true, task.DoTaskOk);
			task.DoTaskOk = false;

			ntp.Add(new TimeSpan(0,0,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, task.DoTaskOk);

			ntp.Add(new TimeSpan(0,5,8));
			tt.RunIfTimingUp();
			Assert.AreEqual(true, task.DoTaskOk);
		}
	}
}
