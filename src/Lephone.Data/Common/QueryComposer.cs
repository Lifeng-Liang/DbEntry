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
            if(oi == null)
            {
                throw new ArgumentNullException("oi");
            }
            Info = oi;
        }

        public virtual SqlStatement GetMaxStatement(Condition iwc, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetMaxColumn(columnName);
            return sb.ToSqlStatement(Info);
        }

        public virtual SqlStatement GetMinStatement(Condition iwc, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetMinColumn(columnName);
            return sb.ToSqlStatement(Info);
        }

        public virtual SqlStatement GetSumStatement(Condition iwc, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null);
            sb.Where.Conditions = iwc;
            sb.SetSumColumn(columnName);
            return sb.ToSqlStatement(Info);
        }

        public SqlStatement GetResultCountStatement(Condition iwc)
        {
            return GetResultCountStatement(iwc, false);
        }

        public virtual SqlStatement GetResultCountStatement(Condition iwc, bool isDistinct)
        {
            var sb = new SelectStatementBuilder(Info.From, null, null) {IsDistinct = isDistinct};
            sb.Where.Conditions = iwc;
            if(isDistinct)
            {
                Info.Handler.SetValuesForSelect(sb, false);
                string cs = sb.GetColumns(Info.Context.Dialect, true, false);
                sb.SetCountColumn(cs);
                sb.IsDistinct = false;
                sb.Keys.Clear();
            }
            else
            {
                sb.SetCountColumn("*");
            }
            return sb.ToSqlStatement(Info);
        }

        public virtual SqlStatement GetGroupByStatement(Condition iwc, OrderBy order, string columnName)
        {
            var sb = new SelectStatementBuilder(Info.From, order, null);
            sb.Where.Conditions = iwc;
            var list = columnName.Split(',');
            foreach (string s in list)
            {
                sb.Keys.Add(new KeyValuePair<string, string>(s, null));
                sb.SetAsGroupBy(s);
            }
            return sb.ToSqlStatement(Info);
        }

        public virtual SqlStatement GetGroupBySumStatement(Condition iwc, OrderBy order, string groupbyColumnName, string sumColumnName)
        {
            var sb = new SelectStatementBuilder(Info.From, order, null);
            sb.Where.Conditions = iwc;
            var list = groupbyColumnName.Split(',');
            foreach (string s in list)
            {
                sb.Keys.Add(new KeyValuePair<string, string>(s, null));
                sb.SetAsGroupBySum(groupbyColumnName, sumColumnName);
            }
            return sb.ToSqlStatement(Info);
        }

        public SqlStatement GetSelectStatement(FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct, bool noLazy, Type returnType)
        {
            var sb = GetSelectStatementBuilder(from, iwc, oc, lc, isDistinct, noLazy, returnType, null);
            return sb.ToSqlStatement(Info);
        }

        public virtual SelectStatementBuilder GetSelectStatementBuilder(FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct, bool noLazy, Type returnType, string colName)
        {
            var sb = new SelectStatementBuilder(from ?? Info.From, oc, lc) { IsDistinct = isDistinct, NoLazy = noLazy, };
            sb.Where.Conditions = iwc;
            if (!colName.IsNullOrEmpty())
            {
                sb.Keys.Add(new KeyValuePair<string, string>(colName, null));
            }
            else if (returnType.Name.StartsWith("<"))
            {
                SetSelectColumnsForDynamicLinqObject(sb, returnType);
            }
            else
            {
                Info.Handler.SetValuesForSelect(sb, noLazy);
            }
            return sb;
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

        public virtual SqlStatement GetInsertStatement(object obj)
        {
            InsertStatementBuilder sb = GetInsertStatementBuilder(obj);
            return sb.ToSqlStatement(Info);
        }

        public virtual InsertStatementBuilder GetInsertStatementBuilder(object obj)
        {
            var sb = new InsertStatementBuilder(Info.From.MainTableName);
            Info.Handler.SetValuesForInsert(sb, obj);
            return sb;
        }

        public virtual SqlStatement GetUpdateStatement(object obj, Condition iwc)
        {
            var sb = new UpdateStatementBuilder(Info.From.MainTableName);
            Info.Handler.SetValuesForUpdate(sb, obj);
            sb.Where.Conditions = iwc;
            return sb.ToSqlStatement(Info);
        }

        public virtual SqlStatement GetDeleteStatement(object obj)
        {
            var sb = new DeleteStatementBuilder(Info.From.MainTableName);
            sb.Where.Conditions = ObjectInfo.GetKeyWhereClause(obj);
            return sb.ToSqlStatement(Info);
        }

        public virtual SqlStatement GetDeleteStatement(Condition iwc)
        {
            var sb = new DeleteStatementBuilder(Info.From.MainTableName);
            sb.Where.Conditions = iwc;
            return sb.ToSqlStatement(Info);
        }

        public virtual SqlStatement GetCreateStatement()
        {
            CreateTableStatementBuilder cts = GetCreateTableStatementBuilder();
            return cts.ToSqlStatement(Info);
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
