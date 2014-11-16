using System.Configuration;
using System.Collections.Specialized;
using Leafing.Core.Ioc;

namespace Leafing.Core.Setting
{
    [DependenceEntry, Implementation("Default")]
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
