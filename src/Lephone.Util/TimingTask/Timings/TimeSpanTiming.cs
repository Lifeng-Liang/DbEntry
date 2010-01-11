using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class TimeSpanTiming : ITiming
	{
		protected int SpanSeconds;
		protected DateTime LastActiveTime;
		protected MiscProvider miscTimeProvider;

        public TimeSpanTiming(TimeSpan span) : this(span, MiscProvider.Instance) { }

		public TimeSpanTiming(TimeSpan span, MiscProvider miscTimeProvider)
		{
			this.SpanSeconds = (int)span.TotalSeconds;
			this.miscTimeProvider = miscTimeProvider;
			LastActiveTime = DateTime.MinValue;
		}

		public virtual bool TimesUp()
		{
			var ts = (int)TimeSpanFromNowOn().TotalSeconds;
			return ( ts <= 0 );
		}

		public virtual TimeSpan TimeSpanFromNowOn()
		{
			var dt = GetNextActiveTime();
            var ts = dt.Subtract(miscTimeProvider.Now);
			return ts;
		}

		public virtual void Reset()
		{
            LastActiveTime = miscTimeProvider.Now;
		}

		protected DateTime GetNextActiveTime()
		{
			return LastActiveTime.AddSeconds(SpanSeconds);
		}
	}
}
