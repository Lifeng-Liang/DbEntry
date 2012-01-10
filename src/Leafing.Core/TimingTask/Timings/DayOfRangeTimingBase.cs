using System;

namespace Leafing.Core.TimingTask.Timings
{
	public abstract class DayOfRangeTimingBase : ITiming
	{
		protected TimeSpan TimeOfDay;
		protected int DayOfRange;
		protected DateTime LastCheckTime = DateTime.Now;

	    protected DayOfRangeTimingBase(TimeOfDayStructure timeOfDay, int dayOfRange)
		{
			this.TimeOfDay = timeOfDay.TimeSpanFromMidNight;
			this.DayOfRange = dayOfRange;
		}

		public bool TimesUp()
		{
			bool bRet = false;
			try
			{
				long ts = TimeSpanFromNowOn().Ticks;
				// using recode last check stat
				bRet = ( ts <= 0 && LastCheckTime < Util.Now.Date.Add(TimeOfDay) );
			}
			catch ( ArgumentException ) {}

            LastCheckTime = Util.Now;

			return bRet;
		}

		// return today TimeSpan only.
		public TimeSpan TimeSpanFromNowOn()
		{
            DateTime itsDate = Util.Now.Date;
			if ( IsDayOfRange() )
			{
				DateTime dt = itsDate.Add(TimeOfDay);
                TimeSpan ts = dt.Subtract(Util.Now);
				return ts;
			}
			// return new TimeSpan(1, 0, 0, 0);
			throw new ArgumentException("Not today");
		}

		public void Reset() {}

		protected abstract bool IsDayOfRange();

	}
}
