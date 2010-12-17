using System;
using System.Collections.Generic;
using Lephone.Data.Common;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Definition
{
    public class LazyLoadField<T> : LazyLoadOneBase<T>
    {
        public LazyLoadField(object owner, string relationName)
            : base(owner, relationName)
        {
        }

        protected override void DoLoad()
        {
            ObjectInfo oi = ObjectInfo.GetInstance(Owner.GetType());
            object key = oi.Handler.GetKeyValue(Owner);
            var sb = new SelectStatementBuilder(oi.From, null, null);
            string kn = oi.KeyFields[0].Name;
            sb.Where.Conditions = CK.K[kn] == key;
            sb.Keys.Add(new KeyValuePair<string, string>(RelationName, null));
            SqlStatement sql = sb.ToSqlStatement(oi.Context.Dialect);
            oi.LogSql(sql);
            object o = oi.Context.ExecuteScalar(sql);
            if (o == DBNull.Value)
            {
                o = null;
            }
            m_Value = (T)o;
        }
    }
}
