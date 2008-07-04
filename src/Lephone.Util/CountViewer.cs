using Lephone.Util.Logging;

namespace Lephone.Util
{
	public class CountViewer
	{
		private readonly string Teamplate;
		private readonly int OutputCount;
		private readonly int nMax;
		private int n;

		public CountViewer(string Teamplate, int OutputCount, int nMax)
		{
			this.Teamplate = Teamplate;
			this.OutputCount = OutputCount;
			this.nMax = nMax;
			n = 0;
		}

		public void Output()
		{
			n++;
			if ( (n % OutputCount) == 0 || n == nMax )
			{
				Logger.Default.Info(string.Format(Teamplate, n, nMax));
			}
		}
	}
}
