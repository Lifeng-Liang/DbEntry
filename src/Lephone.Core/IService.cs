namespace Lephone.Core
{
	public interface IService
	{
		string ServiceName { get; }
		void Start();
		void Stop();
	}
}
