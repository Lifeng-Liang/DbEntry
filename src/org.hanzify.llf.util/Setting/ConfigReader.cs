
#region usings

using System;
using System.Configuration;
using System.Collections.Specialized;

#endregion

namespace org.hanzify.llf.util.Setting
{
    public class ConfigReader
    {
        public virtual NameValueCollection GetSection(string SectionName)
        {
            NameValueCollection c = (NameValueCollection)ConfigurationManager.GetSection(SectionName);
            if (c == null)
            {
                c = new NameValueCollection();
            }
            return c;
        }
    }
}
