using System;
using System.Text;
using Lephone.Data.Common;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectBase : IDbObject
    {
        public DbObjectBase()
        {
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
