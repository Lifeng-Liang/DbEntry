using System;

namespace Lephone.Core.TimingTask
{
	public interface ITiming
	{
		bool TimesUp();

		TimeSpan TimeSpanFromNowOn();

		void Reset();
	}
}
