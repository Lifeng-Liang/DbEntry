namespace Leafing.Core.TimingTask.Timings
{
	public class MonthTiming : DayOfRangeTimingBase
	{
		public MonthTiming(TimeOfDayStructure timeOfDay, int dayOfMonth)
			: base(timeOfDay, dayOfMonth) {}

		protected override bool IsDayOfRange()
		{
            return (Util.Now.Day == DayOfRange);
		}
	}
}
