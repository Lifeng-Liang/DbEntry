using System;
using System.Collections.Generic;
using Lephone.Data.Builder;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;
using Lephone.Data.Model.Handler;
using Lephone.Data.Model.Member;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Model.Composer
{
    internal class QueryComposer
    {
        protected ModelContext Context;

        public QueryComposer(ModelContext ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }
            this.Context = ctx;
        }

        public virtual SqlStatement GetCreateStatement()
        {
            return this.GetCreateTableStatementBuilder().ToSqlStatement(this.Context);
        }

        public virtual CreateTableStatementBuilder GetCreateTableStatementBuilder()
        {
            var builder = new CreateTableStatementBuilder(this.Context.Info.From.MainTableName);
            foreach (MemberHandler member in this.Context.Info.Members)
            {
                if ((!member.Is.HasMany && !member.Is.HasOne) && !member.Is.HasAndBelongsToMany)
                {
                    var item = new ColumnInfo(member);
                    builder.Columns.Add(item);
                }
            }
            foreach (string key in this.Context.Info.Indexes.Keys)
            {
                bool unique = this.Context.Info.UniqueIndexes.ContainsKey(key);
                builder.Indexes.Add(new DbIndex(key, unique, this.Context.Info.Indexes[key].ToArray()));
            }
            return builder;
        }

        public virtual SqlStatement GetDeleteStatement(Condition iwc)
        {
            var builder = new DeleteStatementBuilder(this.Context.Info.From.MainTableName);
            builder.Where.Conditions = iwc;
            return builder.ToSqlStatement(this.Context);
        }

        public virtual SqlStatement GetDeleteStatement(object obj)
        {
            var builder = new DeleteStatementBuilder(this.Context.Info.From.MainTableName);
            builder.Where.Conditions = ModelContext.GetKeyWhereClause(obj);
            return builder.ToSqlStatement(this.Context);
        }

        public virtual SqlStatement GetGroupByStatement(Condition iwc, OrderBy order, string columnName)
        {
            var builder = new SelectStatementBuilder(Context.Info.From, order, null);
            builder.Where.Conditions = iwc;
            var list = columnName.Split(',');
            foreach (string s in list)
            {
                builder.Keys.Add(new KeyValuePair<string, string>(s, null));
                builder.SetAsGroupBy(s);
            }
            return builder.ToSqlStatement(Context);
        }

        public virtual SqlStatement GetGroupBySumStatement(Condition iwc, OrderBy order, string groupbyColumnName, string sumColumnName)
        {
            var builder = new SelectStatementBuilder(Context.Info.From, order, null);
            builder.Where.Conditions = iwc;
            var list = groupbyColumnName.Split(',');
            foreach (string s in list)
            {
                builder.Keys.Add(new KeyValuePair<string, string>(s, null));
                builder.SetAsGroupBySum(groupbyColumnName, sumColumnName);
            }
            return builder.ToSqlStatement(Context);
        }

        public virtual SqlStatement GetInsertStatement(object obj)
        {
            return this.GetInsertStatementBuilder(obj).ToSqlStatement(this.Context);
        }

        public virtual InsertStatementBuilder GetInsertStatementBuilder(object obj)
        {
            var isv = new InsertStatementBuilder(this.Context.Info.From.MainTableName);
            this.Context.Handler.SetValuesForInsert(isv, obj);
            return isv;
        }

        public virtual SqlStatement GetMaxStatement(Condition iwc, string columnName)
        {
            var builder = new SelectStatementBuilder(this.Context.Info.From, null, null);
            builder.Where.Conditions = iwc;
            builder.SetMaxColumn(columnName);
            return builder.ToSqlStatement(this.Context);
        }

        public virtual SqlStatement GetMinStatement(Condition iwc, string columnName)
        {
            var builder = new SelectStatementBuilder(this.Context.Info.From, null, null);
            builder.Where.Conditions = iwc;
            builder.SetMinColumn(columnName);
            return builder.ToSqlStatement(this.Context);
        }

        public SqlStatement GetResultCountStatement(Condition iwc)
        {
            return this.GetResultCountStatement(iwc, false);
        }

        public virtual SqlStatement GetResultCountStatement(Condition iwc, bool isDistinct)
        {
            var builder2 = new SelectStatementBuilder(this.Context.Info.From, null, null) {IsDistinct = isDistinct};
            SelectStatementBuilder isv = builder2;
            isv.Where.Conditions = iwc;
            if (isDistinct)
            {
                this.Context.Handler.SetValuesForSelect(isv, false);
                string columnName = isv.GetColumns(this.Context.Provider.Dialect, true, false);
                isv.SetCountColumn(columnName);
                isv.IsDistinct = false;
                isv.Keys.Clear();
            }
            else
            {
                isv.SetCountColumn("*");
            }
            return isv.ToSqlStatement(this.Context);
        }

        public SqlStatement GetSelectStatement(FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct, bool noLazy, Type returnType)
        {
            return this.GetSelectStatementBuilder(from, iwc, oc, lc, isDistinct, noLazy, returnType, null).ToSqlStatement(this.Context);
        }

        public virtual SelectStatementBuilder GetSelectStatementBuilder(FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct, bool noLazy, Type returnType, string colName)
        {
            var builder2 = new SelectStatementBuilder(from ?? this.Context.Info.From, oc, lc)
                               {
                                   IsDistinct = isDistinct,
                                   NoLazy = noLazy
                               };
            SelectStatementBuilder sb = builder2;
            sb.Where.Conditions = iwc;
            if (!colName.IsNullOrEmpty())
            {
                sb.Keys.Add(new KeyValuePair<string, string>(colName, null));
                return sb;
            }
            if (returnType.Name.StartsWith("<"))
            {
                this.SetSelectColumnsForDynamicLinqObject(sb, returnType);
                return sb;
            }
            this.Context.Handler.SetValuesForSelect(sb, noLazy);
            return sb;
        }

        public virtual SqlStatement GetSumStatement(Condition iwc, string columnName)
        {
            var builder = new SelectStatementBuilder(this.Context.Info.From, null, null);
            builder.Where.Conditions = iwc;
            builder.SetSumColumn(columnName);
            return builder.ToSqlStatement(this.Context);
        }

        public virtual SqlStatement GetUpdateStatement(object obj, Condition iwc)
        {
            var isv = new UpdateStatementBuilder(this.Context.Info.From.MainTableName);
            this.Context.Handler.SetValuesForUpdate(isv, obj);
            isv.Where.Conditions = iwc;
            return isv.ToSqlStatement(this.Context);
        }

        public virtual void ProcessAfterSave(object obj)
        {
        }

        private void SetSelectColumnsForDynamicLinqObject(SelectStatementBuilder sb, Type returnType)
        {
            var handler = DynamicLinqObjectHandler.Factory.GetInstance(returnType);
            handler.Init(Context.Info);
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
    }
}

