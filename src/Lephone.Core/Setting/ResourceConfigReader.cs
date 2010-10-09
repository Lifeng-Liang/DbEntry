using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Lephone.Core.Setting
{
    public class ResourceConfigReader : ConfigReader
    {
        private const string ConfigFilePostFix = ".config.xml";
        private Dictionary<string, NameValueCollection> _xmlConfigs;

        public override NameValueCollection GetSection(string sectionName)
        {
            if (_xmlConfigs == null)
            {
                _xmlConfigs = new Dictionary<string, NameValueCollection>();
                InitAllXmlConfigFiles();
            }
            if (_xmlConfigs.ContainsKey(sectionName))
            {
                return _xmlConfigs[sectionName];
            }
            return new NameValueCollection();
        }

        private void InitAllXmlConfigFiles()
        {
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in ass)
            {
                if (!a.GlobalAssemblyCache)
                {
                    foreach (string s in GetManifestResourceNames(a))
                    {
                        if (s.EndsWith(ConfigFilePostFix))
                        {
                            string xml = ResourceHelper.ReadToEnd(a, s, false);
                            ParseConfig(xml);
                            return;
                        }
                    }
                }
            }
        }

        private static string[] GetManifestResourceNames(Assembly a)
        {
            try
            {
                return a.GetManifestResourceNames();
            }
            catch(Exception)
            {
                return new string[] { };
            }
        }

        private void ParseConfig(string s)
        {
            var h = new NameValueSectionHandler();
            using (var ms = new MemoryStream())
            {
                byte[] bs = Encoding.Default.GetBytes(s);
                ms.Write(bs, 0, bs.Length);
                ms.Flush();
                ms.Position = 0;
                var xd = new XmlDocument();
                xd.Load(ms);

                XmlElement node  = xd["configuration"];
                if (node == null)
                {
                    throw new SettingException("configuration section not found.");
                }

                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name != "configSections")
                    {
                        var l = (NameValueCollection)h.Create(null, null, n);
                        lock (_xmlConfigs)
                        {
                            _xmlConfigs[n.Name] = l;
                        }
                    }
                }
            }
        }
    }
}
