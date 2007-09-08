
#region usings

using System;
using NUnit.Framework;
using Lephone.Util.TimingTask;
using Lephone.Util.TimingTask.Timings;

#endregion

namespace Lephone.UnitTest.util.timingTask
{
	[TestFixture]
	public class DayTimingTest
	{
		[Test]
		public void TestIt()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3), ntp);

			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(1,2,3));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,3,6,1,0,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(2,2,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(2,6,9));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(1,51,49));
			Assert.AreEqual(false, t.TimesUp());

			ntp.SetNow(new DateTime(2004,3,6,7,12,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(true, t.TimesUp());

			t.Reset();
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());
		}

		[Test]
		public void TestIt2()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3), ntp);

			ntp.SetNow(new DateTime(2004,3,6,7,12,2));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(true, t.TimesUp());

			t.Reset();
			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());
		}

		[Test]
		public void TestOverTimingPoint()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,3,5,7,12,2,0));
			ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3), ntp);

			Assert.AreEqual(false, t.TimesUp());
			
			ntp.Add(new TimeSpan(0,0,10));
			Assert.AreEqual(true, t.TimesUp());
		}

		[Test]
		public void TestAtTimingPoint()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,3,5,7,12,5,0));
			ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3), ntp);

			Assert.AreEqual(false, t.TimesUp());
			
			ntp.Add(new TimeSpan(0,0,10));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,50));
			Assert.AreEqual(false, t.TimesUp());
		}

		[Test]
		public void TestOneDayOnce()
		{
			MockNowTimeProvider ntp = new MockNowTimeProvider(new DateTime(2004,3,5,7,10,2,0));
			ITiming t = new DayTiming(new TimeOfDayStructure(7, 12, 3), ntp);

			Assert.AreEqual(false, t.TimesUp());
			
			ntp.SetNow(new DateTime(2004,3,5,7,12,2,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,10));
			Assert.AreEqual(true, t.TimesUp());

			t.Reset();
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,10));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,20));
			Assert.AreEqual(false, t.TimesUp());
		}
	}
}
