using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Data;

namespace Lephone.Data.Common
{
    [Serializable]
	public class DbObjectList<T> : List<T>, IXmlSerializable
	{
        public DbObjectList() { }

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
            //TODO: why left this?
            //ObjectInfo Info = ObjectInfo.GetInstance(t);
            foreach (object or in this)
            {
                var o = (IXmlSerializable)or;
                writer.WriteStartElement(t.Name);
                //TODO: how to handle this may null?
                o.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public DataTable ToDataTable()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            var dt = new DataTable(oi.From.MainTableName);
            foreach (MemberHandler m in oi.SimpleFields)
            {
                DataColumn dc 
                    = m.FieldType.IsGenericType 
                    ? new DataColumn(m.Name, m.FieldType.GetGenericArguments()[0]) 
                    : new DataColumn(m.Name, m.FieldType);
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
                        dr[m.Name] 
                            = m.FieldType.IsGenericType 
                            ? m.FieldType.GetMethod("get_Value").Invoke(ov, null) 
                            : ov;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
