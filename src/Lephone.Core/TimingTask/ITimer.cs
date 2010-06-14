﻿using System.Timers;

namespace Lephone.Core.TimingTask
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
