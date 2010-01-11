namespace Lephone.Util.TimingTask
{
	public class TimingTask
	{
		private ITask _task;
		private ITiming _timing;

		public ITask Task
		{
			set
			{
				_task = value;
			}
			get
			{
				return _task;
			}
		}

		public ITiming Timing
		{
			set 
			{
				_timing = value;
			}
			get
			{
				return _timing;
			}
		}

		public TimingTask(ITask task, ITiming timing)
		{
			_task = task;
			_timing = timing;
		}

		public void RunIfTimingUp()
		{
			if ( _timing.TimesUp() )
			{
				_timing.Reset();
				_task.Run();
			}
		}
	}
}
