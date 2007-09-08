
#region usings

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace Lephone.Util.Text
{
    public class XmlSerializer<T> : StringSerializer<T>
    {
        private string RootName;

        public XmlSerializer() {}

        public XmlSerializer(string RootName)
        {
            this.RootName = RootName;
        }

        public override string Serialize(T obj)
        {
            Type t = typeof(T);
            string rn = (string.IsNullOrEmpty(RootName)) ? t.Name : RootName;
            XmlSerializer ois = new XmlSerializer(t, new XmlRootAttribute(rn));
            using (MemoryStream ms = new MemoryStream())
            {
                ois.Serialize(ms, obj);
                byte[] bs = ms.ToArray();
                string s = Encoding.UTF8.GetString(bs);
                return s;
            }
        }

        public override T Deserialize(string Source)
        {
            Type t = typeof(T);
            string rn = (string.IsNullOrEmpty(RootName)) ? t.Name : RootName;
            XmlSerializer ois = new XmlSerializer(t, new XmlRootAttribute(rn));
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bs = Encoding.UTF8.GetBytes(Source);
                ms.Write(bs, 0, bs.Length);
                ms.Position = 0;
                return (T)ois.Deserialize(ms);
            }
        }
    }
}
