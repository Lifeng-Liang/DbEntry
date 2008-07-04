using System.Collections;

namespace Lephone.Data
{
    public interface IPagedSelector
    {
        int PageSize{get;}
        long GetResultCount();
        IList GetCurrentPage(int PageIndex);
    }
}
