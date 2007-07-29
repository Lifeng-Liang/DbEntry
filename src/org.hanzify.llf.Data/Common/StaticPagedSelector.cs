
#region usings

using System;
using System.Collections;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Builder.Clause;

#endregion

namespace org.hanzify.llf.Data.Common
{
    public class StaticPagedSelector<T> : PagedSelector<T>
    {
        public StaticPagedSelector(WhereCondition iwc, OrderBy oc, int PageSize, DbContext ds)
            : base(iwc, oc, PageSize, ds)
        {
        }

        public override IList GetCurrentPage(int PageIndex)
        {
            long rc = GetResultCount();
            int firstPageSize = (int)(rc % _PageSize);
            if (firstPageSize == 0)
            {
                firstPageSize = _PageSize;
            }
            int pages = (int)((rc - firstPageSize) / _PageSize);
            PageIndex = pages - PageIndex;
            if (PageIndex <= 0)
            {
                return Entry.From<T>().Where(iwc).OrderBy(oc.OrderItems).Range(1, firstPageSize).Select();
            }
            else
            {
                int StartWith = firstPageSize + _PageSize * (PageIndex - 1);
                int tn = StartWith + _PageSize;
                IList ret = Entry.From<T>().Where(iwc).OrderBy(oc.OrderItems).Range(StartWith + 1, tn).Select();
                return ret;
            }
        }
    }
}
