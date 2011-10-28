using System.Collections.Generic;
using Lephone.Data.Definition;

namespace Lephone.Data
{
    public interface IPagedSelector<T> where T : class, IDbObject
    {
        int PageSize{get;}
        long GetResultCount();
        List<T> GetCurrentPage(long pageIndex);
        long GetPageCount();
    }
}
