
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Driver;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    public class HasManyAndBelongsTo<T> : LazyLoadListBase<T>
    {
        private OrderBy Order;

        public HasManyAndBelongsTo()
        {
            this.Order = new OrderBy();
        }

        public HasManyAndBelongsTo(OrderBy Order)
        {
            this.Order = Order;
        }

        public HasManyAndBelongsTo(string OrderByString)
        {
            this.Order = OrderBy.Parse(OrderByString);
        }

        protected override void InnerWrite(object item)
        {
            /*
            ObjectInfo ti = DbObjectHelper.GetObjectInfo(typeof(T));
            if (ti.BelongsToField != null)
            {
                Type t = ti.BelongsToField.FieldType.GetGenericArguments()[0];
                if (t == owner.GetType())
                {
                    ILazyLoading ll = (ILazyLoading)ti.BelongsToField.GetValue(item);
                    ll.Write(owner);
                }
            }
            */
        }

        protected override IList<T> InnerLoad()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(owner.GetType());
            object key = oi.KeyFields[0].GetValue(owner);
            DbObjectList<T> il = new DbObjectList<T>();
            (new DbContext(driver)).FillCollection(il, typeof(T), oi.ManyToManyMediFrom,
                CK.K[ForeignKeyName] == key, Order, null);
            return il;
        }
    }
}
