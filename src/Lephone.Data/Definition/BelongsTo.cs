using System;
using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class BelongsTo<T> : IBelongsTo where T : class, IDbObject
    {
        private readonly object owner;
        private string ForeignKeyName;
        private object _ForeignKey;

        private DbContext context;
        private bool _IsLoaded;
        private T _Value;

        public event CallbackObjectHandler<string> ValueChanged;

        public BelongsTo(object owner)
        {
            this.owner = owner;
            ObjectInfo oi = ObjectInfo.GetInstance(owner.GetType());
            MemberHandler mh = oi.GetBelongsTo(typeof(T));
            ForeignKeyName = mh.Name;
            ObjectInfo oi1 = ObjectInfo.GetInstance(typeof(T));
            _ForeignKey = oi1.GetPrimaryKeyDefaultValue();
            //_ForeignKey = Info.GetPrimaryKeyDefaultValue();
            DbObjectSmartUpdate o = owner as DbObjectSmartUpdate;
            if (o != null)
            {
                ValueChanged += o.m_ColumnUpdated;
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
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            if (oi.KeyFields != null & oi.KeyFields.Length == 1)
            {
                _Value = (T)item;
                _ForeignKey = (item == null) ? CommonHelper.GetEmptyValue(oi.KeyFields[0].FieldType) : oi.KeyFields[0].GetValue(item);
                _IsLoaded = true;
                context = null;
                if (ValueChanged != null && !IsLoad)
                {
                    ValueChanged(ForeignKeyName);
                }
            }
            else
            {
                throw new DataException("The object must have one key.");
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
            if (_Value != null)
            {
                ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
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
        }

        //TODO: why left this?
        //private string GetKeyName()
        //{
        //    ObjectInfo Info = ObjectInfo.GetInstance(typeof(T));
        //    if (Info.KeyFields != null & Info.KeyFields.Length == 1)
        //    {
        //        return Info.KeyFields[0].Name;
        //    }
        //    throw new DataException("The object must have one primary key.");
        //}
    }
}
