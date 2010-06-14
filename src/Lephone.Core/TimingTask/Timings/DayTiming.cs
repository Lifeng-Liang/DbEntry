namespace Lephone.Core.TimingTask.Timings
{
	public class DayTiming : DayOfRangeTimingBase
	{
        public DayTiming(TimeOfDayStructure timeOfDay) : base(timeOfDay, 0, MiscProvider.Instance) { }

		public DayTiming(TimeOfDayStructure timeOfDay, MiscProvider miscTimeProvider)
			: base(timeOfDay, 0, miscTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return true;
		}

	}
}
