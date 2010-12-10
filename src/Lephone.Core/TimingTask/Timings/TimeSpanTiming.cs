﻿using System;

namespace Lephone.Core.TimingTask.Timings
{
	public class TimeSpanTiming : ITiming
	{
		protected int SpanSeconds;
		protected DateTime LastActiveTime;

		public TimeSpanTiming(TimeSpan span)
            : this(span, true)
		{
		}

        public TimeSpanTiming(TimeSpan span, bool startImmediately)
        {
            this.SpanSeconds = (int)span.TotalSeconds;
            LastActiveTime = startImmediately ? DateTime.MinValue : MiscProvider.Instance.Now;
        }

        public virtual bool TimesUp()
		{
			var ts = (int)TimeSpanFromNowOn().TotalSeconds;
			return ( ts <= 0 );
		}

		public virtual TimeSpan TimeSpanFromNowOn()
		{
			var dt = GetNextActiveTime();
            var ts = dt.Subtract(MiscProvider.Instance.Now);
			return ts;
		}

		public virtual void Reset()
		{
            LastActiveTime = MiscProvider.Instance.Now;
		}

		protected DateTime GetNextActiveTime()
		{
			return LastActiveTime.AddSeconds(SpanSeconds);
		}
	}
}
