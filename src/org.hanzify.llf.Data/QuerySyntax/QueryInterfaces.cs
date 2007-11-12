
#region usings

using System;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Util;

#endregion

namespace Lephone.Data.QuerySyntax
{
    public interface ISelectable<T> where T : IDbObject
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

    public interface IRangeable<T> : ISelectable<T>, IGroupByable where T : IDbObject
    {
        ISelectable<T> Range(int StartIndex, int EndIndex);
        ISelectable<T> Range(Range r);
        IGetPagedSelector PageSize(int PageSize);
    }

    public interface IAfterWhere<T> : ISelectable<T>, IGroupByable where T : IDbObject
    {
        IRangeable<T> OrderBy(string key);
        IRangeable<T> OrderBy(params ASC[] os);
        IRangeable<T> OrderBy(OrderBy order);
        long GetCount();
    }

    public interface IWhere<T> where T : IDbObject
    {
        IAfterWhere<T> Where(WhereCondition where);
        IAfterWhere<T> Where(CallbackHandler<FieldNameGetter<T>, WhereCondition> callback);
    }
}
