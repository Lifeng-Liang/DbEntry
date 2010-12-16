using System.Collections.Generic;

namespace Lephone.Data.Definition
{
    public abstract class DbObjectModelAsTree<T, TKey> : DbObjectModel<T, TKey>
        where T : DbObjectModelAsTree<T, TKey>, new()
        where TKey : struct
    {
        protected DbObjectModelAsTree()
        {
            XChildren = new HasMany<T>(this, "Id", "BelongsTo_Id");
            XParent = new BelongsTo<T, TKey>(this, "BelongsTo_Id");
        }

        public HasMany<T> XChildren;

        [DbColumn("BelongsTo_Id")]
        public BelongsTo<T, TKey> XParent;

        [HasMany(OrderBy = "Id")]
        public IList<T> Children
        {
            get
            {
                return XChildren;
            }
        }

        [BelongsTo, DbColumn("BelongsTo_Id")]
        public T Parent
        {
            get
            {
                return XParent.Value;
            }
            set
            {
                XParent.Value = value;
            }
        }
    }

    public abstract class DbObjectModelAsTree<T> : DbObjectModelAsTree<T, long>
        where T : DbObjectModelAsTree<T, long>, new()
    {
    }
}
