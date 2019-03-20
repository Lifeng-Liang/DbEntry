using Leafing.Core.Ioc;

namespace Leafing.Data.Caching {
    [DependenceEntry]
    public abstract class CacheProvider {
        public static readonly CacheProvider Instance = SimpleContainer.Get<CacheProvider>();

        public abstract object this[string key] { get; set; }
        public abstract void Remove(string key);
        public abstract void Clear();
        public abstract int Count { get; }
    }
}