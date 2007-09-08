
#region usings

using System;
using System.Collections;

#endregion

namespace Lephone.Data
{
    public interface IPagedSelector
    {
        int PageSize{get;}
        long GetResultCount();
        IList GetCurrentPage(int PageIndex);
    }
}
