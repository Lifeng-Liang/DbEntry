using System;
using System.Text;
using Lephone.Data.Common;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectBase : IRenew, IDbObject
    {
        public DbObjectBase()
        {
        }

        void IRenew.SetAsNew()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(this.GetType());
            if (oi.HasOnePremarykey)
            {
                oi.KeyFields[0].SetValue(this, oi.KeyFields[0].UnsavedValue);
            }
            if (oi.IsAssociateObject)
            {
                foreach (MemberHandler f in oi.RelationFields)
                {
                    if (f.IsHasOne || f.IsHasMany)
                    {
                        object obj = f.GetValue(this);
                        if (obj != null)
                        {
                            var ll = (IRenew)obj;
                            ll.SetAsNew();
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(this.GetType());
            var sb = new StringBuilder("{ ");
            foreach (MemberHandler m in oi.Fields)
            {
                if (!(m.IsHasMany || m.IsHasAndBelongsToMany || m.IsHasOne))
                {
                    sb.Append(m.Name).Append(" = ");
                    object o = m.GetValue(this);
                    if (m.IsBelongsTo)
                    {
                        o = ((IBelongsTo)o).ForeignKey;
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
