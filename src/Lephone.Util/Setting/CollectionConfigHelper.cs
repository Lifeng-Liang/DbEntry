using System.Collections.Specialized;

namespace Lephone.Util.Setting
{
    public class CollectionConfigHelper : ConfigHelperBase
    {
        private readonly NameValueCollection _config;

        public CollectionConfigHelper(NameValueCollection config)
        {
            _config = config;
        }

        protected override string GetString(string key)
        {
            return _config[key];
        }
    }
}
