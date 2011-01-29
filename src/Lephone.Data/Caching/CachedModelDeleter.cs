using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.Model;

namespace Lephone.Data.Caching
{
    public class CachedModelDeleter : ModelDeleter
    {
        public CachedModelDeleter(IDbObject obj) : base(obj)
        {
        }

        protected override int InnerDelete()
        {
            var ret = base.InnerDelete();
            CacheProvider.Instance.Remove(KeyGenerator.Instance[Obj]);
            return ret;
        }
    }
}
