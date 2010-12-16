using System;
using Lephone.Data.Common;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class BelongsTo<T, TKey> : LazyLoadOneBase<T>, IBelongsTo where T : DbObjectModel<T, TKey>, new() where TKey : struct
    {
        private readonly DbObjectSmartUpdate _osu;
        private TKey _foreignKey;

        public BelongsTo(object owner, string relationName)
            : base(owner, relationName)
        {
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
            set
            {
                _foreignKey = (TKey)Convert.ChangeType(value, typeof(TKey));
            }
        }

        protected override void DoWrite(object oldValue, bool isLoad)
        {
            var oi = ObjectInfo.GetInstance(typeof(T));
            if (oi.HasOnePrimaryKey)
            {
                if (m_Value != null)
                {
                    _foreignKey = (TKey)oi.KeyFields[0].GetValue(m_Value);
                }
                else
                {
                    _foreignKey = default(TKey);
                }
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
            var oi = ObjectInfo.GetInstance(Owner.GetType());
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
