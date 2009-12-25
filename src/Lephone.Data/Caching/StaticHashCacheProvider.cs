using System.Collections;
using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Data.Caching
{
    public class StaticHashCacheProvider : CacheProvider
    {
        protected static Hashtable Pool = new Hashtable(DataSetting.CacheSize);

        protected internal StaticHashCacheProvider() { }

        public override object this[string key]
        {
            get
            {
                var tv = (TimeValue)Pool[key];
                if (tv == null)
                {
                    return null;
                }
                if (MiscProvider.Instance.Now > tv.ExpiredOn)
                {
                    Remove(key);
                    return null;
                }
                return tv.Value;
            }
            set
            {
                if (value == null)
                {
                    Remove(key);
                }
                else
                {
                    TimeValue tv = TimeValue.CreateTimeValue(value);
                    lock (Pool.SyncRoot)
                    {
                        if (Pool.Count > DataSetting.CacheSize)
                        {
                            Pool.Clear();
                        }

                        Pool[key] = tv;
                    }
                }
            }
        }

        public override void Remove(string key)
        {
            lock (Pool.SyncRoot)
            {
                Pool.Remove(key);
            }
        }

        public override void Clear()
        {
            lock (Pool.SyncRoot)
            {
                Pool.Clear();
            }
        }

        public override int Count
        {
            get { return Pool.Count; }
        }
    }
}
