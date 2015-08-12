using System;
using System.Collections.Generic;
using Leafing.Data.Builder;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Definition
{
    public class LazyLoad<T> : LazyLoadOneBase<T>
    {
        public LazyLoad(DbObjectSmartUpdate owner, string relationName)
            : base(owner, relationName)
        {
        }

        protected override void DoLoad()
        {
            var ctx = Owner.Context;
            object key = ctx.Handler.GetKeyValue(Owner);
            var sb = new SelectStatementBuilder(ctx.Info.From, null, null);
            string kn = ctx.Info.KeyMembers[0].Name;
            sb.Where.Conditions = CK.K[kn] == key;
            sb.Keys.Add(new KeyValuePair<string, string>(RelationName, null));
            SqlStatement sql = sb.ToSqlStatement(ctx);
            object o = ctx.Provider.ExecuteScalar(sql);
            if (o == DBNull.Value)
            {
                o = null;
            }
            m_Value = (T)o;
        }
    }
}
