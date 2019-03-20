using System;
using Leafing.Core.TimingTask;
using Leafing.Core.TimingTask.Timings;
using Leafing.UnitTest.Mocks;
using NUnit.Framework;

namespace Leafing.UnitTest.util.timingTask {
    [TestFixture]
    public class TimeSpanTimingTest {
        [Test]
        public void TestForSecends() {
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 10, 5, 10, 0));
            ITiming t = new TimeSpanTiming(new TimeSpan(0, 0, 3));

            Assert.AreEqual(true, t.TimesUp());
            t.Reset();

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(true, t.TimesUp());
            t.Reset();

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 4));
            Assert.AreEqual(true, t.TimesUp());
        }

        [Test]
        public void TestForSecends2() {
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 10, 5, 10, 0));
            ITiming t = new TimeSpanTiming(new TimeSpan(0, 0, 3), false);

            Assert.AreEqual(false, t.TimesUp());
            t.Reset();

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(true, t.TimesUp());
            t.Reset();

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 4));
            Assert.AreEqual(true, t.TimesUp());
        }

        [Test]
        public void TestForMinutes() {
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 10, 5, 10, 0));
            ITiming t = new TimeSpanTiming(new TimeSpan(0, 5, 0));

            Assert.AreEqual(true, t.TimesUp());
            t.Reset();

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 1, 0));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 3, 0));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 58));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(true, t.TimesUp());
            t.Reset();

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 5, 10));
            Assert.AreEqual(true, t.TimesUp());
        }

        [Test]
        public void TestForHours() {
            MockMiscProvider.MockNow = (new DateTime(2004, 3, 5, 10, 5, 10, 0));
            ITiming t = new TimeSpanTiming(new TimeSpan(6, 0, 0));

            Assert.AreEqual(true, t.TimesUp());
            t.Reset();

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 1, 0));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(1, 3, 0));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(4, 55, 58));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(true, t.TimesUp());
            t.Reset();

            MockMiscProvider.Add(new TimeSpan(0, 0, 1));
            Assert.AreEqual(false, t.TimesUp());

            MockMiscProvider.Add(new TimeSpan(6, 0, 8));
            Assert.AreEqual(true, t.TimesUp());
        }
    }
}