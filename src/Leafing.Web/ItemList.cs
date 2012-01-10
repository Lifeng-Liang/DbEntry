using System.Collections.Generic;
using Leafing.Data.Definition;

namespace Leafing.Web
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


