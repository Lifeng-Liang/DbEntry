
using System;

namespace org.hanzify.llf.util.TimingTask.Timings
{
	public class TimeOfDayStructure
	{
		private TimeSpan _TimeOfDay;

		public TimeOfDayStructure(int Hour, int Minute, int Second)
		{
			_TimeOfDay = new TimeSpan(Hour, Minute, Second);
		}

		public TimeSpan TimeSpanFromMidNight
		{
			get { return _TimeOfDay; }
		}
	}
}
