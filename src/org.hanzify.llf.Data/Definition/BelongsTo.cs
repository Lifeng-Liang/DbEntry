
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
        private string ForeignKeyName;
        private object _ForeignKey;

        private DbContext context;
        private bool _IsLoaded;
        private T _Value;

        public event CallbackObjectHandler<string> ValueChanged;

        public BelongsTo(object owner)
        {
            this.owner = owner;
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(owner.GetType());
            MemberHandler mh = oi.GetBelongsTo(typeof(T));
            ForeignKeyName = mh.Name;
            ObjectInfo oi1 = DbObjectHelper.GetObjectInfo(typeof(T));
            _ForeignKey = oi1.GetPrimaryKeyDefaultValue();
            //_ForeignKey = oi.GetPrimaryKeyDefaultValue();
            DbObjectSmartUpdate o = owner as DbObjectSmartUpdate;
            if (o != null)
            {
                ValueChanged += new CallbackObjectHandler<string>(o.m_ColumnUpdated);
            }
        }

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
                context = null;
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
                context = null;
                if (ValueChanged != null && !IsLoad)
                {
                    ValueChanged(ForeignKeyName);
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

        void ILazyLoading.Init(DbContext driver, string ForeignKeyName)
        {
            this.context = driver;
            this.ForeignKeyName = ForeignKeyName;
        }

        void ILazyLoading.Load()
        {
            _Value = context.GetObject<T>(_ForeignKey);
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
