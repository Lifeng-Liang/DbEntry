
using System;

namespace org.hanzify.llf.util.TimingTask.Timings
{
	public interface INowTimeProvider
	{
		DateTime Now { get; }
	}
}
