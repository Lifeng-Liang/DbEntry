using System.Collections.Generic;

namespace Leafing.Data.Definition {
    public abstract class DbObjectModelAsTree<T, TKey> : DbObjectModel<T, TKey>
        where T : DbObjectModelAsTree<T, TKey>, new()
        where TKey : struct {
        [OrderBy("Id")]
        public HasMany<T> Children { get; set; }

        [DbColumn("BelongsTo_Id")]
        public BelongsTo<T, TKey> MParent { get; set; }

        [Exclude]
        public T Parent {
            get { return MParent.Value; }
            set { MParent.Value = value; }
        }
    }

    public abstract class DbObjectModelAsTree<T> : DbObjectModelAsTree<T, long>
        where T : DbObjectModelAsTree<T, long>, new() {
    }
}