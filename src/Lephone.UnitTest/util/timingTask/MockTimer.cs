using Lephone.Util.TimingTask;

namespace Lephone.UnitTest.util.timingTask
{
	public class MockTimer : ITimer
	{
		public MockTimer() {}

		public event System.Timers.ElapsedEventHandler Elapsed;

		public bool Enabled
		{
			get { return false; }
			set {}
		}

		public double Interval
		{
			get { return 0; }
			set {}
		}

		public void Start() {}

		public void Stop() {}

		public void RaiseElapsed()
		{
			if ( Elapsed != null )
			{
				Elapsed(null, null);
			}
		}
	}
}
