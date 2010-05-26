using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Data.Caching
{
    public abstract class CacheProvider
    {
        public static readonly CacheProvider Instance = (CacheProvider)ClassHelper.CreateInstance(DataSettings.CacheProvider);

        public abstract object this[string key] { get; set; }
        public abstract void Remove(string key);
        public abstract void Clear();
        public abstract int Count { get; }
    }
}
