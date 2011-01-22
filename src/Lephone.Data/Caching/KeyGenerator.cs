using System;
using Lephone.Core;
using Lephone.Data.Common;

namespace Lephone.Data.Caching
{
    public class KeyGenerator
    {
        public static readonly KeyGenerator Instance = (KeyGenerator)ClassHelper.CreateInstance(DataSettings.CacheKeyGenerator);

        public string this[object obj]
        {
            get
            {
                var type = obj.GetType();
                var ctx = ModelContext.GetInstance(type);
                return GetKey(type, ctx.Handler.GetKeyValue(obj));
            }
        }

        public virtual string GetKey(Type t, object dbKey)
        {
            string s = string.Format("{0},{1}", t.FullName, dbKey);
            return s;
        }
    }

    public class FullKeyGenerator : KeyGenerator
    {
        public override string GetKey(Type t, object dbKey)
        {
            string s = string.Format("{0},{1},{2}", t.Assembly.FullName, t.FullName, dbKey);
            return s;
        }
    }
}
