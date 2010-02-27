using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;

namespace Lephone.Data.Common
{
    internal class SoftDeleteQueryComposer : QueryComposer
    {
        private readonly string ColumnName;
        private readonly KeyValueClause colExp;

        public SoftDeleteQueryComposer(ObjectInfo oi, string ColumnName)
            : base(oi)
        {
            this.ColumnName = ColumnName;
            colExp = (CK.K[ColumnName] == false);
        }

        public override CreateTableStatementBuilder GetCreateTableStatementBuilder()
        {
            CreateTableStatementBuilder cts = base.GetCreateTableStatementBuilder();
            cts.Columns.Add(new ColumnInfo(ColumnName, typeof(bool), false, false, false, false, 0));
            return cts;
        }

        public override SqlStatement GetDeleteStatement(DbDialect dialect, object obj)
        {
            var sb = new UpdateStatementBuilder(Info.From.MainTableName);
            sb.Values.Add(new KeyValue(ColumnName, true));
            sb.Where.Conditions = ObjectInfo.GetKeyWhereClause(obj) && colExp;
            return sb.ToSqlStatement(dialect);
        }

        public override SqlStatement GetDeleteStatement(DbDialect dialect, Condition iwc)
        {
            return base.GetDeleteStatement(dialect, iwc && colExp);
        }

        public override SqlStatement GetGroupByStatement(DbDialect dialect, Condition iwc, OrderBy order, string columnName)
        {
            return base.GetGroupByStatement(dialect, iwc && colExp, order, columnName);
        }

        public override InsertStatementBuilder GetInsertStatementBuilder(object obj)
        {
            InsertStatementBuilder sb = base.GetInsertStatementBuilder(obj);
            sb.Values.Add(new KeyValue(ColumnName, false));
            return sb;
        }

        public override SqlStatement GetResultCountStatement(DbDialect dialect, Condition iwc, bool isDistinct)
        {
            return base.GetResultCountStatement(dialect, iwc && colExp, isDistinct);
        }

        public override SqlStatement GetSelectStatement(DbDialect dialect, FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct)
        {
            return base.GetSelectStatement(dialect, from, iwc && colExp, oc, lc, isDistinct);
        }

        public override SqlStatement GetUpdateStatement(DbDialect dialect, object obj, Condition iwc)
        {
            return base.GetUpdateStatement(dialect, obj, iwc && colExp);
        }
    }
}
