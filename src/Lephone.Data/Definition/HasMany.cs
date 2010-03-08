using System;
using System.Collections.Generic;
using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Data.Definition
{
    public interface IHasMany
    {
        List<object> RemovedValues { get; }
    }

    [Serializable]
    public class HasMany<T> : LazyLoadListBase<T>, IHasMany where T : class, IDbObject
    {
        private readonly OrderBy _order;

        internal HasMany(object owner)
            : base(owner)
        {
            this._order = new OrderBy();
        }

        public HasMany(object owner, OrderBy order)
            : base(owner)
        {
            this._order = order;
        }

        public HasMany(object owner, string orderByString)
            : base(owner)
        {
            this._order = OrderBy.Parse(orderByString);
        }

        private readonly List<object> _removedValues = new List<object>();
        List<object> IHasMany.RemovedValues { get { return _removedValues; } }

        protected override void InnerWrite(object item, bool isLoad)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            MemberHandler mh = oi.GetBelongsTo(Owner.GetType());
            if (mh != null)
            {
                var ll = (ILazyLoading)mh.GetValue(item);
                ll.Write(Owner, isLoad);
            }
        }

        protected override IList<T> InnerLoad()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(Owner.GetType());
            object key = oi.KeyFields[0].GetValue(Owner);
            IList<T> l = oi.Context
                .From<T>()
                .Where(CK.K[ForeignKeyName] == key)
                .OrderBy(_order)
                .Select();
            return l;
        }

        protected override void OnRemoveItem(T item)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            if (!oi.IsNewObject(item))
            {
                Type ot = Owner.GetType();
                MemberHandler mh = oi.GetBelongsTo(ot);
                var o = (IBelongsTo)mh.GetValue(item);
                o.ForeignKey = CommonHelper.GetEmptyValue(o.ForeignKey.GetType());
                o.ForeignKeyChanged();
                _removedValues.Add(item);
            }
        }
    }
}
