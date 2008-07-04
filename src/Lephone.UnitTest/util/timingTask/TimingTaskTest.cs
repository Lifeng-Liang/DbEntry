using System;
using Lephone.Util.TimingTask;
using Lephone.Util.TimingTask.Timings;
using NUnit.Framework;

namespace Lephone.UnitTest.util.timingTask
{
	[TestFixture]
	public class TimingTaskTest
	{
		[Test]
		public void TestIt()
		{
			MockTask Task = new MockTask();
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming Timing = new TimeSpanTiming(new TimeSpan(0, 5, 0), ntp);

			TimingTask tt = new TimingTask(Task, Timing);
			tt.RunIfTimingUp();
			Assert.AreEqual(true, Task.DoTaskOk);
			Task.DoTaskOk = false;

			ntp.Add(new TimeSpan(0,0,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, Task.DoTaskOk);

			ntp.Add(new TimeSpan(0,1,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, Task.DoTaskOk);

			ntp.Add(new TimeSpan(0,3,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, Task.DoTaskOk);

			ntp.Add(new TimeSpan(0,0,56));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, Task.DoTaskOk);

			ntp.Add(new TimeSpan(0,0,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(true, Task.DoTaskOk);
			Task.DoTaskOk = false;

			ntp.Add(new TimeSpan(0,0,1));
			tt.RunIfTimingUp();
			Assert.AreEqual(false, Task.DoTaskOk);

			ntp.Add(new TimeSpan(0,5,8));
			tt.RunIfTimingUp();
			Assert.AreEqual(true, Task.DoTaskOk);
		}
	}
}
