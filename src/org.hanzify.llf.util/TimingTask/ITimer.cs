
#region usings

using System;
using System.Timers;

#endregion

namespace org.hanzify.llf.util.TimingTask
{
	public interface ITimer
	{
		event ElapsedEventHandler Elapsed;
		bool Enabled { set; get; }
		double Interval { set; get; }
		void Start();
		void Stop();
	}
}
