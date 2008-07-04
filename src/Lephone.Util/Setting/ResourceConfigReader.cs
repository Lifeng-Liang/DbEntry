using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Lephone.Util.Setting
{
    public class ResourceConfigReader : ConfigReader
    {
        private const string ConfigFilePostFix = ".config.xml";
        private Dictionary<string, NameValueCollection> XmlConfigs;

        public override NameValueCollection GetSection(string sectionName)
        {
            if (XmlConfigs == null)
            {
                InitAllXmlConfigFiles();
            }
            if (XmlConfigs.ContainsKey(sectionName))
            {
                return XmlConfigs[sectionName];
            }
            return new NameValueCollection();
        }

        private void InitAllXmlConfigFiles()
        {
            XmlConfigs = new Dictionary<string, NameValueCollection>();
            Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
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
            NameValueSectionHandler h = new NameValueSectionHandler();
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bs = Encoding.Default.GetBytes(s);
                ms.Write(bs, 0, bs.Length);
                ms.Flush();
                ms.Position = 0;
                XmlDocument xd = new XmlDocument();
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
                        NameValueCollection l = (NameValueCollection)h.Create(null, null, n);
                        lock (XmlConfigs)
                        {
                            XmlConfigs[n.Name] = l;
                        }
                    }
                }
            }
        }
    }
}
