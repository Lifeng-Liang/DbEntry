
#region usings

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

#endregion

namespace org.hanzify.llf.util.Setting
{
    public class ConfigReaderProxy : ConfigReader
    {
        private static ConfigReader _Instance = new ConfigReaderProxy();

        public static ConfigReader Instance
        {
            get { return _Instance; }
        }

        protected void SetInstance(ConfigReader r)
        {
            _Instance = r;
            new ConfigHelper();
        }

        public override NameValueCollection GetSection(string SectionName)
        {
            NameValueCollection nvc = base.GetSection(SectionName);
            if (nvc.Count == 0)
            {
                nvc = (new ResourceConfigReader()).GetSection(SectionName);
            }
            return nvc;
        }
    }
}
