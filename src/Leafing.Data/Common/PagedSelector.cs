using System;
using System.Collections.Generic;
using Leafing.Data.Definition;

namespace Leafing.Data.Common {
    public class PagedSelector<T> : IPagedSelector<T> where T : class, IDbObject, new() {
        protected Condition iwc;
        protected OrderBy oc;
        internal int _PageSize;
        protected ModelContext Entry;
        internal bool IsDistinct;
        protected long ResultCount = -1;
        protected long PageCount = -1;

        public PagedSelector(Condition iwc, OrderBy oc, int pageSize)
            : this(iwc, oc, pageSize, false) {
        }

        public PagedSelector(Condition iwc, OrderBy oc, int pageSize, bool isDistinct) {
            this.iwc = iwc;
            this.oc = oc;
            this._PageSize = pageSize;
            this.Entry = ModelContext.GetInstance(typeof(T));
            this.IsDistinct = isDistinct;
        }

        public int PageSize {
            get { return _PageSize; }
        }

        public long GetResultCount() {
            if (ResultCount < 0) {
                ResultCount = Entry.Operator.GetResultCount(iwc, IsDistinct);
            }
            return ResultCount;
        }

        public long GetPageCount() {
            if (PageCount < 0) {
                PageCount = (long)Math.Floor((double)(GetResultCount() - 1) / _PageSize) + 1;
            }
            return PageCount;
        }

        public virtual List<T> GetCurrentPage(long pageIndex) {
            long startWith = _PageSize * pageIndex;
            long tn = startWith + _PageSize;
            var query = Entry.From<T>().Where(iwc).OrderBy(oc.OrderItems.ToArray()).Range(startWith + 1, tn);
            if (IsDistinct) {
                return query.SelectDistinct();
            }
            return query.Select();
        }
    }
}
