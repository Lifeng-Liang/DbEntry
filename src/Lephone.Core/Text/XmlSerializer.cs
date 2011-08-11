using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Lephone.Core.Text
{
    public class XmlSerializer<T>
    {
        public static readonly XmlSerializer<T> Xml = new XmlSerializer<T>();

        private readonly string _rootName;

        public XmlSerializer() {}

        public XmlSerializer(string rootName)
        {
            this._rootName = rootName;
        }

        private XmlRootAttribute GetXmlRootAttribute()
        {
            Type t = typeof(T);
            var xt = t.GetAttribute<XmlTypeAttribute>(false);
            if (xt == null)
            {
                var xr = t.GetAttribute<XmlRootAttribute>(false);
                if (xr == null)
                {
                    string rn = (string.IsNullOrEmpty(_rootName)) ? t.Name : _rootName;
                    xr = new XmlRootAttribute(rn);
                }
                return xr;
            }
            return new XmlRootAttribute(xt.TypeName);
        }

        public virtual string Serialize(T obj)
        {
            Type t = typeof(T);
            XmlRootAttribute xr = GetXmlRootAttribute();
            var ois = new XmlSerializer(t, xr);
            using (var ms = new MemoryStream())
            {
                ois.Serialize(ms, obj);
                byte[] bs = ms.ToArray();
                string s = Encoding.UTF8.GetString(bs);
                return s;
            }
        }

        public virtual T Deserialize(string source)
        {
            Type t = typeof(T);
            XmlRootAttribute xr = GetXmlRootAttribute();
            var ois = new XmlSerializer(t, xr);
            using (var ms = new MemoryStream())
            {
                byte[] bs = Encoding.UTF8.GetBytes(source);
                ms.Write(bs, 0, bs.Length);
                ms.Position = 0;
                return (T)ois.Deserialize(ms);
            }
        }
    }
}
