using System;
using System.Text;
using Lephone.Data.Model.Member;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectBase : IDbObject
    {
        public override string ToString()
        {
            var ctx = ModelContext.GetInstance(this.GetType());
            var sb = new StringBuilder("{ ");
            foreach (MemberHandler m in ctx.Info.Members)
            {
                if (!(m.Is.HasMany || m.Is.HasAndBelongsToMany || m.Is.HasOne))
                {
                    sb.Append(m.Name).Append(" = ");
                    object o = m.GetValue(this);
                    if (m.Is.BelongsTo)
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
