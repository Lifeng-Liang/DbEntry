
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.Builder;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.util.Logging;

namespace org.hanzify.llf.Data.Dialect
{
    public abstract class SequencedDialect : DbDialect
    {
        public override object ExecuteInsert(DataProvider dp, InsertStatementBuilder sb, ObjectInfo oi)
        {
            string seqStr = GetSelectSequenceSql(oi.From.GetMainTableName());
            SqlStatement seq = new SqlStatement(CommandType.Text, seqStr);
            oi.LogSql(seq);
            object key = dp.ExecuteScalar(seq);
            sb.Values.Add(new KeyValue(oi.KeyFields[0].Name, key));
            SqlStatement sql = sb.ToSqlStatement(dp.Dialect);
            oi.LogSql(sql);
            dp.ExecuteNonQuery(sql);
            return key;
        }

        protected abstract string GetSelectSequenceSql(string TableName);
    }
}
