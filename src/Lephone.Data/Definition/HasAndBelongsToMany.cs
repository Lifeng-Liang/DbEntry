using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Common;

namespace Lephone.Data.Definition
{
    public class HasAndBelongsToMany<T> : LazyLoadListBase<T>, IHasAndBelongsToManyRelations where T : class, IDbObject
    {
        private readonly OrderBy Order;

        private readonly List<object> _SavedNewRelations = new List<object>();
        List<object> IHasAndBelongsToManyRelations.SavedNewRelations { get { return _SavedNewRelations; } }

        private readonly List<object> _RemovedRelations = new List<object>();
        List<object> IHasAndBelongsToManyRelations.RemovedRelations { get { return _RemovedRelations; } }

        internal HasAndBelongsToMany(object owner)
            : base(owner)
        {
            this.Order = new OrderBy();
            InitForeignKeyName();
        }

        public HasAndBelongsToMany(object owner, OrderBy Order)
            : base(owner)
        {
            this.Order = Order;
            InitForeignKeyName();
        }

        public HasAndBelongsToMany(object owner, string OrderByString)
            : base(owner)
        {
            this.Order = OrderBy.Parse(OrderByString);
            InitForeignKeyName();
        }

        private void InitForeignKeyName()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(owner.GetType());
            MemberHandler mh = oi.GetHasAndBelongsToMany(typeof(T));
            ForeignKeyName = mh.Name;
        }

        protected override void InnerWrite(object item, bool IsLoad)
        {
            if (m_IsLoaded)
            {
                ObjectInfo oi = ObjectInfo.GetInstance(item.GetType());
                if (oi.HasOnePremarykey)
                {
                    object key = oi.Handler.GetKeyValue(item);
                    if (!key.Equals(oi.KeyFields[0].UnsavedValue))
                    {
                        _SavedNewRelations.Add(key);
                    }
                }
                else
                {
                    throw new DataException("HasAndBelongsToMany relation need the class has one primary key.");
                }
            }
        }

        protected override IList<T> InnerLoad()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(owner.GetType());
            object key = oi.KeyFields[0].GetValue(owner);
            DbObjectList<T> il = new DbObjectList<T>();
            context.FillCollection(il, typeof(T), oi.ManyToManys[typeof(T)].From,
                CK.K[ForeignKeyName] == key, Order, null);
            return il;
        }

        protected override void OnRemoveItem(T item)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(item.GetType());
            object key = oi.Handler.GetKeyValue(item);
            if (key == oi.KeyFields[0].UnsavedValue)
            {
                _SavedNewRelations.Remove(key);
            }
            else
            {
                _RemovedRelations.Add(key);
            }
        }
    }
}
