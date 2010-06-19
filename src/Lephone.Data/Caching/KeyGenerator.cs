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
                ObjectInfo oi = ObjectInfo.GetInstance(obj.GetType());
                return GetKey(oi.HandleType, oi.Handler.GetKeyValue(obj));
            }
        }

        public virtual string GetKey(Type t, object dbKey)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            string s = string.Format("{0},{1}", oi.HandleType.FullName, dbKey);
            return s;
        }
    }

    public class FullKeyGenerator : KeyGenerator
    {
        public override string GetKey(Type t, object dbKey)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            string s = string.Format("{0},{1},{2}", oi.HandleType.Assembly.FullName, oi.HandleType.FullName, dbKey);
            return s;
        }
    }
}
