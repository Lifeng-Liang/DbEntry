using Lephone.Util.Logging;

namespace Lephone.Util
{
	public class CountViewer
	{
		private readonly string _teamplate;
		private readonly int _outputCount;
		private readonly int _nMax;
		private int _n;

		public CountViewer(string teamplate, int outputCount, int nMax)
		{
			this._teamplate = teamplate;
			this._outputCount = outputCount;
			this._nMax = nMax;
			_n = 0;
		}

		public void Output()
		{
			_n++;
			if ( (_n % _outputCount) == 0 || _n == _nMax )
			{
				Logger.Default.Info(string.Format(_teamplate, _n, _nMax));
			}
		}
	}
}
