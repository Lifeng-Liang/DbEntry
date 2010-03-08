using System;
using Lephone.Data.Common;

namespace Lephone.Data.Definition
{
    public interface IHasOne
    {
        object LastValue { get; set; }
    }

    [Serializable]
    public class HasOne<T> : LazyLoadOneBase<T>, IHasOne where T : class, IDbObject
    {
        private readonly OrderBy _order;

        object IHasOne.LastValue { get; set; }

        internal HasOne(object owner) : base(owner) { }

        public HasOne(object owner, OrderBy order)
            : base(owner)
        {
            this._order = order;
            if (_order == null)
            {
                var oi = ObjectInfo.GetInstance(Owner.GetType());
                if (oi.HasSystemKey)
                {
                    _order = new OrderBy(oi.KeyFields[0].Name);
                }
            }
        }

        public HasOne(object owner, string orderByString)
            : base(owner)
        {
            this._order = OrderBy.Parse(orderByString);
        }

        protected override void DoWrite(object oldValue, bool isLoad)
        {
            if (m_Value == null)
            {
                if (oldValue != null)
                {
                    var oi = ObjectInfo.GetInstance(typeof(T));
                    MemberHandler mh = oi.GetBelongsTo(Owner.GetType());
                    if (mh != null)
                    {
                        var ll = (ILazyLoading)mh.GetValue(oldValue);
                        ll.Write(null, false);
                    }
                }
            }
            else
            {
                var oi = ObjectInfo.GetInstance(typeof(T));
                MemberHandler mh = oi.GetBelongsTo(Owner.GetType());
                if (mh != null)
                {
                    var ll = (ILazyLoading)mh.GetValue(m_Value);
                    ll.Write(Owner, false);
                }
            }
            ((IHasOne)this).LastValue = oldValue;
        }

        protected override void DoLoad()
        {
            if (RelationName == null) { return; }
            var oi = ObjectInfo.GetInstance(Owner.GetType());
            object key = oi.KeyFields[0].GetValue(Owner);
            m_Value = oi.Context.GetObject<T>(CK.K[RelationName] == key, _order);

            if (m_Value != null)
            {
                var ti = ObjectInfo.GetInstance(typeof(T));
                MemberHandler mh = ti.GetBelongsTo(Owner.GetType());
                if (mh != null)
                {
                    var ll = (ILazyLoading)mh.GetValue(m_Value);
                    ll.Write(Owner, true);
                }
            }
        }
    }
}
