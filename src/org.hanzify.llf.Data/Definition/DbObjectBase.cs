
#region usings

using System;
using System.Xml.Serialization;
using System.Text;
using Lephone.Data.Common;
using Lephone.Util;

#endregion

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectBase : IRenew, IXmlSerializable
    {
        public DbObjectBase()
        {
        }

        void IRenew.SetAsNew()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(this.GetType());
            if (oi.HasOnePremarykey)
            {
                oi.KeyFields[0].SetValue(this, oi.KeyFields[0].UnsavedValue);
            }
            if (oi.IsAssociateObject)
            {
                foreach (MemberHandler f in oi.RelationFields)
                {
                    if (f.IsHasOne || f.IsHasMany)
                    {
                        object obj = f.GetValue(this);
                        if (obj == null)
                        {
                            IRenew ll = (IRenew)obj;
                            ll.SetAsNew();
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(this.GetType());
            StringBuilder sb = new StringBuilder("{ ");
            foreach (MemberHandler m in oi.Fields)
            {
                if (!(m.IsHasMany || m.IsHasAndBelongsToMany || m.IsHasOne))
                {
                    sb.Append(m.Name).Append(" = ");
                    object o = m.GetValue(this);
                    if (m.IsBelongsTo)
                    {
                        o = (o as IBelongsTo).ForeignKey;
                    }
                    sb.Append(o == null ? "<NULL>" : o.ToString());
                    sb.Append(", ");
                }
            }
            if (sb.Length > 2)
            {
                sb.Length -= 2;
            }
            sb.Append(" }");
            return sb.ToString();
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(this.GetType());
            foreach (MemberHandler mh in oi.SimpleFields)
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
