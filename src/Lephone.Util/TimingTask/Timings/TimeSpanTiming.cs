using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class TimeSpanTiming : ITiming
	{
		protected int SpanSeconds;
		protected DateTime LastActiveTime;
		protected MiscProvider MiscTimeProvider;

        public TimeSpanTiming(TimeSpan span) : this(span, MiscProvider.Instance) { }

		public TimeSpanTiming(TimeSpan span, MiscProvider miscTimeProvider)
            : this(span, true, miscTimeProvider)
		{
		}

        public TimeSpanTiming(TimeSpan span, bool startImmediately)
            : this(span, startImmediately, MiscProvider.Instance)
        {
        }

        public TimeSpanTiming(TimeSpan span, bool startImmediately, MiscProvider miscTimeProvider)
        {
            this.SpanSeconds = (int)span.TotalSeconds;
            this.MiscTimeProvider = miscTimeProvider;
            LastActiveTime = startImmediately ? DateTime.MinValue : MiscTimeProvider.Now;
        }

        public virtual bool TimesUp()
		{
			var ts = (int)TimeSpanFromNowOn().TotalSeconds;
			return ( ts <= 0 );
		}

		public virtual TimeSpan TimeSpanFromNowOn()
		{
			var dt = GetNextActiveTime();
            var ts = dt.Subtract(MiscTimeProvider.Now);
			return ts;
		}

		public virtual void Reset()
		{
            LastActiveTime = MiscTimeProvider.Now;
		}

		protected DateTime GetNextActiveTime()
		{
			return LastActiveTime.AddSeconds(SpanSeconds);
		}
	}
}
