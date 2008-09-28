using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class TimeSpanTiming : ITiming
	{
		protected int SpanSeconds;
		protected DateTime LastActiveTime;
		protected MiscProvider miscTimeProvider;

        public TimeSpanTiming(TimeSpan Span) : this(Span, MiscProvider.Instance) { }

		public TimeSpanTiming(TimeSpan Span, MiscProvider miscTimeProvider)
		{
			this.SpanSeconds = (int)Span.TotalSeconds;
			this.miscTimeProvider = miscTimeProvider;
			LastActiveTime = DateTime.MinValue;
		}

		public virtual bool TimesUp()
		{
			int ts = (int)TimeSpanFromNowOn().TotalSeconds;
			return ( ts <= 0 );
		}

		public virtual TimeSpan TimeSpanFromNowOn()
		{
			DateTime dt = GetNextActiveTime();
            TimeSpan ts = dt.Subtract(miscTimeProvider.Now);
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
