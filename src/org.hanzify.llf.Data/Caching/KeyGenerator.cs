
using System;
using Lephone.Util;
using Lephone.Data.Common;

namespace Lephone.Data.Caching
{
    public class KeyGenerator
    {
        public static readonly KeyGenerator Instance = (KeyGenerator)ClassHelper.CreateInstance(DataSetting.CacheKeyGenerator);

        public string this[object obj]
        {
            get
            {
                ObjectInfo oi = ObjectInfo.GetInstance(obj.GetType());
                return GetKey(oi.BaseType, oi.Handler.GetKeyValue(obj));
            }
        }

        public virtual string GetKey(Type t, object DbKey)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            string s = string.Format("{0},{1}", oi.BaseType.FullName, DbKey);
            return s;
        }
    }

    public class FullKeyGenerator : KeyGenerator
    {
        public override string GetKey(Type t, object DbKey)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            string s = string.Format("{0},{1},{2}", oi.BaseType.Assembly.FullName, oi.BaseType.FullName, DbKey);
            return s;
        }
    }
}
