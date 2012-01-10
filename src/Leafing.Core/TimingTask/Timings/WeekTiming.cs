using System;

namespace Leafing.Core.TimingTask.Timings
{
	public class WeekTiming : DayOfRangeTimingBase
	{
		public WeekTiming(TimeOfDayStructure timeOfDay, DayOfWeek dow)
			: base(timeOfDay, (int)dow) {}

		protected override bool IsDayOfRange()
		{
            return ((int)Util.Now.DayOfWeek == DayOfRange);
		}
	}
}
