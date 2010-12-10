namespace Lephone.Core.TimingTask.Timings
{
	public class DayTiming : DayOfRangeTimingBase
	{
		public DayTiming(TimeOfDayStructure timeOfDay)
			: base(timeOfDay, 0) {}

		protected override bool IsDayOfRange()
		{
			return true;
		}
	}
}
