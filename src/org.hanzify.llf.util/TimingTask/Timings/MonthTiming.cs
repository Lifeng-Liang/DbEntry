
using System;

namespace org.hanzify.llf.util.TimingTask.Timings
{
	public class MonthTiming : DayOfRangeTimingBase
	{
		public MonthTiming(TimeOfDayStructure TimeOfDay, int DayOfMonth) : base(TimeOfDay, DayOfMonth) {}

		public MonthTiming(TimeOfDayStructure TimeOfDay, int DayOfMonth, INowTimeProvider NowTimeProvider)
			: base(TimeOfDay, DayOfMonth, NowTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return (NowTimeProvider.Now.Day == DayOfRange);
		}
	}
}
