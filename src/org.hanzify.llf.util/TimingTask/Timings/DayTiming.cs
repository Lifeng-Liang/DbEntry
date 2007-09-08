
using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class DayTiming : DayOfRangeTimingBase
	{
		public DayTiming(TimeOfDayStructure TimeOfDay) : base(TimeOfDay, 0, SystemNowTimeProvider.Instance) {}

		public DayTiming(TimeOfDayStructure TimeOfDay, INowTimeProvider NowTimeProvider)
			: base(TimeOfDay, 0, NowTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return true;
		}

	}
}
