using Leafing.Data.Builder.Clause;
using Leafing.Data.SqlEntry;
using Leafing.Data.Builder;

namespace Leafing.Data.Model.Composer {
    internal class DeleteToQueryComposer : QueryComposer {
        public DeleteToQueryComposer(ModelContext ctx)
            : base(ctx) {
        }

        public override SqlStatement GetDeleteStatement(object obj) {
            SqlStatement sql = base.GetDeleteStatement(obj);
            InsertStatementBuilder sb = GetInsertStatementBuilder(obj);
            sb.Values.Add(new KeyOpValue("DeletedOn", null, KvOpertation.Now));
            sb.From = new FromClause(Context.Info.DeleteToTableName);
            SqlStatement isql = sb.ToSqlStatement(Context);
            sql.SqlCommandText += isql.SqlCommandText;
            sql.Parameters.Add(isql.Parameters);
            return sql;
        }

        public override SqlStatement GetDeleteStatement(Condition iwc) {
            throw new DataException("DeleteTo class doesn't support delete by Condition.");
        }
    }
}
