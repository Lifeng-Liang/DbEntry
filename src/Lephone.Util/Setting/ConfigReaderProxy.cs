using System.Collections.Specialized;

namespace Lephone.Util.Setting
{
    public class ConfigReaderProxy : ConfigReader
    {
        private static ConfigReader instance = new ConfigReaderProxy();

        public static ConfigReader Instance
        {
            get { return instance; }
        }

        protected static void SetInstance(ConfigReader r)
        {
            instance = r;
            new ConfigHelper();
        }

        public override NameValueCollection GetSection(string sectionName)
        {
            NameValueCollection nvc = base.GetSection(sectionName);
            if (nvc.Count == 0)
            {
                nvc = (new ResourceConfigReader()).GetSection(sectionName);
            }
            return nvc;
        }
    }
}
