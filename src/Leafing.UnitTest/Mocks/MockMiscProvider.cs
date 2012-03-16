using System;
using System.Collections.Generic;
using Leafing.Core;
using Leafing.Core.Ioc;

namespace Leafing.UnitTest.Mocks
{
    [Implementation(2)]
    public class MockMiscProvider : MiscProvider
    {
        public static DateTime MockNow = DateTime.MinValue;

        public override DateTime Now
        {
            get { return MockNow; }
        }

        public static void Add(TimeSpan ts)
        {
            MockNow = MockNow.Add(ts);
        }

        public static Queue<Guid> MockGuids = new Queue<Guid>();

        public override Guid NewGuid()
        {
            if (MockGuids.Count > 0)
            {
                return MockGuids.Dequeue();
            }
            return Guid.NewGuid();
        }

        public static long MockSecends;

        public override long Secends
        {
            get { return MockSecends; }
        }

        public static int MockSystemRunningMillisecends;
        
        public override int SystemRunningMillisecends
        {
            get { return MockSystemRunningMillisecends; }
        }

        public static int SleepMilliSecends;

        public override void Sleep(int millisecends)
        {
            SleepMilliSecends += millisecends;
        }
    }
}