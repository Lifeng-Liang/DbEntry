
using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class WeekTiming : DayOfRangeTimingBase
	{
		public WeekTiming(TimeOfDayStructure TimeOfDay, DayOfWeek dow) : base(TimeOfDay, (int)dow) {}

		public WeekTiming(TimeOfDayStructure TimeOfDay, DayOfWeek dow, NowProvider NowTimeProvider)
			: base(TimeOfDay, (int)dow, NowTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return ((int)NowTimeProvider.Now.DayOfWeek == DayOfRange);
		}
	}
}
