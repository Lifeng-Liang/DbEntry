using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;

namespace Lephone.Data.Common
{
    internal class QueryComposer
    {
        protected ObjectInfo oi;

        public QueryComposer(ObjectInfo oi)
        {
            this.oi = oi;
        }

        public virtual SqlStatement GetResultCountStatement(DbDialect Dialect, WhereCondition iwc)
        {
            var sb = new SelectStatementBuilder(oi.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetCountColumn("*");
            return sb.ToSqlStatement(Dialect);
        }

        public virtual SqlStatement GetGroupByStatement(DbDialect dialect, WhereCondition iwc, OrderBy order, string columnName)
        {
            var sb = new SelectStatementBuilder(oi.From, order, null);
            sb.Where.Conditions = iwc;
            sb.Keys.Add(columnName);
            sb.SetAsGroupBy(columnName);
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetSelectStatement(DbDialect Dialect, FromClause from, WhereCondition iwc, OrderBy oc, Range lc)
        {
            var sb = new SelectStatementBuilder(from ?? oi.From, oc, lc);
            sb.Where.Conditions = iwc;
            oi.Handler.SetValuesForSelect(sb);
            // DataBase Process
            return sb.ToSqlStatement(Dialect);
        }

        public virtual SqlStatement GetInsertStatement(DbDialect Dialect, object obj)
        {
            InsertStatementBuilder sb = GetInsertStatementBuilder(obj);
            return sb.ToSqlStatement(Dialect);
        }

        public virtual InsertStatementBuilder GetInsertStatementBuilder(object obj)
        {
            var sb = new InsertStatementBuilder(oi.From.GetMainTableName());
            oi.Handler.SetValuesForInsert(sb, obj);
            return sb;
        }

        public virtual SqlStatement GetUpdateStatement(DbDialect Dialect, object obj, WhereCondition iwc)
        {
            var sb = new UpdateStatementBuilder(oi.From.GetMainTableName());
            oi.Handler.SetValuesForUpdate(sb, obj);
            sb.Where.Conditions = iwc;
            return sb.ToSqlStatement(Dialect);
        }

        public virtual SqlStatement GetDeleteStatement(DbDialect Dialect, object obj)
        {
            var sb = new DeleteStatementBuilder(oi.From.GetMainTableName());
            sb.Where.Conditions = ObjectInfo.GetKeyWhereClause(obj);
            return sb.ToSqlStatement(Dialect);
        }

        public virtual SqlStatement GetDeleteStatement(DbDialect Dialect, WhereCondition iwc)
        {
            var sb = new DeleteStatementBuilder(oi.From.GetMainTableName());
            sb.Where.Conditions = iwc;
            return sb.ToSqlStatement(Dialect);
        }

        public virtual SqlStatement GetCreateStatement(DbDialect Dialect)
        {
            CreateTableStatementBuilder cts = GetCreateTableStatementBuilder();
            return cts.ToSqlStatement(Dialect);
        }

        public virtual CreateTableStatementBuilder GetCreateTableStatementBuilder()
        {
            string tname = oi.From.GetMainTableName();
            var cts = new CreateTableStatementBuilder(tname);
            foreach (MemberHandler fh in oi.Fields)
            {
                if (!fh.IsHasMany && !fh.IsHasOne && !fh.IsHasAndBelongsToMany)
                {
                    var ci = new ColumnInfo(fh);
                    cts.Columns.Add(ci);
                }
            }
            foreach (string s in oi.Indexes.Keys)
            {
                bool u = oi.UniqueIndexes.ContainsKey(s) ? true : false;
                cts.Indexes.Add(new DbIndex(s, u, oi.Indexes[s].ToArray()));
            }
            return cts;
        }
    }
}
