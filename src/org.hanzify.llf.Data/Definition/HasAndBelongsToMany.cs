
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
    public class HasAndBelongsToMany<T> : LazyLoadListBase<T>, ISavedNewRelations
    {
        private OrderBy Order;

        private List<long> _SavedNewRelations = new List<long>();
        List<long> ISavedNewRelations.SavedNewRelations { get { return _SavedNewRelations; } }

        internal HasAndBelongsToMany(object owner)
            : base(owner)
        {
            this.Order = new OrderBy();
            InitForeignKeyName();
        }

        public HasAndBelongsToMany(object owner, OrderBy Order)
            : base(owner)
        {
            this.Order = Order;
            InitForeignKeyName();
        }

        public HasAndBelongsToMany(object owner, string OrderByString)
            : base(owner)
        {
            this.Order = OrderBy.Parse(OrderByString);
            InitForeignKeyName();
        }

        private void InitForeignKeyName()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(owner.GetType());
            MemberHandler mh = oi.GetHasAndBelongsToMany(typeof(T));
            ForeignKeyName = mh.Name;
        }

        protected override void InnerWrite(object item, bool IsLoad)
        {
            if (m_IsLoaded)
            {
                DbObject o = item as DbObject;
                if (o.Id != 0)
                {
                    _SavedNewRelations.Add(o.Id);
                }
            }
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
            context.FillCollection(il, typeof(T), oi.ManyToManyMediFrom,
                CK.K[ForeignKeyName] == key, Order, null);
            return il;
        }
    }
}
