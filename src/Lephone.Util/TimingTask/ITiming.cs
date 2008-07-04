using System;

namespace Lephone.Util.TimingTask
{
	public interface ITiming
	{
		bool TimesUp();

		TimeSpan TimeSpanFromNowOn();

		void Reset();
	}
}
