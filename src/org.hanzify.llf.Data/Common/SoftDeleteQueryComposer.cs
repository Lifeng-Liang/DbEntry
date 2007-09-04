
using System;
using System.Collections.Generic;
using System.Text;
using org.hanzify.llf.Data.Builder;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.Builder.Clause;

namespace org.hanzify.llf.Data.Common
{
    internal class SoftDeleteQueryComposer : QueryComposer
    {
        private string ColumnName;
        private KeyValueClause colExp;

        public SoftDeleteQueryComposer(ObjectInfo oi, string ColumnName)
            : base(oi)
        {
            this.ColumnName = ColumnName;
            colExp = (CK.K[ColumnName] == false);
        }

        protected override CreateTableStatementBuilder GetCreateTableStatementBuilder()
        {
            CreateTableStatementBuilder cts = base.GetCreateTableStatementBuilder();
            cts.Columns.Add(new ColumnInfo(ColumnName, typeof(bool), false, false, false, false, 0));
            return cts;
        }

        public override SqlStatement GetDeleteStatement(DbDialect Dialect, object obj)
        {
            UpdateStatementBuilder sb = new UpdateStatementBuilder(oi.From.GetMainTableName());
            sb.Values.Add(new KeyValue(ColumnName, true));
            sb.Where.Conditions = colExp;
            return sb.ToSqlStatement(Dialect);
        }

        public override SqlStatement GetDeleteStatement(DbDialect Dialect, WhereCondition iwc)
        {
            return base.GetDeleteStatement(Dialect, iwc && colExp);
        }

        public override SqlStatement GetGroupByStatement(DbDialect Dialect, WhereCondition iwc, OrderBy order, string ColumnName)
        {
            return base.GetGroupByStatement(Dialect, iwc && colExp, order, ColumnName);
        }

        public override InsertStatementBuilder GetInsertStatementBuilder(object obj)
        {
            InsertStatementBuilder sb = base.GetInsertStatementBuilder(obj);
            sb.Values.Add(new KeyValue(ColumnName, false));
            return sb;
        }

        public override SqlStatement GetResultCountStatement(DbDialect Dialect, WhereCondition iwc)
        {
            return base.GetResultCountStatement(Dialect, iwc && colExp);
        }

        public override SqlStatement GetSelectStatement(DbDialect Dialect, FromClause from, WhereCondition iwc, OrderBy oc, Range lc)
        {
            return base.GetSelectStatement(Dialect, from, iwc && colExp, oc, lc);
        }

        public override SqlStatement GetUpdateStatement(DbDialect Dialect, object obj, WhereCondition iwc)
        {
            return base.GetUpdateStatement(Dialect, obj, iwc && colExp);
        }
    }
}
