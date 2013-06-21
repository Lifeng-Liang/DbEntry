using Leafing.Data;
using Leafing.Data.Definition;
using Leafing.Data.Model.QuerySyntax;
using Leafing.Web;
using Leafing.Web.Mvc;

// ReSharper disable CheckNamespace
public static class CommonWebExtends
{
    public static ItemList<T> GetItemList<T>(this IPagedSelector<T> selector, long pageIndex) where T : class, IDbObject, new()
    {
        var result = new ItemList<T>
                         {
                             List = selector.GetCurrentPage(pageIndex),
                             Count = selector.GetResultCount(),
                             PageSize = selector.PageSize,
                             PageCount = selector.GetPageCount(),
                             PageIndex = pageIndex + 1
                         };
        return result;
    }

    public static ItemList<T> GetItemList<T>(this IGetPagedSelector<T> psd, ListStyle style, long? pageIndex) where T : class, IDbObject, new()
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
        var list = ps.GetItemList((pageIndex ?? 1) - 1);
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
}
