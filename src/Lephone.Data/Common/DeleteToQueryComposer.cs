using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder;

namespace Lephone.Data.Common
{
    internal class DeleteToQueryComposer : QueryComposer
    {
        public DeleteToQueryComposer(ObjectInfo oi) : base(oi)
        {
        }

        public override SqlStatement GetDeleteStatement(DbDialect dialect, object obj)
        {
            SqlStatement sql = base.GetDeleteStatement(dialect, obj);
            InsertStatementBuilder sb = GetInsertStatementBuilder(obj);
            sb.Values.Add(new KeyValue("DeletedOn", AutoValue.DbNow));
            sb.TableName = Info.DeleteToTableName;
            SqlStatement isql = sb.ToSqlStatement(dialect);
            sql.SqlCommandText += isql.SqlCommandText;
            sql.Parameters.Add(isql.Parameters);
            return sql;
        }

        public override SqlStatement GetDeleteStatement(DbDialect dialect, WhereCondition iwc)
        {
            throw new DataException("DeleteTo class doesn't support delete by WhereCondition.");
        }
    }
}
