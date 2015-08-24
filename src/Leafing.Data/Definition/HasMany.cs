using System;
using System.Collections.Generic;
using Leafing.Data.Model.Member;

namespace Leafing.Data.Definition
{
    public interface IHasMany
    {
        List<object> RemovedValues { get; }
    }

    [Serializable]
    public class HasMany<T> : LazyLoadListBase<T>, IHasMany where T : class, IDbObject, new()
    {
        private readonly OrderBy _order;
        private readonly List<object> _removedValues = new List<object>();

        List<object> IHasMany.RemovedValues { get { return _removedValues; } }

        public HasMany(DbObjectSmartUpdate owner, string orderByString, string foreignKeyName)
            : base(owner, foreignKeyName)
        {
            this._order = OrderBy.Parse(orderByString);
        }

        protected override void InnerWrite(object item, bool isLoad)
        {
            var ctx = ModelContext.GetInstance(typeof(T));
            MemberHandler mh = ctx.Info.GetBelongsTo(Owner.GetType());
            if (mh != null)
            {
                var ll = (ILazyLoading)mh.GetValue(item);
                ll.Write(Owner, isLoad);
            }
        }

        protected override IList<T> InnerLoad()
        {
            object key = Owner.Context.Info.KeyMembers[0].GetValue(Owner);
            IList<T> l = DbEntry
                .From<T>()
                .Where(CK.K[ForeignKeyName] == key)
                .OrderBy(_order)
                .Select();
            return l;
        }

        protected override void OnRemoveItem(T item)
        {
            var ctx = ModelContext.GetInstance(typeof(T));
            if (!ctx.IsNewObject(item))
            {
                Type ot = Owner.GetType();
                MemberHandler mh = ctx.Info.GetBelongsTo(ot);
                var o = (IBelongsTo)mh.GetValue(item);
                o.ForeignKey = null;
                o.ForeignKeyChanged();
                _removedValues.Add(item);
            }
        }
    }
}
