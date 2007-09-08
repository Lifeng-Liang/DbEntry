
#region usings

using System;
using System.Timers;
using Lephone.Util.Logging;

#endregion

namespace Lephone.Util.TimingTask
{
	public class TimingTaskController : IDisposable
	{
		private ITimer tCheck;
		private TimingTaskCollection Tasks;
		private bool Starting;

		public TimingTaskController(TimingTaskCollection Tasks)
			: this(Tasks, new ThreadingTimerAdapter(1000)) {}

		public TimingTaskController(TimingTaskCollection Tasks, ITimer it)
		{
			Starting = false;
			this.Tasks = Tasks;
			tCheck = it;
		}

		private void tCheck_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach ( TimingTask t in Tasks )
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
			if ( !Starting )
			{
				Starting = true;
				tCheck.Elapsed += new ElapsedEventHandler(tCheck_Elapsed);
				tCheck.Start();
			}
		}

		public void Dispose()
		{
			if ( Starting )
			{
				Starting = false;
				tCheck.Elapsed -= new ElapsedEventHandler(tCheck_Elapsed);
				tCheck.Stop();
			}
		}
	}
}
