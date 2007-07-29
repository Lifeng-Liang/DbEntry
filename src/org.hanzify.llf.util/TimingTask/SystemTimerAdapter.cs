
#region usings

using System;
using System.Timers;

#endregion

namespace org.hanzify.llf.util.TimingTask
{
	internal class SystemTimerAdapter : ITimer
	{
		private Timer _Timer;
		public event System.Timers.ElapsedEventHandler Elapsed;

		public SystemTimerAdapter()
		{
			_Timer = new Timer();
			Init();
		}

		public SystemTimerAdapter(double Interval)
		{
			_Timer = new Timer(Interval);
			Init();
		}

		private void Init()
		{
			_Timer.Elapsed += new ElapsedEventHandler(_Timer_Elapsed);
		}

		private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if ( Elapsed != null )
			{
				Elapsed(sender, e);
			}
		}

		private void _Timer_Elapsed()
		{
		}

		public bool Enabled
		{
			get
			{
				return _Timer.Enabled;
			}
			set
			{
				_Timer.Enabled = value;
			}
		}

		public double Interval
		{
			get
			{
				return _Timer.Interval;
			}
			set
			{
				_Timer.Interval = value;
			}
		}

		public void Start()
		{
			_Timer.Start();
		}

		public void Stop()
		{
			_Timer.Stop();
		}
	}
}
