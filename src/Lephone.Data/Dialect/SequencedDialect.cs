using System.Data;
using Lephone.Data.Common;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Dialect
{
    public abstract class SequencedDialect : DbDialect
    {
        protected override object ExecuteInsertIntKey(InsertStatementBuilder sb, ObjectInfo oi)
        {
            string seqStr = GetSelectSequenceSql(oi.From.MainTableName);
            var seq = new SqlStatement(CommandType.Text, seqStr);
            object key = oi.Context.ExecuteScalar(seq);
            sb.Values.Add(new KeyValue(oi.KeyFields[0].Name, key));
            SqlStatement sql = sb.ToSqlStatement(oi);
            oi.Context.ExecuteNonQuery(sql);
            return key;
        }

        public abstract string GetSelectSequenceSql(string tableName);
    }
}
