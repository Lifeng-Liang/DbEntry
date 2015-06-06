using System.Xml.Schema;
using System.Xml.Serialization;
using Leafing.Core;
using Leafing.Data.Common;
using Leafing.Data.Definition;
using Leafing.Data.Model.Member;

namespace Leafing.Extra
{
    public abstract class XmlSerializableDbObjectModel<T>
        : XmlSerializableDbObjectModel<T, long>
         where T : XmlSerializableDbObjectModel<T, long>, new()
    {
    }

    public abstract class XmlSerializableDbObjectModel<T, TKey>
        : DbObjectModel<T, TKey>, IXmlSerializable
        where T : DbObjectModel<T, TKey>, new()
        where TKey : struct
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
			var xs = new XmlSchema ();
			xs.Id = ModelContext.Info.HandleType.Name;

			var el = new XmlSchemaElement { Name = xs.Id };
            var xct = new XmlSchemaComplexType();
            var xss = new XmlSchemaSequence();
            foreach (MemberHandler mh in ModelContext.Info.SimpleMembers)
            {
                var xe = new XmlSchemaElement
                {
                    Name = mh.MemberInfo.Name,
                    SchemaType = XmlSchemaTypeParser.GetSchemaType(mh.MemberType),
                };
                xss.Items.Add(xe);
            }
            xct.Particle = xss;
            el.SchemaType = xct;
            xs.Items.Add(el);
            return xs;
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();
            foreach (MemberHandler mh in ModelContext.Info.SimpleMembers)
            {
                var ns = reader.ReadElementString(mh.MemberInfo.Name);
                object o = ClassHelper.ChangeType(ns, mh.MemberType);
                mh.SetValue(this, o);
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (MemberHandler mh in ModelContext.Info.SimpleMembers)
            {
                object o = mh.GetValue(this);
                if (o != null)
                {
                    writer.WriteElementString(mh.MemberInfo.Name, o.ToString());
                }
            }
        }
    }
}
