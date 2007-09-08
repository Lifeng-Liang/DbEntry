
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Driver;

#endregion

namespace Lephone.Data.Definition
{
    [Serializable]
    public class HasMany<T> : LazyLoadListBase<T>
    {
        private OrderBy Order;

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

        protected override void InnerWrite(object item, bool IsLoad)
        {
            ObjectInfo ti = DbObjectHelper.GetObjectInfo(typeof(T));
            MemberHandler mh = ti.GetBelongsTo(owner.GetType());
            if (mh != null)
            {
                ILazyLoading ll = (ILazyLoading)mh.GetValue(item);
                ll.Write(owner, IsLoad);
            }
        }

        protected override IList<T> InnerLoad()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(owner.GetType());
            object key = oi.KeyFields[0].GetValue(owner);
            IList<T> l = context
                .From<T>()
                .Where(CK.K[ForeignKeyName] == key)
                .OrderBy(Order)
                .Select();
            return l;
        }
    }
}
