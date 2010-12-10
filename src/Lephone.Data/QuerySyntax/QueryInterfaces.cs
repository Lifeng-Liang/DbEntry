using System;
using System.Linq.Expressions;
using Lephone.Data.Builder;
using Lephone.Data.Common;
using Lephone.Data.Definition;

namespace Lephone.Data.QuerySyntax
{
    public interface ISelectable<T> where T : class, IDbObject
    {
        DbObjectList<T> Select();
        DbObjectList<T> SelectDistinct();
        DbObjectList<T> SelectNoLazy();
        DbObjectList<T> SelectDistinctNoLazy();
    }

    public interface IGetPagedSelector<T>
    {
        IPagedSelector GetPagedSelector();
        IPagedSelector GetDistinctPagedSelector();
        IPagedSelector GetStaticPagedSelector();
        IPagedSelector GetDistinctStaticPagedSelector();
    }

    public interface IGroupByable
    {
        DbObjectList<GroupByObject<T1>> GroupBy<T1>(string columnName);
        DbObjectList<GroupBySumObject<T1, T2>> GroupBySum<T1, T2>(string groupbyColumnName, string sumColumnName);
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
        long GetCount();
        decimal? GetMax(string columnName);
        decimal? GetMin(string columnName);
        DateTime? GetMaxDate(string columnName);
        DateTime? GetMinDate(string columnName);
        decimal? GetSum(string columnName);

        IRangeable<T> OrderBy(Expression<Func<T, object>> expr);
        IRangeable<T> OrderByDescending(Expression<Func<T, object>> expr);
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
