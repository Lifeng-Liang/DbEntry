
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Lephone.Data.Common;
using Lephone.Data.Driver;

#endregion

namespace Lephone.Data.Definition
{
    public interface IHasOne
    {
        object LastValue { get; set; }
    }

    [Serializable]
    public class HasOne<T> : LazyLoadOneBase<T>, IRenew, IHasOne
    {
        private OrderBy Order;

        private object _LastValue;
        object IHasOne.LastValue { get { return _LastValue; } set { _LastValue = value; } }

        internal HasOne(object owner) : base(owner) { }

        public HasOne(object owner, OrderBy Order)
            : base(owner)
        {
            this.Order = Order;
        }

        public HasOne(object owner, string OrderByString)
            : base(owner)
        {
            this.Order = OrderBy.Parse(OrderByString);
        }

        protected override void DoWrite(object OldValue, bool IsLoad)
        {
            if (m_Value == null)
            {
                if (OldValue != null)
                {
                    ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
                    MemberHandler mh = oi.GetBelongsTo(owner.GetType());
                    if (mh != null)
                    {
                        ILazyLoading ll = (ILazyLoading)mh.GetValue(OldValue);
                        ll.Write(null, false);
                    }
                }
            }
            else
            {
                ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
                MemberHandler mh = oi.GetBelongsTo(owner.GetType());
                if (mh != null)
                {
                    ILazyLoading ll = (ILazyLoading)mh.GetValue(m_Value);
                    ll.Write(owner, false);
                }
            }
            _LastValue = OldValue;
        }

        protected override void DoSetOwner()
        {
            if (Order == null)
            {
                ObjectInfo oi = ObjectInfo.GetInstance(owner.GetType());
                if (oi.HasSystemKey)
                {
                    Order = new OrderBy(oi.KeyFields[0].Name);
                }
            }
        }

        protected override void DoLoad()
        {
            if (RelationName == null) { return; }
            ObjectInfo oi = ObjectInfo.GetInstance(owner.GetType());
            object key = oi.KeyFields[0].GetValue(owner);
            m_Value = context.GetObject<T>(CK.K[RelationName] == key, Order);

            if (m_Value != null)
            {
                ObjectInfo ti = ObjectInfo.GetInstance(typeof(T));
                MemberHandler mh = ti.GetBelongsTo(owner.GetType());
                if (mh != null)
                {
                    ILazyLoading ll = (ILazyLoading)mh.GetValue(m_Value);
                    ll.Write(owner, true);
                }
            }
        }

        void IRenew.SetAsNew()
        {
            if (m_Value != null)
            {
                MemberHandler f = ObjectInfo.GetInstance(typeof(T)).KeyFields[0];
                f.SetValue(m_Value, f.UnsavedValue);
            }
        }
    }
}
