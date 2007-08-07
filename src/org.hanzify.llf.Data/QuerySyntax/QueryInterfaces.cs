
#region usings

using System;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.QuerySyntax
{
    public interface ISelectable<T>
    {
        DbObjectList<T> Select();
    }

    public interface IGetPagedSelector
    {
        IPagedSelector GetPagedSelector();
        IPagedSelector GetStaticPagedSelector();
    }

    public interface IGroupByable
    {
        DbObjectList<GroupByObject<T1>> GroupBy<T1>(string ColumnName);
    }

    public interface IRangeable<T> : ISelectable<T>, IGroupByable
    {
        ISelectable<T> Range(int StartIndex, int EndIndex);
        ISelectable<T> Range(Range r);
        IGetPagedSelector PageSize(int PageSize);
    }

    public interface IAfterWhere<T> : ISelectable<T>, IGroupByable
    {
        IRangeable<T> OrderBy(string key);
        IRangeable<T> OrderBy(params ASC[] os);
        IRangeable<T> OrderBy(OrderBy order);
        long GetCount();
    }

    public interface IWhere<T>
    {
        IAfterWhere<T> Where(WhereCondition where);
    }
}
