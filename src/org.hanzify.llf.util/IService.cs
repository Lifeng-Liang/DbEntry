
using System;

namespace org.hanzify.llf.util
{
	public interface IService
	{
		string ServiceName { get; }
		void Start();
		void Stop();
	}
}
