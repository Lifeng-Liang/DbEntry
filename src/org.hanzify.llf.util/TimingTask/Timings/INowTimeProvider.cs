
using System;

namespace Lephone.Util.TimingTask.Timings
{
	public interface INowTimeProvider
	{
		DateTime Now { get; }
	}
}
