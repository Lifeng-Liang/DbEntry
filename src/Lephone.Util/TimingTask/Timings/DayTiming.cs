namespace Lephone.Util.TimingTask.Timings
{
	public class DayTiming : DayOfRangeTimingBase
	{
        public DayTiming(TimeOfDayStructure TimeOfDay) : base(TimeOfDay, 0, MiscProvider.Instance) { }

		public DayTiming(TimeOfDayStructure TimeOfDay, MiscProvider miscTimeProvider)
			: base(TimeOfDay, 0, miscTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return true;
		}

	}
}
