
using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class MonthTiming : DayOfRangeTimingBase
	{
		public MonthTiming(TimeOfDayStructure TimeOfDay, int DayOfMonth) : base(TimeOfDay, DayOfMonth) {}

		public MonthTiming(TimeOfDayStructure TimeOfDay, int DayOfMonth, NowProvider NowTimeProvider)
			: base(TimeOfDay, DayOfMonth, NowTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return (NowTimeProvider.Now.Day == DayOfRange);
		}
	}
}
