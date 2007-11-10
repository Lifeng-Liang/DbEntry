
#region usings

using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Collections.Generic;
using Lephone.Data.Common;
using Lephone.Data.QuerySyntax;

#endregion

namespace Lephone.Data.Definition
{
    [Serializable]
    [XmlRoot("DbObject")]
    public class DbObjectSmartUpdate : DbObjectBase, IXmlSerializable
    {
        [Exclude]
        internal protected Dictionary<string, object> m_UpdateColumns = null;

        [Exclude]
        internal bool m_InternalInit = false;

        protected void m_InitUpdateColumns()
        {
            m_UpdateColumns = new Dictionary<string, object>();
        }

        protected internal void m_ColumnUpdated(string ColumnName)
        {
            if (m_UpdateColumns != null && !m_InternalInit)
            {
                m_UpdateColumns[ColumnName] = 1;
            }
        }

        public XmlSchema GetSchema()
        {
            //ObjectInfo oi = ObjectInfo.GetInstance(this.GetType());
            //XmlSchema xs = new XmlSchema();
            //XmlSchemaComplexType xct = new XmlSchemaComplexType();
            //xct.Name = "DbObject";
            //XmlSchemaSequence xss = new XmlSchemaSequence();
            //foreach (MemberHandler mh in oi.SimpleFields)
            //{
            //    XmlSchemaElement xe = new XmlSchemaElement();
            //    xe.Name = mh.MemberInfo.Name;
            //    xe.ElementSchemaType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Int);
            //    xss.Items.Add(xe);
            //}
            //xct.ContentModel = xss;
            //xs.Items.Add(xct);
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(this.GetType());
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
            ValidateHandler v = new ValidateHandler();
            v.ValidateObject(this);
            return v;
        }

        public virtual bool IsValid()
        {
            return Validate().IsValid;
        }
    }
}
