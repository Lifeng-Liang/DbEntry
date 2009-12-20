using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;
using Lephone.Data.QuerySyntax;
using Lephone.Web.Rails;

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
            = (style == ListStyle.Static) || (style == ListStyle.Hybird && pageIndex != null)
            ? psd.GetStaticPagedSelector()
            : psd.GetPagedSelector();

        var listPageCount = ps.GetPageCount();
        if (pageIndex == null)
        {
            if (style == ListStyle.Static)
            {
                pageIndex = listPageCount;
            }
        }
        return ps.GetItemList<T>((pageIndex ?? 1) - 1);
    }
}
