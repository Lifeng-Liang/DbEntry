using System;
using Lephone.Data.Common;

namespace Lephone.Data
{
    public static class DynamicObject
    {
        public static T NewObject<T>(params object[] os)
        {
            return DynamicObjectBuilder.Instance.NewObject<T>(os);
        }

        public static Type GetImplType(Type sourceType)
        {
            return DynamicObjectBuilder.Instance.GetImplType(sourceType);
        }
    }
}
