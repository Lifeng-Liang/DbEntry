using System.Collections.Generic;
using Lephone.Core;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.QuerySyntax;
using Lephone.Web.Mvc;

public static class CommonWebExtends
{
    public static ItemList<T> GetItemList<T>(this IPagedSelector selector, long pageIndex)  where T : class, IDbObject
    {
        var result = new ItemList<T>
                         {
                             List = (List<T>) selector.GetCurrentPage(pageIndex),
                             Count = selector.GetResultCount(),
                             PageSize = selector.PageSize,
                             PageCount = selector.GetPageCount(),
                             PageIndex = pageIndex + 1
                         };
        return result;
    }

    public static ItemList<T> GetItemList<T>(this IGetPagedSelector<T> psd, ListStyle style, long? pageIndex) where T : class, IDbObject
    {
        var ps
            = (style == ListStyle.Static) || (style == ListStyle.StaticLite) || (style == ListStyle.Hybird && pageIndex != null)
            ? psd.GetStaticPagedSelector()
            : psd.GetPagedSelector();

        var listPageCount = ps.GetPageCount();
        if(style == ListStyle.StaticLite)
        {
            listPageCount--;
        }
        if (pageIndex == null)
        {
            if (style == ListStyle.Static || style == ListStyle.StaticLite)
            {
                pageIndex = listPageCount;
            }
        }
        var list = ps.GetItemList<T>((pageIndex ?? 1) - 1);
        if (style == ListStyle.Hybird && pageIndex == null)
        {
            list.PageIndex = 0;
        }
        if (style == ListStyle.StaticLite)
        {
            list.PageCount--;
        }
        return list;
    }

    public static T ParseFromRequst<T>(this T obj) where T : IDbObject
    {
        var request = System.Web.HttpContext.Current.Request;
        var oi = ObjectInfo.GetInstance(typeof(T));
        foreach(var field in oi.SimpleFields)
        {
            var value = request[field.Name];
            if(!value.IsNullOrEmpty())
            {
                field.SetValue(obj, ClassHelper.ChangeType(value, field.FieldType));
            }
        }
        return obj;
    }
}
