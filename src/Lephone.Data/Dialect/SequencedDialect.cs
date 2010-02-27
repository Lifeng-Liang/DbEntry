using System.Data;
using Lephone.Data.Common;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Dialect
{
    public abstract class SequencedDialect : DbDialect
    {
        protected override object ExecuteInsertIntKey(DataProvider dp, InsertStatementBuilder sb, ObjectInfo oi)
        {
            string seqStr = GetSelectSequenceSql(oi.From.MainTableName);
            var seq = new SqlStatement(CommandType.Text, seqStr);
            oi.LogSql(seq);
            object key = dp.ExecuteScalar(seq);
            sb.Values.Add(new KeyValue(oi.KeyFields[0].Name, key));
            SqlStatement sql = sb.ToSqlStatement(dp.Dialect);
            oi.LogSql(sql);
            dp.ExecuteNonQuery(sql);
            return key;
        }

        public abstract string GetSelectSequenceSql(string tableName);
    }
}
