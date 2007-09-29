
using System;
using System.Collections.Generic;
using System.Text;
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
            SelectStatementBuilder sb = new SelectStatementBuilder(oi.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetCountColumn("*");
            return sb.ToSqlStatement(Dialect);
        }

        public virtual SqlStatement GetGroupByStatement(DbDialect Dialect, WhereCondition iwc, OrderBy order, string ColumnName)
        {
            SelectStatementBuilder sb = new SelectStatementBuilder(oi.From, order, null);
            sb.Where.Conditions = iwc;
            sb.Keys.Add(ColumnName);
            sb.SetAsGroupBy(ColumnName);
            return sb.ToSqlStatement(Dialect);
        }

        public virtual SqlStatement GetSelectStatement(DbDialect Dialect, FromClause from, WhereCondition iwc, OrderBy oc, Range lc)
        {
            SelectStatementBuilder sb = new SelectStatementBuilder(from != null ? from : oi.From, oc, lc);
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
            InsertStatementBuilder sb = new InsertStatementBuilder(oi.From.GetMainTableName());
            oi.Handler.SetValuesForInsert(sb, obj);
            return sb;
        }

        public virtual SqlStatement GetUpdateStatement(DbDialect Dialect, object obj, WhereCondition iwc)
        {
            UpdateStatementBuilder sb = new UpdateStatementBuilder(oi.From.GetMainTableName());
            oi.Handler.SetValuesForUpdate(sb, obj);
            sb.Where.Conditions = iwc;
            return sb.ToSqlStatement(Dialect);
        }

        public virtual SqlStatement GetDeleteStatement(DbDialect Dialect, object obj)
        {
            DeleteStatementBuilder sb = new DeleteStatementBuilder(oi.From.GetMainTableName());
            sb.Where.Conditions = DbObjectHelper.GetKeyWhereClause(obj);
            return sb.ToSqlStatement(Dialect);
        }

        public virtual SqlStatement GetDeleteStatement(DbDialect Dialect, WhereCondition iwc)
        {
            DeleteStatementBuilder sb = new DeleteStatementBuilder(oi.From.GetMainTableName());
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
            CreateTableStatementBuilder cts = new CreateTableStatementBuilder(tname);
            foreach (MemberHandler fh in oi.Fields)
            {
                if (!fh.IsHasMany && !fh.IsHasOne && !fh.IsHasAndBelongsToMany)
                {
                    ColumnInfo ci = new ColumnInfo(fh);
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
