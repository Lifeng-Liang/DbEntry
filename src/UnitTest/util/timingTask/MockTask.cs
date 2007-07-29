
#region usings

using System;
using org.hanzify.llf.util.TimingTask;

#endregion

namespace org.hanzify.llf.UnitTest.util.timingTask
{
	public class MockTask : ITask
	{
		public int Times = 0;
		private bool _DoTaskOk;

		public bool DoTaskOk
		{
			get
			{
				return _DoTaskOk;
			}
			set
			{
				_DoTaskOk = value;
			}
		}

		public MockTask() {}

		public void Run()
		{
			Times ++;
			_DoTaskOk = true;
		}
	}
}
