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
                throw new ModelException(dbObjectType, "The model class should be public.");
            }
            var c = ClassHelper.GetArgumentlessConstructor(dbObjectType);
            if (c == null)
            {
                throw new ModelException(dbObjectType, "The model need a public/protected(DbObjectModel) argumentless constructor");
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
