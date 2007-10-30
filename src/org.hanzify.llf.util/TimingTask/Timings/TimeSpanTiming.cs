
using System;

namespace Lephone.Util.TimingTask.Timings
{
	public class TimeSpanTiming : ITiming
	{
		protected int SpanSeconds;
		protected DateTime LastActiveTime;
		protected NowProvider NowTimeProvider;

        public TimeSpanTiming(TimeSpan Span) : this(Span, NowProvider.Instance) { }

		public TimeSpanTiming(TimeSpan Span, NowProvider NowTimeProvider)
		{
			this.SpanSeconds = (int)Span.TotalSeconds;
			this.NowTimeProvider = NowTimeProvider;
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
			TimeSpan ts = dt.Subtract(NowTimeProvider.Now);
			return ts;
		}

		public virtual void Reset()
		{
			LastActiveTime = NowTimeProvider.Now;
		}

		protected DateTime GetNextActiveTime()
		{
			return LastActiveTime.AddSeconds(SpanSeconds);
		}
	}
}
