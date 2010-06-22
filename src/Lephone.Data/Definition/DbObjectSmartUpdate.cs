using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Collections.Generic;
using Lephone.Core;
using Lephone.Data.Common;

namespace Lephone.Data.Definition
{
    [Serializable]
    [XmlRoot("DbObject")]
    public class DbObjectSmartUpdate : DbObjectBase, IXmlSerializable
    {
        [Exclude]
        internal protected Dictionary<string, object> m_UpdateColumns;

        [Exclude]
        internal bool m_InternalInit;

        internal protected void m_InitUpdateColumns()
        {
            m_UpdateColumns = new Dictionary<string, object>();
        }

        protected internal void m_ColumnUpdated(string columnName)
        {
            if (m_UpdateColumns != null && !m_InternalInit)
            {
                m_UpdateColumns[columnName] = 1;
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            var info = ObjectInfo.GetInstance(this.GetType());
            var xs = new XmlSchema();

            var el = new XmlSchemaElement {Name = info.HandleType.Name};
            var xct = new XmlSchemaComplexType();
            var xss = new XmlSchemaSequence();
            foreach (MemberHandler mh in info.SimpleFields)
            {
                var xe = new XmlSchemaElement
                             {
                                 Name = mh.MemberInfo.Name,
                                 SchemaType = XmlSchemaTypeParser.GetSchemaType(mh.FieldType),
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
            var oi = ObjectInfo.GetInstance(GetType());
            reader.ReadStartElement();
            foreach (MemberHandler mh in oi.SimpleFields)
            {
                var ns = reader.ReadElementString(mh.MemberInfo.Name);
                object o = ClassHelper.ChangeType(ns, mh.FieldType);
                mh.SetValue(this, o);
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            var oi = ObjectInfo.GetInstance(GetType());
            foreach (MemberHandler mh in oi.SimpleFields)
            {
                object o = mh.GetValue(this);
                if (o != null)
                {
                    writer.WriteElementString(mh.MemberInfo.Name, o.ToString());
                }
            }
        }

        public virtual void Save()
        {
            DbEntry.Save(this);
        }

        public virtual void Delete()
        {
            DbEntry.Delete(this);
        }

        public virtual ValidateHandler Validate()
        {
            var v = new ValidateHandler();
            v.ValidateObject(this);
            return v;
        }

        public virtual bool IsValid()
        {
            return Validate().IsValid;
        }
    }
}
