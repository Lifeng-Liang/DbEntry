
#region usings

using System;
using System.Collections.Generic;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;
using Lephone.Data.Definition;

#endregion

namespace Lephone.Data.QuerySyntax
{
    [Serializable]
    public class QueryContent<T> : IWhere<T>, IAfterWhere<T>, IRangeable<T>, ISelectable<T>, IGetPagedSelector where T : IDbObject
    {
        protected WhereCondition m_where;
        protected OrderBy m_order;
        protected Range m_range;
        protected DbContext m_entry;
        protected int m_pagesize;

        public QueryContent(DbContext entry)
        {
            this.m_entry = entry;
        }

        protected QueryContent(QueryContent<T> content)
        {
            if (content != null)
            {
                this.m_where = content.m_where;
                this.m_order = content.m_order;
                this.m_range = content.m_range;
                this.m_entry = content.m_entry;
            }
        }

        public IAfterWhere<T> Where(WhereCondition where)
        {
            this.m_where = where;
            return this;
        }

        public IRangeable<T> OrderBy(string key)
        {
            return OrderBy(new OrderBy(key));
        }

        public IRangeable<T> OrderBy(params ASC[] os)
        {
            this.m_order = new OrderBy(os);
            return this;
        }

        public IRangeable<T> OrderBy(OrderBy order)
        {
            this.m_order = order;
            return this;
        }

        public ISelectable<T> Range(int StartIndex, int EndIndex)
        {
            this.m_range = new Range(StartIndex, EndIndex);
            return this;
        }

        public ISelectable<T> Range(Range r)
        {
            if (r != null)
            {
                this.m_range = r;
            }
            return this;
        }

        public DbObjectList<T> Select()
        {
            DbObjectList<T> ret = new DbObjectList<T>();
            m_entry.FillCollection(ret, typeof(T), m_where, m_order, m_range);
            return ret;
        }

        public IPagedSelector GetPagedSelector()
        {
            return new PagedSelector<T>(m_where, m_order, m_pagesize, m_entry);
        }

        public IPagedSelector GetStaticPagedSelector()
        {
            return new StaticPagedSelector<T>(m_where, m_order, m_pagesize, m_entry);
        }

        public long GetCount()
        {
            return m_entry.GetResultCount(typeof(T), m_where);
        }

        public DbObjectList<GroupByObject<T1>> GroupBy<T1>(string ColumnName)
        {
            return m_entry.GetGroupBy<T1>(typeof(T), m_where, m_order, ColumnName);
        }

        public IGetPagedSelector PageSize(int PageSize)
        {
            this.m_pagesize = PageSize;
            return this;
        }
    }
}
