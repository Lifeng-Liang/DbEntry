using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class WeekTiming : DayOfRangeTimingBase
	{
		public WeekTiming(TimeOfDayStructure timeOfDay, DayOfWeek dow) : base(timeOfDay, (int)dow) {}

		public WeekTiming(TimeOfDayStructure timeOfDay, DayOfWeek dow, MiscProvider miscTimeProvider)
			: base(timeOfDay, (int)dow, miscTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return ((int)miscTimeProvider.Now.DayOfWeek == DayOfRange);
		}
	}
}
