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

        public HasOne(DbObjectSmartUpdate owner, string orderByString, string relationName)
            : base(owner, relationName)
        {
            this._order = OrderBy.Parse(orderByString);
        }

        protected override void DoWrite(object oldValue, bool isLoad)
        {
            if (m_Value == null)
            {
                if (oldValue != null)
                {
                    var ctx = ModelContext.GetInstance(typeof(T));
                    var mh = ctx.Info.GetBelongsTo(Owner.GetType());
                    if (mh != null)
                    {
                        var ll = (ILazyLoading)mh.GetValue(oldValue);
                        ll.Write(null, false);
                    }
                }
            }
            else
            {
                var ctx = ModelContext.GetInstance(typeof(T));
                MemberHandler mh = ctx.Info.GetBelongsTo(Owner.GetType());
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
            object key = Owner.Context.Info.KeyFields[0].GetValue(Owner);
            m_Value = DbEntry.GetObject<T>(CK.K[RelationName] == key, _order);

            if (m_Value != null)
            {
                var ctx0 = ModelContext.GetInstance(typeof(T));
                MemberHandler mh = ctx0.Info.GetBelongsTo(Owner.GetType());
                if (mh != null)
                {
                    var ll = (ILazyLoading)mh.GetValue(m_Value);
                    ll.Write(Owner, true);
                }
            }
        }
    }
}
