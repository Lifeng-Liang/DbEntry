
using System;
using System.Collections.Generic;
using System.Text;

namespace org.hanzify.llf.Data.Definition
{
    public abstract class DbObjectModelAsTree<T> : DbObjectModel<T>
    {
        [HasMany, OrderBy("Id")]
        public abstract IList<T> Children { get; set; }

        [BelongsTo, DbColumn("BelongsTo_Id")]
        public abstract T Parent { get; set; }
    }
}
