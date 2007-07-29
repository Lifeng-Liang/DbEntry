
using System;

namespace org.hanzify.llf.util.TimingTask.Timings
{
	public class WeekTiming : DayOfRangeTimingBase
	{
		public WeekTiming(TimeOfDayStructure TimeOfDay, DayOfWeek dow) : base(TimeOfDay, (int)dow) {}

		public WeekTiming(TimeOfDayStructure TimeOfDay, DayOfWeek dow, INowTimeProvider NowTimeProvider)
			: base(TimeOfDay, (int)dow, NowTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return ((int)NowTimeProvider.Now.DayOfWeek == DayOfRange);
		}
	}
}
