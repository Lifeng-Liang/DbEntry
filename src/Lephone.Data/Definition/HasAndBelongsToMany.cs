using System.Collections.Generic;
using Lephone.Data.Model;

namespace Lephone.Data.Definition
{
    public class HasAndBelongsToMany<T> : LazyLoadListBase<T>, IHasAndBelongsToManyRelations where T : class, IDbObject
    {
        private readonly OrderBy _order;

        private readonly List<object> _savedNewRelations = new List<object>();
        List<object> IHasAndBelongsToManyRelations.SavedNewRelations { get { return _savedNewRelations; } }

        private readonly List<object> _removedRelations = new List<object>();
        List<object> IHasAndBelongsToManyRelations.RemovedRelations { get { return _removedRelations; } }

        public HasAndBelongsToMany(DbObjectSmartUpdate owner, string orderByString, string foreignKeyName)
            : base(owner, foreignKeyName)
        {
            _order = OrderBy.Parse(orderByString);
        }

        protected override void InnerWrite(object item, bool isLoad)
        {
            if (IsLoaded)
            {
                var ctx = ModelContext.GetInstance(item.GetType());
                if (ctx.Info.HasOnePrimaryKey)
                {
                    object key = ctx.Handler.GetKeyValue(item);
                    if (!key.Equals(ctx.Info.KeyMembers[0].UnsavedValue))
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
            var ctx = Owner.Context;
            object key = ctx.Info.KeyMembers[0].GetValue(Owner);
            var il = new DbObjectList<T>();
            var t = typeof(T);
            var ctx0 = ModelContext.GetInstance(typeof(T));
            ctx0.Operator.FillCollection(il, t, ctx.Info.CrossTables[t].From,
                CK.K[ForeignKeyName] == key, _order, null, false);
            return il;
        }

        protected override void OnRemoveItem(T item)
        {
            var ctx = ModelContext.GetInstance(item.GetType());
            object key = ctx.Handler.GetKeyValue(item);
            if (key == ctx.Info.KeyMembers[0].UnsavedValue)
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
