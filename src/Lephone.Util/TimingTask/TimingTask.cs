namespace Lephone.Util.TimingTask
{
	public class TimingTask
	{
	    public ITask Task { get; set; }

	    public ITiming Timing { get; set; }

	    public TimingTask(ITask task, ITiming timing)
		{
			Task = task;
			Timing = timing;
		}

		public void RunIfTimingUp()
		{
			if ( Timing.TimesUp() )
			{
				Timing.Reset();
				Task.Run();
			}
		}
	}
}
