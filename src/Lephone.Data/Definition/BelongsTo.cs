using System;
using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class BelongsTo<T> : LazyLoadOneBase<T>, IBelongsTo where T : class, IDbObject
    {
        private readonly DbObjectSmartUpdate _osu;
        private object _foreignKey;
        private ObjectInfo oi;

        public BelongsTo(object owner) : base(owner)
        {
            oi = ObjectInfo.GetInstance(owner.GetType());
            MemberHandler mh = oi.GetBelongsTo(typeof(T));
            RelationName = mh.Name;
            var oi1 = ObjectInfo.GetInstance(typeof(T));
            _foreignKey = oi1.GetPrimaryKeyDefaultValue();
            _osu = owner as DbObjectSmartUpdate;
        }

        public void ForeignKeyChanged()
        {
            if (_osu != null)
            {
                _osu.m_ColumnUpdated(RelationName);
            }
        }

        public object ForeignKey
        {
            get { return _foreignKey; }
            set { _foreignKey = value; }
        }

        protected override void DoWrite(object oldValue, bool isLoad)
        {
            var oi = ObjectInfo.GetInstance(typeof(T));
            if (oi.HasOnePrimaryKey)
            {
                _foreignKey = (m_Value == null) ? CommonHelper.GetEmptyValue(oi.KeyFields[0].FieldType) : oi.KeyFields[0].GetValue(m_Value);
                if (!isLoad)
                {
                    ForeignKeyChanged();
                    if(m_Value == null && oldValue != null)
                    {
                        // TODO: remove oldValue
                    }
                    else if (m_Value != null && m_Value != oldValue)
                    {
                        foreach (var mh in oi.RelationFields)
                        {
                            if (mh.IsHasOne || mh.IsHasMany)
                            {
                                Type st = mh.FieldType.GetGenericArguments()[0];
                                st = st.IsAbstract ? AssemblyHandler.Instance.GetImplType(st) : st;
                                Type ot = Owner.GetType();
                                if (st == ot)
                                {
                                    var ll = (ILazyLoading)mh.GetValue(m_Value);
                                    if(!ll.IsLoaded)
                                    {
                                        ll.Write(Owner, true);
                                        ll.IsLoaded = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                throw new DataException("The object must have one key.");
            }
        }

        protected override void DoLoad()
        {
            m_Value = oi.Context.GetObject<T>(_foreignKey);
            if (m_Value != null)
            {
                var oi1 = ObjectInfo.GetInstance(typeof(T));
                foreach (MemberHandler f in oi1.RelationFields)
                {
                    if (f.IsHasOne || f.IsHasMany)
                    {
                        Type t = f.FieldType.GetGenericArguments()[0];
                        if (t == Owner.GetType())
                        {
                            var ll = (ILazyLoading)f.GetValue(m_Value);
                            ll.Write(Owner, true);
                        }
                    }
                }
            }
        }
    }
}
