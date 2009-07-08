using System.Collections.Generic;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.Dialect;
using Lephone.Data.Builder.Clause;

namespace Lephone.Data.Common
{
    internal class QueryComposer
    {
        protected ObjectInfo Info;

        public QueryComposer(ObjectInfo oi)
        {
            Info = oi;
        }

        public virtual SqlStatement GetMaxStatement(DbDialect dialect, WhereCondition iwc, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetMaxColumn(columnName);
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetMinStatement(DbDialect dialect, WhereCondition iwc, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetMinColumn(columnName);
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetSumStatement(DbDialect dialect, WhereCondition iwc, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetSumColumn(columnName);
            return sb.ToSqlStatement(dialect);
        }

        public SqlStatement GetResultCountStatement(DbDialect dialect, WhereCondition iwc)
        {
            return GetResultCountStatement(dialect, iwc, false);
        }

        public virtual SqlStatement GetResultCountStatement(DbDialect dialect, WhereCondition iwc, bool isDistinct)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null) {IsDistinct = isDistinct};
            sb.Where.Conditions = iwc;
            if(isDistinct)
            {
                Info.Handler.SetValuesForSelect(sb);
                string cs = sb.GetColumns(dialect, true, false);
                sb.SetCountColumn(cs);
                sb.IsDistinct = false;
                sb.Keys.Clear();
            }
            else
            {
                sb.SetCountColumn("*");
            }
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetGroupByStatement(DbDialect dialect, WhereCondition iwc, OrderBy order, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, order, null);
            sb.Where.Conditions = iwc;
            var list = columnName.Split(',');
            foreach (string s in list)
            {
                sb.Keys.Add(new KeyValuePair<string, string>(s, null));
                sb.SetAsGroupBy(s);
            }
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetSelectStatement(DbDialect dialect, FromClause from, WhereCondition iwc, OrderBy oc, Range lc, bool isDistinct)
        {
            var sb = new SelectStatementBuilder(from ?? Info.From, oc, lc) {IsDistinct = isDistinct};
            sb.Where.Conditions = iwc;
            Info.Handler.SetValuesForSelect(sb);
            // DataBase Process
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetInsertStatement(DbDialect dialect, object obj)
        {
            InsertStatementBuilder sb = GetInsertStatementBuilder(obj);
            return sb.ToSqlStatement(dialect);
        }

        public virtual InsertStatementBuilder GetInsertStatementBuilder(object obj)
        {
            var sb = new InsertStatementBuilder(Info.From.GetMainTableName());
            Info.Handler.SetValuesForInsert(sb, obj);
            return sb;
        }

        public virtual SqlStatement GetUpdateStatement(DbDialect dialect, object obj, WhereCondition iwc)
        {
            var sb = new UpdateStatementBuilder(Info.From.GetMainTableName());
            Info.Handler.SetValuesForUpdate(sb, obj);
            sb.Where.Conditions = iwc;
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetDeleteStatement(DbDialect dialect, object obj)
        {
            var sb = new DeleteStatementBuilder(Info.From.GetMainTableName());
            sb.Where.Conditions = ObjectInfo.GetKeyWhereClause(obj);
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetDeleteStatement(DbDialect dialect, WhereCondition iwc)
        {
            var sb = new DeleteStatementBuilder(Info.From.GetMainTableName());
            sb.Where.Conditions = iwc;
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetCreateStatement(DbDialect dialect)
        {
            CreateTableStatementBuilder cts = GetCreateTableStatementBuilder();
            return cts.ToSqlStatement(dialect);
        }

        public virtual CreateTableStatementBuilder GetCreateTableStatementBuilder()
        {
            string tname = Info.From.GetMainTableName();
            var cts = new CreateTableStatementBuilder(tname);
            foreach (MemberHandler fh in Info.Fields)
            {
                if (!fh.IsHasMany && !fh.IsHasOne && !fh.IsHasAndBelongsToMany)
                {
                    var ci = new ColumnInfo(fh);
                    cts.Columns.Add(ci);
                }
            }
            foreach (string s in Info.Indexes.Keys)
            {
                bool u = Info.UniqueIndexes.ContainsKey(s) ? true : false;
                cts.Indexes.Add(new DbIndex(s, u, Info.Indexes[s].ToArray()));
            }
            return cts;
        }

        public virtual void ProcessAfterSave(object obj) {}
    }
}
