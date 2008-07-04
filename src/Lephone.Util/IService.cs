namespace Lephone.Util
{
	public interface IService
	{
		string ServiceName { get; }
		void Start();
		void Stop();
	}
}
