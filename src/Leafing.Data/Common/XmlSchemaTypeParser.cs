using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace Leafing.Data.Common {
    public static class XmlSchemaTypeParser {
        private static Dictionary<Type, XmlSchemaType> _jar;

        static XmlSchemaTypeParser() {
            _jar = new Dictionary<Type, XmlSchemaType>
                       {
                           {typeof(int), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Int)},
                           {typeof(long), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Long)},
                           {typeof(string), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String)},
                           {typeof(DateTime), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.DateTime)},
                           {typeof(Date), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Date)},
                           {typeof(Time), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Time)},
                           {typeof(bool), XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean)},
                       };
        }

        public static XmlSchemaType GetSchemaType(Type type) {
            return _jar[type];
        }
    }
}
