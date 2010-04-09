using System.Collections.Generic;
using Lephone.Data.Common;

namespace Lephone.Data.Definition
{
    public class HasAndBelongsToMany<T> : LazyLoadListBase<T>, IHasAndBelongsToManyRelations where T : class, IDbObject
    {
        private readonly OrderBy _order;

        private readonly List<object> _savedNewRelations = new List<object>();
        List<object> IHasAndBelongsToManyRelations.SavedNewRelations { get { return _savedNewRelations; } }

        private readonly List<object> _removedRelations = new List<object>();
        List<object> IHasAndBelongsToManyRelations.RemovedRelations { get { return _removedRelations; } }

        internal HasAndBelongsToMany(object owner)
            : base(owner)
        {
            _order = new OrderBy();
            InitForeignKeyName();
        }

        public HasAndBelongsToMany(object owner, OrderBy order)
            : base(owner)
        {
            this._order = order;
            InitForeignKeyName();
        }

        public HasAndBelongsToMany(object owner, string orderByString)
            : base(owner)
        {
            _order = OrderBy.Parse(orderByString);
            InitForeignKeyName();
        }

        private void InitForeignKeyName()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(Owner.GetType());
            MemberHandler mh = oi.GetHasAndBelongsToMany(typeof(T));
            ForeignKeyName = mh.Name;
        }

        protected override void InnerWrite(object item, bool isLoad)
        {
            if (IsLoaded)
            {
                var oi = ObjectInfo.GetInstance(item.GetType());
                if (oi.HasOnePrimaryKey)
                {
                    object key = oi.Handler.GetKeyValue(item);
                    if (!key.Equals(oi.KeyFields[0].UnsavedValue))
                    {
                        _savedNewRelations.Add(key);
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
            ObjectInfo oi = ObjectInfo.GetInstance(Owner.GetType());
            object key = oi.KeyFields[0].GetValue(Owner);
            var il = new DbObjectList<T>();
            var t = typeof(T);
            oi.Context.FillCollection(il, t, t, oi.CrossTables[t].From,
                CK.K[ForeignKeyName] == key, _order, null, false);
            return il;
        }

        protected override void OnRemoveItem(T item)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(item.GetType());
            object key = oi.Handler.GetKeyValue(item);
            if (key == oi.KeyFields[0].UnsavedValue)
            {
                _savedNewRelations.Remove(key);
            }
            else
            {
                _removedRelations.Add(key);
            }
        }
    }
}
