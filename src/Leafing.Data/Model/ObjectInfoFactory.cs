using System;
using Leafing.Core;

namespace Leafing.Data.Model
{
    public class ObjectInfoFactory : FlyweightBase<Type, ObjectInfo>
    {
        public static readonly ObjectInfoFactory Instance = new ObjectInfoFactory();

        protected override ObjectInfo CreateInst(Type t)
        {
            return new ObjectInfo(t);
        }
    }
}

