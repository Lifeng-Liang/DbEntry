
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Driver;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public class HasMany<T> : LazyLoadListBase<T>
    {
        private OrderBy Order;

        public HasMany()
        {
            this.Order = new OrderBy();
        }

        public HasMany(OrderBy Order)
        {
            this.Order = Order;
        }

        public HasMany(string OrderByString)
        {
            this.Order = OrderBy.Parse(OrderByString);
        }

        protected override void InnerWrite(object item)
        {
            ObjectInfo ti = DbObjectHelper.GetObjectInfo(typeof(T));
            if (ti.BelongsToField != null)
            {
                Type t = ti.BelongsToField.FieldType.GetGenericArguments()[0];
                if (t == owner.GetType())
                {
                    ILazyLoading ll = (ILazyLoading)ti.BelongsToField.GetValue(item);
                    ll.Write(owner, false);
                }
            }
        }

        protected override IList<T> InnerLoad()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(owner.GetType());
            object key = oi.KeyFields[0].GetValue(owner);
            IList<T> l = (new DbContext(driver))
                .From<T>()
                .Where(CK.K[ForeignKeyName] == key)
                .OrderBy(Order)
                .Select();
            return l;
        }
    }
}
