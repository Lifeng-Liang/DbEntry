using System;
using System.Timers;
using Lephone.Util.Logging;

namespace Lephone.Util.TimingTask
{
	public class TimingTaskController : IDisposable
	{
		private readonly ITimer _tCheck;
		private readonly TimingTaskCollection _tasks;
		private bool _starting;

		public TimingTaskController(TimingTaskCollection tasks)
			: this(tasks, new ThreadingTimerAdapter(1000)) {}

		public TimingTaskController(TimingTaskCollection tasks, ITimer it)
		{
			_starting = false;
			this._tasks = tasks;
			_tCheck = it;
		}

		private void tCheck_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach ( TimingTask t in _tasks )
			{
				try
				{
					t.RunIfTimingUp();
				}
				catch ( Exception ex )
				{
					Logger.Default.Error(ex);
				}
			}
		}

		public void Start()
		{
			if ( !_starting )
			{
				_starting = true;
				_tCheck.Elapsed += tCheck_Elapsed;
				_tCheck.Start();
			}
		}

		public void Dispose()
		{
			if ( _starting )
			{
				_starting = false;
				_tCheck.Elapsed -= tCheck_Elapsed;
				_tCheck.Stop();
			}
		}
	}
}
