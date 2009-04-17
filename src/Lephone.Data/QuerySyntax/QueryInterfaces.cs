using Lephone.Data.Common;
using Lephone.Data.Definition;

namespace Lephone.Data.QuerySyntax
{
    public interface ISelectable<T> where T : class, IDbObject
    {
        DbObjectList<T> Select();
        DbObjectList<T> SelectDistinct();
    }

    public interface IGetPagedSelector
    {
        IPagedSelector GetPagedSelector();
        IPagedSelector GetDistinctPagedSelector();
        IPagedSelector GetStaticPagedSelector();
        IPagedSelector GetDistinctStaticPagedSelector();
    }

    public interface IGroupByable
    {
        DbObjectList<GroupByObject<T1>> GroupBy<T1>(string ColumnName);
    }

    public interface IRangeable<T> : ISelectable<T>, IGroupByable where T : class, IDbObject
    {
        ISelectable<T> Range(int StartIndex, int EndIndex);
        ISelectable<T> Range(Range r);
        IGetPagedSelector PageSize(int PageSize);
    }

    public interface IAfterWhere<T> : ISelectable<T>, IGroupByable where T : class, IDbObject
    {
        IRangeable<T> OrderBy(string key);
        IRangeable<T> OrderBy(params ASC[] os);
        IRangeable<T> OrderBy(OrderBy order);
        long GetCount();
        decimal? GetMax(string columnName);
        decimal? GetMin(string columnName);
        decimal? GetSum(string columnName);
    }

    public interface IWhere<T> where T : class, IDbObject
    {
        IAfterWhere<T> Where(WhereCondition where);
    }
}
