using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class WeekTiming : DayOfRangeTimingBase
	{
		public WeekTiming(TimeOfDayStructure TimeOfDay, DayOfWeek dow) : base(TimeOfDay, (int)dow) {}

		public WeekTiming(TimeOfDayStructure TimeOfDay, DayOfWeek dow, MiscProvider miscTimeProvider)
			: base(TimeOfDay, (int)dow, miscTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return ((int)miscTimeProvider.Now.DayOfWeek == DayOfRange);
		}
	}
}
