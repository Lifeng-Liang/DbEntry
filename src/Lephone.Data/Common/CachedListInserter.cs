using System;
using System.Collections;
using Lephone.Data.Caching;

namespace Lephone.Data.Common
{
    public class CachedListInserter : IProcessor
    {
		private readonly IList list;
        private readonly ObjectInfo oi;

        public CachedListInserter(IList list, Type t)
		{
			this.list = list;
            oi = ObjectInfo.GetInstance(t);
		}

		public bool Process(object obj)
		{
            if (oi.HasOnePrimaryKey && oi.Cacheable)
            {
                CacheProvider.Instance[KeyGenerator.Instance[obj]] = ObjectInfo.CloneObject(obj);
            }
			list.Add( obj );
			return true;
		}
    }
}
