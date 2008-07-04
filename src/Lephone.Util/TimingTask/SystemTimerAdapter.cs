using System;
using System.Timers;

namespace Lephone.Util.TimingTask
{
	internal class SystemTimerAdapter : ITimer
	{
		private readonly Timer _Timer;
		public event ElapsedEventHandler Elapsed;

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
			_Timer.Elapsed += _Timer_Elapsed;
		}

		private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if ( Elapsed != null )
			{
				Elapsed(sender, e);
			}
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
