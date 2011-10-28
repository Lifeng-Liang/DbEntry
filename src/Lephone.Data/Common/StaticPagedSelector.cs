using System.Collections.Generic;
using Lephone.Data.Definition;

namespace Lephone.Data.Common
{
    public class StaticPagedSelector<T> : PagedSelector<T> where T : class, IDbObject
    {
        public StaticPagedSelector(Condition iwc, OrderBy oc, int pageSize)
            : base(iwc, oc, pageSize)
        {
        }

        public StaticPagedSelector(Condition iwc, OrderBy oc, int pageSize, bool isDistinct)
            : base(iwc, oc, pageSize, isDistinct)
        {
        }

        public override List<T> GetCurrentPage(long pageIndex)
        {
            long rc = GetResultCount();
            var firstPageSize = (int)(rc % _PageSize);
            if (firstPageSize == 0)
            {
                firstPageSize = _PageSize;
            }
            var pages = (int)((rc - firstPageSize) / _PageSize);
            pageIndex = pages - pageIndex;
            if (pageIndex <= 0)
            {
                return Entry.From<T>().Where(iwc).OrderBy(oc.OrderItems.ToArray()).Range(1, firstPageSize).Select();
            }
            long startWith = firstPageSize + _PageSize * (pageIndex - 1);
            long tn = startWith + _PageSize;
            var ret = Entry.From<T>().Where(iwc).OrderBy(oc.OrderItems.ToArray()).Range(startWith + 1, tn).Select();
            return ret;
        }
    }
}
