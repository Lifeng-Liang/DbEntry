using System.Collections.Generic;

namespace Lephone.Data.Definition
{
    public abstract class DbObjectModelAsTree<T> : DbObjectModel<T> where T : DbObjectModelAsTree<T>
    {
        protected DbObjectModelAsTree()
        {
            XChildren = new HasMany<T>(this, "Id", "BelongsTo_Id");
            XParent = new BelongsTo<T>(this, "BelongsTo_Id");
        }

        public HasMany<T> XChildren;

        [DbColumn("BelongsTo_Id")]
        public BelongsTo<T> XParent;

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
}
