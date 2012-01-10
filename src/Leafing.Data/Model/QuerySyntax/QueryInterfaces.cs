using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Leafing.Data.Builder;
using Leafing.Data.Common;
using Leafing.Data.Definition;

namespace Leafing.Data.Model.QuerySyntax
{
    public interface ISelectable<T> where T : class, IDbObject
    {
        List<T> Select();
        List<TResult> Select<TResult>(Expression<Func<T, TResult>> expr);
        List<T> SelectDistinct();
        List<TResult> SelectDistinct<TResult>(Expression<Func<T, TResult>> expr);
        List<T> SelectNoLazy();
        List<TResult> SelectNoLazy<TResult>(Expression<Func<T, TResult>> expr);
        List<T> SelectDistinctNoLazy();
        List<TResult> SelectDistinctNoLazy<TResult>(Expression<Func<T, TResult>> expr);
    }

    public interface IGetPagedSelector<T> where T : class, IDbObject
    {
        IPagedSelector<T> GetPagedSelector();
        IPagedSelector<T> GetDistinctPagedSelector();
        IPagedSelector<T> GetStaticPagedSelector();
        IPagedSelector<T> GetDistinctStaticPagedSelector();
    }

    public interface IGroupByable
    {
        List<GroupByObject<T1>> GroupBy<T1>(string columnName);
        List<GroupBySumObject<T1, T2>> GroupBySum<T1, T2>(string groupbyColumnName, string sumColumnName);
    }

    public interface IRangeable<T> : ISelectable<T>, IGroupByable where T : class, IDbObject
    {
        ISelectable<T> Range(long startIndex, long endIndex);
        ISelectable<T> Range(Range r);
        IGetPagedSelector<T> PageSize(int pageSize);

        IRangeable<T> ThenBy(Expression<Func<T, object>> expr);
        IRangeable<T> ThenByDescending(Expression<Func<T, object>> expr);
    }

    public interface IAfterWhere<T> : ISelectable<T>, IGroupByable where T : class, IDbObject
    {
        IRangeable<T> OrderBy(string key);
        IRangeable<T> OrderBy(params ASC[] os);
        IRangeable<T> OrderBy(OrderBy order);
        IRangeable<T> OrderBy(Expression<Func<T, object>> expr);
        IRangeable<T> OrderByDescending(Expression<Func<T, object>> expr);

        long GetCount();
        decimal? GetMax(string columnName);
        decimal? GetMin(string columnName);
        DateTime? GetMaxDate(string columnName);
        DateTime? GetMinDate(string columnName);
        decimal? GetSum(string columnName);

        decimal? GetMax(Expression<Func<T, object>> expr);
        DateTime? GetMaxDate(Expression<Func<T, object>> expr);
        decimal? GetMin(Expression<Func<T, object>> expr);
        DateTime? GetMinDate(Expression<Func<T, object>> expr);
        decimal? GetSum(Expression<Func<T, object>> expr);

        SelectStatementBuilder GetStatement(Expression<Func<T, object>> expr);
        SelectStatementBuilder GetDistinctStatement(Expression<Func<T, object>> expr);
        SelectStatementBuilder GetStatement(string columnName);
        SelectStatementBuilder GetDistinctStatement(string columnName);
    }

    public interface IWhere<T> where T : class, IDbObject
    {
        IAfterWhere<T> Where(Condition where);
        IAfterWhere<T> Where(Expression<Func<T, bool>> expr);
    }
}
