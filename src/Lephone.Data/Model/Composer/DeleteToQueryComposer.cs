using Lephone.Data.SqlEntry;
using Lephone.Data.Builder;

namespace Lephone.Data.Model.Composer
{
    internal class DeleteToQueryComposer : QueryComposer
    {
        public DeleteToQueryComposer(ModelContext ctx)
            : base(ctx)
        {
        }

        public override SqlStatement GetDeleteStatement(object obj)
        {
            SqlStatement sql = base.GetDeleteStatement(obj);
            InsertStatementBuilder sb = GetInsertStatementBuilder(obj);
            sb.Values.Add(new KeyValue("DeletedOn", AutoValue.DbNow));
            sb.TableName = Context.Info.DeleteToTableName;
            SqlStatement isql = sb.ToSqlStatement(Context);
            sql.SqlCommandText += isql.SqlCommandText;
            sql.Parameters.Add(isql.Parameters);
            return sql;
        }

        public override SqlStatement GetDeleteStatement(Condition iwc)
        {
            throw new DataException("DeleteTo class doesn't support delete by Condition.");
        }
    }
}
