
using System;

namespace org.hanzify.llf.util.TimingTask
{
	public class TimingTask
	{
		private ITask _Task;
		private ITiming _Timing;

		public ITask Task
		{
			set
			{
				_Task = value;
			}
			get
			{
				return _Task;
			}
		}

		public ITiming Timing
		{
			set 
			{
				_Timing = value;
			}
			get
			{
				return _Timing;
			}
		}

		public TimingTask(ITask Task, ITiming Timing)
		{
			_Task = Task;
			_Timing = Timing;
		}

		public void RunIfTimingUp()
		{
			if ( _Timing.TimesUp() )
			{
				_Timing.Reset();
				_Task.Run();
			}
		}
	}
}
