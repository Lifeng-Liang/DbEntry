using System;
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

        public virtual SqlStatement GetMaxStatement(DbDialect dialect, Condition iwc, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetMaxColumn(columnName);
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetMinStatement(DbDialect dialect, Condition iwc, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetMinColumn(columnName);
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetSumStatement(DbDialect dialect, Condition iwc, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetSumColumn(columnName);
            return sb.ToSqlStatement(dialect);
        }

        public SqlStatement GetResultCountStatement(DbDialect dialect, Condition iwc)
        {
            return GetResultCountStatement(dialect, iwc, false);
        }

        public virtual SqlStatement GetResultCountStatement(DbDialect dialect, Condition iwc, bool isDistinct)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null) {IsDistinct = isDistinct};
            sb.Where.Conditions = iwc;
            if(isDistinct)
            {
                Info.Handler.SetValuesForSelect(sb, false);
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

        public virtual SqlStatement GetGroupByStatement(DbDialect dialect, Condition iwc, OrderBy order, string columnName)
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

        public virtual SqlStatement GetGroupBySumStatement(DbDialect dialect, Condition iwc, OrderBy order, string groupbyColumnName, string sumColumnName)
        {
            var sb = new SelectStatementBuilder(Info.From, order, null);
            sb.Where.Conditions = iwc;
            var list = groupbyColumnName.Split(',');
            foreach (string s in list)
            {
                sb.Keys.Add(new KeyValuePair<string, string>(s, null));
                sb.SetAsGroupBySum(groupbyColumnName, sumColumnName);
            }
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetSelectStatement(DbDialect dialect, FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct, bool noLazy, Type returnType)
        {
            var sb = new SelectStatementBuilder(from ?? Info.From, oc, lc) { IsDistinct = isDistinct, NoLazy = noLazy, };
            sb.Where.Conditions = iwc;
            if(returnType.Name.StartsWith("<"))
            {
                SetSelectColumnsForDynamicLinqObject(sb, returnType);
            }
            else
            {
                Info.Handler.SetValuesForSelect(sb, noLazy);
            }
            // DataBase Process
            return sb.ToSqlStatement(dialect);
        }

        private void SetSelectColumnsForDynamicLinqObject(SelectStatementBuilder sb, Type returnType)
        {
            var handler = DynamicLinqObjectHandler.Factory.GetInstance(returnType);
            handler.Init(Info);
            foreach (MemberHandler fi in handler.GetMembers())
            {
                string value = null;
                if (fi.Name != fi.MemberInfo.Name)
                {
                    value = fi.MemberInfo.Name;
                }
                sb.Keys.Add(new KeyValuePair<string, string>(fi.Name, value));
            }
        }

        public virtual SqlStatement GetInsertStatement(DbDialect dialect, object obj)
        {
            InsertStatementBuilder sb = GetInsertStatementBuilder(obj);
            return sb.ToSqlStatement(dialect);
        }

        public virtual InsertStatementBuilder GetInsertStatementBuilder(object obj)
        {
            var sb = new InsertStatementBuilder(Info.From.MainTableName);
            Info.Handler.SetValuesForInsert(sb, obj);
            return sb;
        }

        public virtual SqlStatement GetUpdateStatement(DbDialect dialect, object obj, Condition iwc)
        {
            var sb = new UpdateStatementBuilder(Info.From.MainTableName);
            Info.Handler.SetValuesForUpdate(sb, obj);
            sb.Where.Conditions = iwc;
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetDeleteStatement(DbDialect dialect, object obj)
        {
            var sb = new DeleteStatementBuilder(Info.From.MainTableName);
            sb.Where.Conditions = ObjectInfo.GetKeyWhereClause(obj);
            return sb.ToSqlStatement(dialect);
        }

        public virtual SqlStatement GetDeleteStatement(DbDialect dialect, Condition iwc)
        {
            var sb = new DeleteStatementBuilder(Info.From.MainTableName);
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
            string tname = Info.From.MainTableName;
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
