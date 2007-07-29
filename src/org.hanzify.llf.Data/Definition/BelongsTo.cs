
#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using org.hanzify.llf.Data.Driver;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public class BelongsTo<T> : IBelongsTo
    {
        private object owner;
        private string ColumnName;
        private string ForeignKeyName;
        private object _ForeignKey;

        private bool LazyLoad;
        private DbDriver driver;
        private bool _IsLoaded;
        private T _Value;

        public event CallbackObjectHandler<string> ValueChanged;

        public object ForeignKey
        {
            get { return _ForeignKey; }
            set { _ForeignKey = value; }
        }

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
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
            if (oi.KeyFields != null & oi.KeyFields.Length == 1)
            {
                _Value = (T)item;
                _ForeignKey = oi.KeyFields[0].GetValue(item);
                _IsLoaded = true;
                driver = null;
                if (ValueChanged != null && !IsLoad)
                {
                    ValueChanged(ColumnName);
                }
            }
            else
            {
                throw new DbEntryException("The object must have one key.");
            }
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

        void ILazyLoading.SetOwner(object owner, string ColumnName)
        {
            this.owner = owner;
            this.ColumnName = ColumnName;
        }

        void ILazyLoading.Init(DbDriver driver, string ForeignKeyName)
        {
            this.driver = driver;
            this.ForeignKeyName = ForeignKeyName;
        }

        void ILazyLoading.Load()
        {
            _Value = (new DbContext(driver)).GetObject<T>(_ForeignKey);
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
            foreach (MemberHandler f in oi.Fields)
            {
                if (f.IsHasOne || f.IsHasMany)
                {
                    Type t = f.FieldType.GetGenericArguments()[0];
                    if (t == owner.GetType())
                    {
                        ILazyLoading ll = (ILazyLoading)f.GetValue(_Value);
                        ll.Write(owner, true);
                    }
                }
            }
        }

        public BelongsTo()
            : this(true)
        {
        }

        private BelongsTo(bool LazyLoad)
        {
            this.LazyLoad = LazyLoad;
        }

        private string GetKeyName()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(typeof(T));
            if (oi.KeyFields != null & oi.KeyFields.Length == 1)
            {
                return oi.KeyFields[0].Name;
            }
            throw new DbEntryException("The object must have one primary key.");
        }
    }
}
