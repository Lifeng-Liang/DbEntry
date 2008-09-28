namespace Lephone.Util.TimingTask.Timings
{
	public class MonthTiming : DayOfRangeTimingBase
	{
		public MonthTiming(TimeOfDayStructure TimeOfDay, int DayOfMonth) : base(TimeOfDay, DayOfMonth) {}

		public MonthTiming(TimeOfDayStructure TimeOfDay, int DayOfMonth, MiscProvider miscTimeProvider)
			: base(TimeOfDay, DayOfMonth, miscTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return (miscTimeProvider.Now.Day == DayOfRange);
		}
	}
}
