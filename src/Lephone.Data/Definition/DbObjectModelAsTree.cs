using System.Collections.Generic;

namespace Lephone.Data.Definition
{
    public abstract class DbObjectModelAsTree<T> : DbObjectModel<T> where T : DbObjectModelAsTree<T>
    {
        [HasMany(OrderBy = "Id")]
        public abstract IList<T> Children { get; set; }

        [BelongsTo, DbColumn("BelongsTo_Id")]
        public abstract T Parent { get; set; }
    }
}
