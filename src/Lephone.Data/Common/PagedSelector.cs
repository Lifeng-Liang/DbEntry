using System.Collections;
using Lephone.Data.Definition;

namespace Lephone.Data.Common
{
    public class PagedSelector<T> : IPagedSelector where T : class, IDbObject
    {
        protected WhereCondition iwc;
        protected OrderBy oc;
        internal int _PageSize;
        protected DbContext Entry;
        internal bool isDistinct;

        public PagedSelector(WhereCondition iwc, OrderBy oc, int PageSize, DbContext ds)
            : this(iwc, oc, PageSize, ds, false)
        {
        }

        public PagedSelector(WhereCondition iwc, OrderBy oc, int PageSize, DbContext ds, bool isDistinct)
        {
            this.iwc = iwc;
            this.oc = oc;
            this._PageSize = PageSize;
            this.Entry = ds;
            this.isDistinct = isDistinct;
        }

        int IPagedSelector.PageSize
        {
            get { return _PageSize; }
        }

        public long GetResultCount()
        {
            return Entry.GetResultCount(typeof(T), iwc, isDistinct);
        }

        public virtual IList GetCurrentPage(int PageIndex)
        {
            int StartWith = _PageSize * PageIndex;
            int tn = StartWith + _PageSize;
            var query = Entry.From<T>().Where(iwc).OrderBy(oc.OrderItems.ToArray()).Range(StartWith + 1, tn);
            if(isDistinct)
            {
                return query.SelectDistinct();
            }
            return query.Select();
        }
    }
}
