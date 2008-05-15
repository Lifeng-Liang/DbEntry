namespace Lephone.Util.TimingTask.Timings
{
	public class DayTiming : DayOfRangeTimingBase
	{
        public DayTiming(TimeOfDayStructure TimeOfDay) : base(TimeOfDay, 0, NowProvider.Instance) { }

		public DayTiming(TimeOfDayStructure TimeOfDay, NowProvider NowTimeProvider)
			: base(TimeOfDay, 0, NowTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return true;
		}

	}
}
