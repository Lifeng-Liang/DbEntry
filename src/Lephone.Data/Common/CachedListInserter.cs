using System;
using System.Collections;
using Lephone.Data.Caching;

namespace Lephone.Data.Common
{
    public class CachedListInserter : IProcessor
    {
		private readonly IList _list;
        private readonly ObjectInfo _oi;

        public CachedListInserter(IList list, Type t)
		{
			this._list = list;
            _oi = ObjectInfo.GetInstance(t);
		}

		public bool Process(object obj)
		{
            if (_oi.HasOnePrimaryKey && _oi.Cacheable)
            {
                CacheProvider.Instance[KeyGenerator.Instance[obj]] = ObjectInfo.CloneObject(obj);
            }
			_list.Add( obj );
			return true;
		}
    }
}
