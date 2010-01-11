using System.Timers;

namespace Lephone.Util.TimingTask
{
	internal class SystemTimerAdapter : ITimer
	{
		private readonly Timer _timer;
		public event ElapsedEventHandler Elapsed;

		public SystemTimerAdapter()
		{
			_timer = new Timer();
			Init();
		}

		public SystemTimerAdapter(double interval)
		{
			_timer = new Timer(interval);
			Init();
		}

		private void Init()
		{
			_timer.Elapsed += _Timer_Elapsed;
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
				return _timer.Enabled;
			}
			set
			{
				_timer.Enabled = value;
			}
		}

		public double Interval
		{
			get
			{
				return _timer.Interval;
			}
			set
			{
				_timer.Interval = value;
			}
		}

		public void Start()
		{
			_timer.Start();
		}

		public void Stop()
		{
			_timer.Stop();
		}
	}
}
