
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Data;
using Lephone.Data.Definition;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data.Common
{
    [Serializable]
	public class DbObjectList<T> : List<T>, IXmlSerializable
	{
        public DbObjectList() { }

        public DbObjectList<Tout> OfType<Tout>() where Tout : new()
        {
            if (typeof(T) == typeof(Tout))
            {
                return (DbObjectList<Tout>)((object)this);
            }
            DbObjectList<Tout> ret = new DbObjectList<Tout>();
            foreach (Tout i in (IEnumerable)this)
            {
                ret.Add(i);
            }
            return ret;
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
            Type t = typeof(T);
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            foreach (object or in this)
            {
                IXmlSerializable o = or as IXmlSerializable;
                writer.WriteStartElement(t.Name);
                o.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public DataTable ToDataTable()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            DataTable dt = new DataTable(oi.From.GetMainTableName());
            foreach (MemberHandler m in oi.SimpleFields)
            {
                DataColumn dc;
                if (m.FieldType.IsGenericType)
                {
                    dc = new DataColumn(m.Name, m.FieldType.GetGenericArguments()[0]);
                }
                else
                {
                    dc = new DataColumn(m.Name, m.FieldType);
                }
                if (m.AllowNull)
                {
                    dc.AllowDBNull = true;
                }
                dt.Columns.Add(dc);
            }
            foreach (object o in this)
            {
                DataRow dr = dt.NewRow();
                foreach (MemberHandler m in oi.SimpleFields)
                {
                    object ov = m.GetValue(o);
                    if (ov == null)
                    {
                        dr[m.Name] = DBNull.Value;
                    }
                    else
                    {
                        if (m.FieldType.IsGenericType)
                        {
                            dr[m.Name] = m.FieldType.GetMethod("get_Value").Invoke(ov, null);
                        }
                        else
                        {
                            dr[m.Name] = ov;
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
