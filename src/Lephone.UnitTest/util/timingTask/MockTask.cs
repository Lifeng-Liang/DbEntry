using Lephone.Util.TimingTask;

namespace Lephone.UnitTest.util.timingTask
{
	public class MockTask : ITask
	{
		public int Times;

	    public bool DoTaskOk { get; set; }

	    public void Run()
		{
			Times ++;
			DoTaskOk = true;
		}
	}
}
