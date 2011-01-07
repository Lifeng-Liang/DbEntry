using System;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
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
            cts.Columns.Add(new ColumnInfo(_columnName, typeof(bool), false, false, false, false, 0, 0));
            return cts;
        }

        public override SqlStatement GetDeleteStatement(object obj)
        {
            var sb = new UpdateStatementBuilder(Info.From.MainTableName);
            sb.Values.Add(new KeyValue(_columnName, true));
            sb.Where.Conditions = ObjectInfo.GetKeyWhereClause(obj) && _colExp;
            return sb.ToSqlStatement(Info);
        }

        public override SqlStatement GetDeleteStatement(Condition iwc)
        {
            return base.GetDeleteStatement(iwc && _colExp);
        }

        public override SqlStatement GetGroupByStatement(Condition iwc, OrderBy order, string columnName)
        {
            return base.GetGroupByStatement(iwc && _colExp, order, columnName);
        }

        public override InsertStatementBuilder GetInsertStatementBuilder(object obj)
        {
            InsertStatementBuilder sb = base.GetInsertStatementBuilder(obj);
            sb.Values.Add(new KeyValue(_columnName, false));
            return sb;
        }

        public override SqlStatement GetResultCountStatement(Condition iwc, bool isDistinct)
        {
            return base.GetResultCountStatement(iwc && _colExp, isDistinct);
        }

        public SqlStatement GetResultCountStatementWithoutDeleteCheck(Condition iwc, bool isDistinct)
        {
            return base.GetResultCountStatement(iwc, isDistinct);
        }

        public override SelectStatementBuilder GetSelectStatementBuilder(FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct, bool noLazy, Type returnType, string colName)
        {
            return base.GetSelectStatementBuilder(from, iwc && _colExp, oc, lc, isDistinct, noLazy, returnType, colName);
        }

        public override SqlStatement GetUpdateStatement(object obj, Condition iwc)
        {
            return base.GetUpdateStatement(obj, iwc && _colExp);
        }
    }
}
