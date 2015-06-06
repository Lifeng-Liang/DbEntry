using System.Collections.Generic;

namespace Leafing.Data.Definition
{
    public abstract class DbObjectModelAsTree<T, TKey> : DbObjectModel<T, TKey>
        where T : DbObjectModelAsTree<T, TKey>, new()
        where TKey : struct
    {
        protected DbObjectModelAsTree()
        {
            Children = new HasMany<T>(this, "Id", "BelongsTo_Id");
            Parent = new BelongsTo<T, TKey>(this, "BelongsTo_Id");
        }

		public HasMany<T> Children { get; private set; }

		[DbColumn("BelongsTo_Id")]
		public BelongsTo<T, TKey> Parent { get; set; }
    }

    public abstract class DbObjectModelAsTree<T> : DbObjectModelAsTree<T, long>
        where T : DbObjectModelAsTree<T, long>, new()
    {
    }
}
