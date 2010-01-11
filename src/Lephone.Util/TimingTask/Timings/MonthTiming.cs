namespace Lephone.Util.TimingTask.Timings
{
	public class MonthTiming : DayOfRangeTimingBase
	{
		public MonthTiming(TimeOfDayStructure timeOfDay, int dayOfMonth) : base(timeOfDay, dayOfMonth) {}

		public MonthTiming(TimeOfDayStructure timeOfDay, int dayOfMonth, MiscProvider miscTimeProvider)
			: base(timeOfDay, dayOfMonth, miscTimeProvider) {}

		protected override bool IsDayOfRange()
		{
			return (miscTimeProvider.Now.Day == DayOfRange);
		}
	}
}
