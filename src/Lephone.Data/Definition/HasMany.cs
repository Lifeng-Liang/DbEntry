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
        private readonly OrderBy Order;

        internal HasMany(object owner)
            : base(owner)
        {
            this.Order = new OrderBy();
        }

        public HasMany(object owner, OrderBy Order)
            : base(owner)
        {
            this.Order = Order;
        }

        public HasMany(object owner, string OrderByString)
            : base(owner)
        {
            this.Order = OrderBy.Parse(OrderByString);
        }

        private readonly List<object> _RemovedValues = new List<object>();
        List<object> IHasMany.RemovedValues { get { return _RemovedValues; } }

        protected override void InnerWrite(object item, bool IsLoad)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            MemberHandler mh = oi.GetBelongsTo(owner.GetType());
            if (mh != null)
            {
                var ll = (ILazyLoading)mh.GetValue(item);
                ll.Write(owner, IsLoad);
            }
        }

        protected override IList<T> InnerLoad()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(owner.GetType());
            object key = oi.KeyFields[0].GetValue(owner);
            IList<T> l = context
                .From<T>()
                .Where(CK.K[ForeignKeyName] == key)
                .OrderBy(Order)
                .Select();
            return l;
        }

        protected override void OnRemoveItem(T item)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            if (!oi.IsNewObject(item))
            {
                Type ot = owner.GetType();
                MemberHandler mh = oi.GetBelongsTo(ot);
                var o = (IBelongsTo)mh.GetValue(item);
                o.ForeignKey = CommonHelper.GetEmptyValue(o.ForeignKey.GetType());
                o.ForeignKeyChanged();
                _RemovedValues.Add(item);
            }
        }
    }
}
