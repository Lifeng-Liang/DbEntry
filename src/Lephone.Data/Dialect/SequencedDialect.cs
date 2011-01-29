using System.Data;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Dialect
{
    public abstract class SequencedDialect : DbDialect
    {
        protected override object ExecuteInsertIntKey(InsertStatementBuilder sb, ModelContext ctx)
        {
            string seqStr = GetSelectSequenceSql(ctx.Info.From.MainTableName);
            var seq = new SqlStatement(CommandType.Text, seqStr);
            object key = ctx.Provider.ExecuteScalar(seq);
            sb.Values.Add(new KeyValue(ctx.Info.KeyMembers[0].Name, key));
            SqlStatement sql = sb.ToSqlStatement(ctx);
            ctx.Provider.ExecuteNonQuery(sql);
            return key;
        }

        public abstract string GetSelectSequenceSql(string tableName);
    }
}
