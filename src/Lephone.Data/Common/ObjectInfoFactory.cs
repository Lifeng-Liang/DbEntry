using System;
using Lephone.Core;

namespace Lephone.Data.Common
{
    public class ObjectInfoFactory : FlyweightBase<Type, ObjectInfo>
    {
        protected override ObjectInfo CreateInst(Type t)
        {
            return new ObjectInfo(t);
        }

        protected override Type CheckKey(Type dbObjectType)
        {
            if (dbObjectType.IsNotPublic)
            {
                throw new DataException("The model class should be public");
            }
            var c = ClassHelper.GetArgumentlessConstructor(dbObjectType);
            if (c == null)
            {
                string typeName = dbObjectType.Name;
                throw new DataException("class {0} need a public/protected(DbObjectModel) argumentless constructor", typeName);
            }
            return dbObjectType;
        }

        internal ObjectInfoBase GetSimpleInstance(Type dbObjectType)
        {
            if (Jar.ContainsKey(dbObjectType))
            {
                return Jar[dbObjectType];
            }
            var oi = new ObjectInfoBase(dbObjectType);
            return oi;
        }
    }
}
