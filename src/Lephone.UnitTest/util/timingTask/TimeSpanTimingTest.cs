using System;
using Lephone.Util.TimingTask;
using Lephone.Util.TimingTask.Timings;
using NUnit.Framework;

namespace Lephone.UnitTest.util.timingTask
{
	[TestFixture]
	public class TimeSpanTimingTest
	{
		[Test]
		public void TestForSecends()
		{
			var ntp = new MockMiscProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming t = new TimeSpanTiming(new TimeSpan(0, 0, 3), ntp);

			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,4));
			Assert.AreEqual(true, t.TimesUp());
		}

        [Test]
        public void TestForSecends2()
        {
            var ntp = new MockMiscProvider(new DateTime(2004, 3, 5, 10, 5, 10, 0));
            ITiming t = new TimeSpanTiming(new TimeSpan(0, 0, 3), false, ntp);

            Assert.AreEqual(false, t.TimesUp());
            t.Reset();

            ntp.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            ntp.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            ntp.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(true, t.TimesUp());
            t.Reset();

            ntp.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            ntp.Add(new TimeSpan(0, 0, 4));
            Assert.AreEqual(true, t.TimesUp());
        }

        [Test]
		public void TestForMinutes()
		{
			var ntp = new MockMiscProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming t = new TimeSpanTiming(new TimeSpan(0, 5, 0), ntp);

			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,1,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,3,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,58));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,5,10));
			Assert.AreEqual(true, t.TimesUp());
		}

		[Test]
		public void TestForHours()
		{
			var ntp = new MockMiscProvider(new DateTime(2004,3,5,10,5,10,0));
			ITiming t = new TimeSpanTiming(new TimeSpan(6, 0, 0), ntp);

			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,1,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(1,3,0));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(4,55,58));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(true, t.TimesUp());
			t.Reset();

			ntp.Add(new TimeSpan(0,0,1));
			Assert.AreEqual(false, t.TimesUp());

			ntp.Add(new TimeSpan(6,0,8));
			Assert.AreEqual(true, t.TimesUp());
		}
	}
}
