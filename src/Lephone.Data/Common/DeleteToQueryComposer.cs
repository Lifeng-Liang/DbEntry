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

        public override SqlStatement GetDeleteStatement(DbDialect Dialect, object obj)
        {
            SqlStatement sql = base.GetDeleteStatement(Dialect, obj);
            InsertStatementBuilder sb = GetInsertStatementBuilder(obj);
            sb.Values.Add(new KeyValue("DeletedOn", AutoValue.DbNow));
            sb.TableName = oi.DeleteToTableName;
            SqlStatement isql = sb.ToSqlStatement(Dialect);
            sql.SqlCommandText += isql.SqlCommandText;
            sql.Paramters.Add(isql.Paramters);
            return sql;
        }

        public override SqlStatement GetDeleteStatement(DbDialect Dialect, WhereCondition iwc)
        {
            throw new DataException("DeleteTo class doesn't support delete by WhereCondition.");
        }
    }
}
