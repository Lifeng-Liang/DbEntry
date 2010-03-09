using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;

namespace Lephone.Data.Common
{
    internal class SoftDeleteQueryComposer : QueryComposer
    {
        private readonly string _columnName;
        private readonly KeyValueClause _colExp;

        public SoftDeleteQueryComposer(ObjectInfo oi, string columnName)
            : base(oi)
        {
            this._columnName = columnName;
            _colExp = (CK.K[columnName] == false);
        }

        public override CreateTableStatementBuilder GetCreateTableStatementBuilder()
        {
            CreateTableStatementBuilder cts = base.GetCreateTableStatementBuilder();
            cts.Columns.Add(new ColumnInfo(_columnName, typeof(bool), false, false, false, false, 0));
            return cts;
        }

        public override SqlStatement GetDeleteStatement(DbDialect dialect, object obj)
        {
            var sb = new UpdateStatementBuilder(Info.From.MainTableName);
            sb.Values.Add(new KeyValue(_columnName, true));
            sb.Where.Conditions = ObjectInfo.GetKeyWhereClause(obj) && _colExp;
            return sb.ToSqlStatement(dialect);
        }

        public override SqlStatement GetDeleteStatement(DbDialect dialect, Condition iwc)
        {
            return base.GetDeleteStatement(dialect, iwc && _colExp);
        }

        public override SqlStatement GetGroupByStatement(DbDialect dialect, Condition iwc, OrderBy order, string columnName)
        {
            return base.GetGroupByStatement(dialect, iwc && _colExp, order, columnName);
        }

        public override InsertStatementBuilder GetInsertStatementBuilder(object obj)
        {
            InsertStatementBuilder sb = base.GetInsertStatementBuilder(obj);
            sb.Values.Add(new KeyValue(_columnName, false));
            return sb;
        }

        public override SqlStatement GetResultCountStatement(DbDialect dialect, Condition iwc, bool isDistinct)
        {
            return base.GetResultCountStatement(dialect, iwc && _colExp, isDistinct);
        }

        public SqlStatement GetResultCountStatementWithoutDeleteCheck(DbDialect dialect, Condition iwc, bool isDistinct)
        {
            return base.GetResultCountStatement(dialect, iwc, isDistinct);
        }

        public override SqlStatement GetSelectStatement(DbDialect dialect, FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct)
        {
            return base.GetSelectStatement(dialect, from, iwc && _colExp, oc, lc, isDistinct);
        }

        public override SqlStatement GetUpdateStatement(DbDialect dialect, object obj, Condition iwc)
        {
            return base.GetUpdateStatement(dialect, obj, iwc && _colExp);
        }
    }
}
