using System.Data;
using Lephone.Data.Builder;
using Lephone.Data.Model;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Dialect
{
    public abstract class SequencedDialect : DbDialect
    {
        protected override object ExecuteInsertIntKey(InsertStatementBuilder sb, ObjectInfo info, DataProvider provider)
        {
            string seqStr = GetSelectSequenceSql(info.From.MainTableName);
            var seq = new SqlStatement(CommandType.Text, seqStr);
            object key = provider.ExecuteScalar(seq);
            sb.Values.Add(new KeyValue(info.KeyMembers[0].Name, key));
            SqlStatement sql = sb.ToSqlStatement(provider.Dialect, null, info.AllowSqlLog);
            provider.ExecuteNonQuery(sql);
            return key;
        }

        public abstract string GetSelectSequenceSql(string tableName);
    }
}
