using System.Collections.Specialized;

namespace Leafing.Core.Setting
{
	public class ConfigHelper : ConfigHelperBase
	{
        public static readonly ConfigHelper AppSettings = new ConfigHelper("appSettings");
        public static readonly ConfigHelper LeafingSettings = new ConfigHelper("Leafing.Settings");

        internal ConfigHelper()
        {
            AppSettings._nvc = new ConfigHelper("appSettings")._nvc;
            LeafingSettings._nvc = new ConfigHelper("Leafing.Settings")._nvc;
        }

        private NameValueCollection _nvc;

		public ConfigHelper(string sectionName)
		{
            _nvc = ConfigReaderProxy.Instance.GetSection(sectionName);
        }

		protected override string GetString(string key)
		{
			return _nvc[key];
		}
	}
}
