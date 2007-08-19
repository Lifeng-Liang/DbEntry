
#region usings

using System;
using System.Text;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public class DbObjectBase : IRenew
    {
        public DbObjectBase()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(this.GetType());
            if (oi.IsAssociateObject)
            {
                foreach (MemberHandler f in oi.Fields)
                {
                    if (f.IsHasOne || f.IsBelongsTo || f.IsHasMany || f.IsHasAndBelongsToMany)
                    {
                        object obj = f.GetValue(this);
                        if (obj == null)
                        {
                            ILazyLoading ll;
                            if (f.OrderByString == null)
                            {
                                ll = (ILazyLoading)ClassHelper.CreateInstance(f.FieldType);
                            }
                            else
                            {
                                ll = (ILazyLoading)ClassHelper.CreateInstance(f.FieldType, f.OrderByString);
                            }
                            f.SetValue(this, ll);
                            obj = ll;
                        }
                        ((ILazyLoading)obj).SetOwner(this, f.Name);
                        if (f.IsBelongsTo)
                        {
                            ((IBelongsTo)obj).ValueChanged += new CallbackObjectHandler<String>(m_ValueChanged);
                        }
                    }
                }
            }
        }

        protected virtual void m_ValueChanged(string s)
        {
        }

        void IRenew.SetAsNew()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(this.GetType());
            if (oi.HasOnePremarykey)
            {
                oi.KeyFields[0].SetValue(this, oi.KeyFields[0].UnsavedValue);
            }
            if (oi.IsAssociateObject)
            {
                foreach (MemberHandler f in oi.Fields)
                {
                    if (f.IsHasOne || f.IsHasMany)
                    {
                        object obj = f.GetValue(this);
                        if (obj == null)
                        {
                            IRenew ll = (IRenew)obj;
                            ll.SetAsNew();
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            ObjectInfo oi = DbObjectHelper.GetObjectInfo(this.GetType());
            StringBuilder sb = new StringBuilder("{ ");
            foreach (MemberHandler m in oi.Fields)
            {
                if (!(m.IsHasMany || m.IsHasAndBelongsToMany || m.IsHasOne))
                {
                    sb.Append(m.Name).Append(" = ");
                    object o = m.GetValue(this);
                    if (m.IsBelongsTo)
                    {
                        o = (o as IBelongsTo).ForeignKey;
                    }
                    sb.Append(o == null ? "<NULL>" : o.ToString());
                    sb.Append(", ");
                }
            }
            if (sb.Length > 2)
            {
                sb.Length -= 2;
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
