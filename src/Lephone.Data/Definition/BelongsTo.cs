using System;
using Lephone.Data.Model.Member;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class BelongsTo<T, TKey> : LazyLoadOneBase<T>, IBelongsTo where T : DbObjectModel<T, TKey>, new() where TKey : struct
    {
        private TKey? _foreignKey;

        public BelongsTo(DbObjectSmartUpdate owner, string relationName)
            : base(owner, relationName)
        {
        }

        public void ForeignKeyChanged()
        {
            Owner.m_ColumnUpdated(RelationName);
        }

        public object ForeignKey
        {
            get { return _foreignKey; }
            set
            {
                if (value == null || value == DBNull.Value)
                {
                    _foreignKey = null;
                }
                else
                {
                    _foreignKey = (TKey)Convert.ChangeType(value, typeof(TKey));
                }
            }
        }

        protected override void DoWrite(object oldValue, bool isLoad)
        {
            var ctx = ModelContext.GetInstance(typeof(T));
            if (ctx.Info.HasOnePrimaryKey)
            {
                if (m_Value != null)
                {
                    _foreignKey = (TKey)ctx.Info.KeyMembers[0].GetValue(m_Value);
                }
                else
                {
                    _foreignKey = null;
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
                        foreach (var mh in ctx.Info.RelationMembers)
                        {
                            if (mh.Is.HasOne || mh.Is.HasMany)
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
            m_Value = DbEntry.GetObject<T>(_foreignKey);
            if (m_Value != null)
            {
                var ctx1 = ModelContext.GetInstance(typeof(T));
                foreach (MemberHandler f in ctx1.Info.RelationMembers)
                {
                    if (f.Is.HasOne || f.Is.HasMany)
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
