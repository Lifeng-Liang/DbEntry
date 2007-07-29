
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Driver;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public class HasOne<T> : ILazyLoading, IRenew
    {
        private OrderBy Order;
        private object owner;
        private string ForeignKeyName;
        private DbDriver driver;
        private bool _IsLoaded;
        private T _Value;

        bool ILazyLoading.IsLoaded
        {
            get { return _IsLoaded; }
            set { _IsLoaded = value; }
        }

        object ILazyLoading.Read()
        {
            if (!_IsLoaded)
            {
                ((ILazyLoading)this).Load();
                _IsLoaded = true;
                driver = null;
            }
            return _Value;
        }

        void ILazyLoading.Write(object item, bool IsLoad)
        {
            _Value = (T)item;
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
            if (oi.BelongsToField != null)
            {
                Type t = oi.BelongsToField.FieldType.GetGenericArguments()[0];
                if (t == owner.GetType())
                {
                    ILazyLoading ll = (ILazyLoading)oi.BelongsToField.GetValue(_Value);
                    ll.Write(owner, false);
                }
            }
            _IsLoaded = true;
            driver = null;
        }

        public T Value
        {
            get
            {
                return (T)((ILazyLoading)this).Read();
            }
            set
            {
                ((ILazyLoading)this).Write(value, false);
            }
        }

        public HasOne() { }

        public HasOne(OrderBy Order)
        {
            this.Order = Order;
        }

        public HasOne(string OrderByString)
        {
            this.Order = OrderBy.Parse(OrderByString);
        }

        void ILazyLoading.SetOwner(object owner, string ColumnName)
        {
            this.owner = owner;
            if (Order == null)
            {
                ObjectInfo oi = DbObjectHelper.GetObjectInfo(owner.GetType());
                if (oi.HasSystemKey)
                {
                    Order = new OrderBy(oi.KeyFields[0].Name);
                }
            }
        }

        void ILazyLoading.Init(DbDriver driver, string ForeignKeyName)
        {
            this.driver = driver;
            this.ForeignKeyName = ForeignKeyName;
        }

        void ILazyLoading.Load()
        {
            if (ForeignKeyName == null) { return; }
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(owner.GetType());
            object key = oi.KeyFields[0].GetValue(owner);
            _Value = (new DbContext(driver)).GetObject<T>(CK.K[ForeignKeyName] == key, Order);

            if (_Value != null)
            {
                ObjectInfo ti = DbObjectHelper.GetObjectInfo(typeof(T));
                if (ti.BelongsToField != null)
                {
                    ILazyLoading ll = (ILazyLoading)ti.BelongsToField.GetValue(_Value);
                    ll.Write(owner, true);
                }
            }
        }

        void IRenew.SetAsNew()
        {
            if (_Value != null)
            {
                MemberHandler f = DbObjectHelper.GetObjectInfo(typeof(T)).KeyFields[0];
                f.SetValue(_Value, f.UnsavedValue);
            }
        }
    }
}
