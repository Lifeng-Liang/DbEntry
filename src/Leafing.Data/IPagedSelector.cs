using System.Collections.Generic;
using Leafing.Data.Definition;

namespace Leafing.Data
{
    public interface IPagedSelector<T> where T : class, IDbObject
    {
        int PageSize{get;}
        long GetResultCount();
        List<T> GetCurrentPage(long pageIndex);
        long GetPageCount();
    }
}
