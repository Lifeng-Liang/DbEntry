
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
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
            foreach (object or in this)
            {
                DbObjectBase o = or as DbObjectBase;
                writer.WriteStartElement(oi.From.GetMainTableName());
                o.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}
