using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lephone.Data.Builder;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.Model.Linq;

namespace Lephone.Data.Model.QuerySyntax
{
    [Serializable]
    public class QueryContent<T> : IWhere<T>, IAfterWhere<T>, IRangeable<T>, IGetPagedSelector<T> where T : class, IDbObject
    {
        private readonly ModelContext _ctx;
        private Condition _where;
        private OrderBy _order;
        private Range _range;
        private int _pageSize;

        public QueryContent(ModelContext ctx)
        {
            _ctx = ctx;
        }

        protected internal QueryContent(QueryContent<T> content)
        {
            if (content != null)
            {
                _where = content._where;
                _order = content._order;
                _range = content._range;
                _ctx = content._ctx;
            }
        }

        public IAfterWhere<T> Where(Condition where)
        {
            _where = where;
            return this;
        }

        public IAfterWhere<T> Where(Expression<Func<T, bool>> expr)
        {
            if (expr != null)
            {
                _where = ExpressionParser<T>.Parse(expr);
            }
            return this;
        }

        public IRangeable<T> OrderBy(string key)
        {
            return OrderBy(new OrderBy(key));
        }

        public IRangeable<T> OrderBy(params ASC[] os)
        {
            _order = new OrderBy(os);
            return this;
        }

        public IRangeable<T> OrderBy(OrderBy order)
        {
            _order = order;
            return this;
        }

        public ISelectable<T> Range(long startIndex, long endIndex)
        {
            _range = new Range(startIndex, endIndex);
            return this;
        }

        public ISelectable<T> Range(Range r)
        {
            if (r != null)
            {
                _range = r;
            }
            return this;
        }

        public List<T> Select()
        {
            return InnerSelect<T>(false, false);
        }

        public List<T> SelectNoLazy()
        {
            return InnerSelect<T>(false, true);
        }

        public List<TResult> SelectNoLazy<TResult>(Expression<Func<T, TResult>> expr)
        {
            LinqExpressionParser<T>.ProcessSelectColumns(expr);
            return InnerSelect<TResult>(false, true);
        }

        public List<T> SelectDistinct()
        {
            return InnerSelect<T>(true, false);
        }

        public List<TResult> SelectDistinct<TResult>(Expression<Func<T, TResult>> expr)
        {
            LinqExpressionParser<T>.ProcessSelectColumns(expr);
            return InnerSelect<TResult>(true, false);
        }

        public List<T> SelectDistinctNoLazy()
        {
            return InnerSelect<T>(true, true);
        }

        public List<TResult> SelectDistinctNoLazy<TResult>(Expression<Func<T, TResult>> expr)
        {
            LinqExpressionParser<T>.ProcessSelectColumns(expr);
            return InnerSelect<TResult>(true, true);
        }

        private List<TResult> InnerSelect<TResult>(bool distinct, bool noLazy)
        {
            var ret = new List<TResult>();
            _ctx.Operator.FillCollection(ret, typeof(TResult), null, _where, _order, _range, distinct, noLazy);
            return ret;
        }

        public List<TResult> Select<TResult>()
        {
            return InnerSelect<TResult>(false, false);
        }

        public List<TResult> Select<TResult>(Expression<Func<T, TResult>> expr)
        {
            LinqExpressionParser<T>.ProcessSelectColumns(expr);
            return Select<TResult>();
        }

        public SelectStatementBuilder GetStatement(Expression<Func<T, object>> expr)
        {
            return InnerGetSelectStatement(false, GetColumnName(expr));
        }

        public SelectStatementBuilder GetDistinctStatement(Expression<Func<T, object>> expr)
        {
            return InnerGetSelectStatement(true, GetColumnName(expr));
        }

        public SelectStatementBuilder GetStatement(string columnName)
        {
            return InnerGetSelectStatement(false, columnName);
        }

        public SelectStatementBuilder GetDistinctStatement(string columnName)
        {
            return InnerGetSelectStatement(true, columnName);
        }

        private SelectStatementBuilder InnerGetSelectStatement(bool distinct, string colName)
        {
            var smt = _ctx.Composer.GetSelectStatementBuilder(null, _where, _order, 
                _range, distinct, false, _ctx.Info.HandleType, colName);
            return smt;
        }

        public IPagedSelector<T> GetPagedSelector()
        {
            return new PagedSelector<T>(_where, _order, _pageSize);
        }

        public IPagedSelector<T> GetDistinctPagedSelector()
        {
            return new PagedSelector<T>(_where, _order, _pageSize, true);
        }

        public IPagedSelector<T> GetStaticPagedSelector()
        {
            return new StaticPagedSelector<T>(_where, _order, _pageSize);
        }

        public IPagedSelector<T> GetDistinctStaticPagedSelector()
        {
            return new StaticPagedSelector<T>(_where, _order, _pageSize, true);
        }

        public long GetCount()
        {
            return _ctx.Operator.GetResultCount(_where);
        }

        public decimal? GetMax(string columnName)
        {
            return _ctx.Operator.GetMax(_where, columnName);
        }

        public decimal? GetMin(string columnName)
        {
            return _ctx.Operator.GetMin(_where, columnName);
        }

        public DateTime? GetMaxDate(string columnName)
        {
            return _ctx.Operator.GetMaxDate(_where, columnName);
        }

        public DateTime? GetMinDate(string columnName)
        {
            return _ctx.Operator.GetMinDate(_where, columnName);
        }

        public decimal? GetSum(string columnName)
        {
            return _ctx.Operator.GetSum(_where, columnName);
        }

        public IRangeable<T> OrderBy(Expression<Func<T, object>> expr)
        {
            return AddOrderBy(this, expr, true);
        }

        public IRangeable<T> OrderByDescending(Expression<Func<T, object>> expr)
        {
            return AddOrderBy(this, expr, false);
        }

        public decimal? GetMax(Expression<Func<T, object>> expr)
        {
            string n = GetColumnName(expr);
            return GetMax(n);
        }

        public DateTime? GetMaxDate(Expression<Func<T, object>> expr)
        {
            string n = GetColumnName(expr);
            return GetMaxDate(n);
        }

        public decimal? GetMin(Expression<Func<T, object>> expr)
        {
            string n = GetColumnName(expr);
            return GetMin(n);
        }

        public DateTime? GetMinDate(Expression<Func<T, object>> expr)
        {
            string n = GetColumnName(expr);
            return GetMinDate(n);
        }

        public decimal? GetSum(Expression<Func<T, object>> expr)
        {
            string n = GetColumnName(expr);
            return GetSum(n);
        }

        public List<GroupByObject<T1>> GroupBy<T1>(string columnName)
        {
            return _ctx.Operator.GetGroupBy<T1>(_where, _order, columnName);
        }

        public List<GroupBySumObject<T1, T2>> GroupBySum<T1, T2>(string groupbyColumnName, string sumColumnName)
        {
            return _ctx.Operator.GetGroupBySum<T1, T2>(_where, _order, groupbyColumnName, sumColumnName);
        }

        public IGetPagedSelector<T> PageSize(int pageSize)
        {
            _pageSize = pageSize;
            return this;
        }

        public IRangeable<T> ThenBy(Expression<Func<T, object>> expr)
        {
            return AddOrderBy(this, expr, true);
        }

        public IRangeable<T> ThenByDescending(Expression<Func<T, object>> expr)
        {
            return AddOrderBy(this, expr, false);
        }

        #region Linq help methods

        private static IRangeable<T> AddOrderBy(QueryContent<T> me, Expression<Func<T, object>> expr, bool isAsc)
        {
            string n = GetColumnName(expr);
            if (me._order == null)
            {
                me._order = new OrderBy();
            }
            me._order.OrderItems.Add(isAsc ? new ASC(n) : new DESC(n));
            return me;
        }

        private static string GetColumnName(Expression<Func<T, object>> expr)
        {
            ColumnFunction function;
            MemberExpression obj;
            var key = ExpressionParser<T>.GetMemberName(expr.Body, out function, out obj);
            return key;
        }

        #endregion
    }
}
