using System.Collections.Specialized;

namespace Lephone.Core.Setting
{
	public class ConfigHelper : ConfigHelperBase
	{
        public static readonly ConfigHelper AppSettings = new ConfigHelper("appSettings");
        public static readonly ConfigHelper DefaultSettings = new ConfigHelper("Lephone.Settings");

        internal ConfigHelper()
        {
            AppSettings._nvc = new ConfigHelper("appSettings")._nvc;
            DefaultSettings._nvc = new ConfigHelper("Lephone.Settings")._nvc;
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
