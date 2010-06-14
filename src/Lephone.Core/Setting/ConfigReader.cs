using System.Configuration;
using System.Collections.Specialized;

namespace Lephone.Core.Setting
{
    public class ConfigReader
    {
        public virtual NameValueCollection GetSection(string sectionName)
        {
            NameValueCollection c = (NameValueCollection)ConfigurationManager.GetSection(sectionName) ??
                                    new NameValueCollection();
            return c;
        }
    }
}
