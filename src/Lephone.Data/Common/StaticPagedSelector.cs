using System.Collections;
using Lephone.Data.Definition;

namespace Lephone.Data.Common
{
    public class StaticPagedSelector<T> : PagedSelector<T> where T : class, IDbObject
    {
        public StaticPagedSelector(Condition iwc, OrderBy oc, int PageSize, DbContext ds)
            : base(iwc, oc, PageSize, ds)
        {
        }

        public StaticPagedSelector(Condition iwc, OrderBy oc, int PageSize, DbContext ds, bool isDistinct)
            : base(iwc, oc, PageSize, ds, isDistinct)
        {
        }

        public override IList GetCurrentPage(int PageIndex)
        {
            long rc = GetResultCount();
            var firstPageSize = (int)(rc % _PageSize);
            if (firstPageSize == 0)
            {
                firstPageSize = _PageSize;
            }
            var pages = (int)((rc - firstPageSize) / _PageSize);
            PageIndex = pages - PageIndex;
            if (PageIndex <= 0)
            {
                return Entry.From<T>().Where(iwc).OrderBy(oc.OrderItems.ToArray()).Range(1, firstPageSize).Select();
            }
            int StartWith = firstPageSize + _PageSize * (PageIndex - 1);
            int tn = StartWith + _PageSize;
            IList ret = Entry.From<T>().Where(iwc).OrderBy(oc.OrderItems.ToArray()).Range(StartWith + 1, tn).Select();
            return ret;
        }
    }
}
