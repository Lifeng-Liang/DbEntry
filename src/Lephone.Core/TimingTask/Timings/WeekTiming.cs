using System;

namespace Lephone.Core.TimingTask.Timings
{
	public class WeekTiming : DayOfRangeTimingBase
	{
		public WeekTiming(TimeOfDayStructure timeOfDay, DayOfWeek dow)
			: base(timeOfDay, (int)dow) {}

		protected override bool IsDayOfRange()
		{
            return ((int)MiscProvider.Instance.Now.DayOfWeek == DayOfRange);
		}
	}
}
