using System.Collections;
using Leafing.Core.Ioc;
using Leafing.Data.Common;
using Leafing.Core;

namespace Leafing.Data.Caching
{
    [Implementation(1)]
    public class StaticHashCacheProvider : CacheProvider
    {
        protected static Hashtable Pool = new Hashtable(DataSettings.CacheSize);

        public override object this[string key]
        {
            get
            {
                var tv = (TimeValue)Pool[key];
                if (tv == null)
                {
                    return null;
                }
                if (Util.Now > tv.ExpiredOn)
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
                        if (Pool.Count > DataSettings.CacheSize)
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
