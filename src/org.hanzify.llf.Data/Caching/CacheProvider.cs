using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Data.Caching
{
    public abstract class CacheProvider
    {
        public static readonly CacheProvider Instance = (CacheProvider)ClassHelper.CreateInstance(DataSetting.CacheProvider);

        public abstract object this[string Key] { get; set; }
        public abstract void Remove(string Key);
        public abstract void Clear();
    }
}
