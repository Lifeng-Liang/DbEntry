
using System;

namespace org.hanzify.llf.util.TimingTask
{
	public interface ITiming
	{
		bool TimesUp();

		TimeSpan TimeSpanFromNowOn();

		void Reset();
	}
}
