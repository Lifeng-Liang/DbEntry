using System.Collections.Generic;
using Lephone.Data.Definition;

namespace Lephone.Web.Mvc
{
    public class ItemList<T> where T : class, IDbObject
    {
        public List<T> List;
        public long Count;
        public int PageSize;
        public long PageCount;
        public long PageIndex;
    }
}


