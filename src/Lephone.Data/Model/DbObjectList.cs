using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Lephone.Data.Model.Member;

namespace Lephone.Data.Model
{
    [Serializable]
    public class DbObjectList<T> : List<T>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            Type t = typeof(T);
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
            var ctx = ModelContext.GetInstance(typeof(T));
            var dt = new DataTable(ctx.Info.From.MainTableName);
            foreach (MemberHandler m in ctx.Info.SimpleMembers)
            {
                DataColumn dc
                    = m.MemberType.IsGenericType
                    ? new DataColumn(m.Name, m.MemberType.GetGenericArguments()[0])
                    : new DataColumn(m.Name, m.MemberType);
                if (m.Is.AllowNull)
                {
                    dc.AllowDBNull = true;
                }
                dt.Columns.Add(dc);
            }
            foreach (object o in this)
            {
                DataRow dr = dt.NewRow();
                foreach (MemberHandler m in ctx.Info.SimpleMembers)
                {
                    object ov = m.GetValue(o);
                    if (ov == null)
                    {
                        dr[m.Name] = DBNull.Value;
                    }
                    else
                    {
                        dr[m.Name]
                            = m.MemberType.IsGenericType
                            ? m.MemberType.GetMethod("get_Value").Invoke(ov, null)
                            : ov;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}

