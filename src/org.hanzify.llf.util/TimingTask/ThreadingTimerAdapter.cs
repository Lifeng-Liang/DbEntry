
#region usings

using System;
using System.Threading;
using Lephone.Util.Logging;

#endregion

namespace Lephone.Util.TimingTask
{
	public class ThreadingTimerAdapter : ITimer
	{
		protected TimerCallback tcb;
		protected Timer mTimer;
		protected bool mEnabled = false;
		protected double mInterval;
		public event System.Timers.ElapsedEventHandler Elapsed;

		public ThreadingTimerAdapter() : this( 1000 ) {}

		public ThreadingTimerAdapter(double Interval)
		{
			tcb = new TimerCallback( Timer_Elapsed );
			mInterval = Interval;
			mTimer = new Timer(tcb, null, (long)mInterval, (long)mInterval);
		}

		private void Timer_Elapsed(object obj)
		{
			if ( Elapsed != null && mEnabled )
			{
				Elapsed(this, null);
			}
		}

		public bool Enabled
		{
			get { return mEnabled; }
			set { mEnabled = value; }
		}

		public double Interval
		{
			get
			{
				return mInterval;
			}
			set
			{
				mInterval = value;
				mTimer = new Timer(tcb, null, (long)mInterval, (long)mInterval);
			}
		}

		public void Start()
		{
			Enabled = true;
		}

		public void Stop()
		{
			Enabled = false;
		}
	}
}
