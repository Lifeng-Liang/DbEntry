using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using Lephone.Core;
using Lephone.Data.Builder;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Definition;
using Lephone.Data.Driver;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Common
{
    public class ModelOperator : ModelUpdater
    {
        protected readonly ObjectInfo Info;
        internal readonly QueryComposer Composer;

        internal ModelOperator(ObjectInfo info, QueryComposer composer)
            : base(DbDriverFactory.Instance.GetInstance(info.ContextName))
        {
            this.Info = info;
            this.Composer = composer;
        }

        internal void TryCreateTable()
        {
            if (Driver.AutoCreateTable)
            {
                if (Driver.TableNames == null)
                {
                    InitTableNames();
                }
                Debug.Assert(Driver.TableNames != null);
                if(Info.CreateTables != null)
                {
                    foreach(var type in Info.CreateTables)
                    {
                        var ctx = ModelContext.GetInstance(type);
                        InnerTryCreateTable(ctx.Info, ctx.Operator);
                    }
                }
                else
                {
                    InnerTryCreateTable(Info, this);
                }
            }
        }

        private void InnerTryCreateTable(ObjectInfo oi, ModelOperator op)
        {
            string name = oi.From.MainTableName;
            if (name != null && !Driver.TableNames.ContainsKey(name.ToLower()))
            {
                CreateTableAndRelations(oi, op, mt => !Driver.TableNames.ContainsKey(mt.Name.ToLower()));
            }
        }

        internal void CreateTableAndRelations(ObjectInfo oi, ModelOperator op, CallbackReturnHandler<CrossTable, bool> callback)
        {
            IfUsingTransaction(Dialect.NeedCommitCreateFirst, delegate
            {
                op.Create();
                if (!string.IsNullOrEmpty(oi.DeleteToTableName))
                {
                    op.CreateDeleteToTable();
                }
                foreach (CrossTable mt in oi.CrossTables.Values)
                {
                    if (callback(mt))
                    {
                        Debug.Assert(oi.HandleType.Assembly.FullName != null);
                        op.CreateCrossTable(mt.HandleType);
                    }
                }
            });
        }

        private void IfUsingTransaction(bool isUsing, CallbackVoidHandler callback)
        {
            if (isUsing)
            {
                NewTransaction(callback);
            }
            else
            {
                callback();
            }
        }

        private void InitTableNames()
        {
            Driver.TableNames = new Dictionary<string, int>();
            foreach (string s in GetTableNames())
            {
                Driver.TableNames.Add(s.ToLower(), 1);
            }
        }

        public long GetResultCount(Condition iwc)
        {
            return GetResultCount(iwc, false);
        }

        public long GetResultCount(Condition iwc, bool isDistinct)
        {
            TryCreateTable();
            SqlStatement sql = Composer.GetResultCountStatement(iwc, isDistinct);
            object ro = ExecuteScalar(sql);
            return Convert.ToInt64(ro);
        }

        internal long GetResultCountAvoidSoftDelete(Condition iwc, bool isDistinct)
        {
            TryCreateTable();
            SqlStatement sql;
            if (Composer is SoftDeleteQueryComposer)
            {
                sql = ((SoftDeleteQueryComposer)Composer).GetResultCountStatementWithoutDeleteCheck(iwc, isDistinct);
            }
            else
            {
                sql = Composer.GetResultCountStatement(iwc, isDistinct);
            }
            object ro = ExecuteScalar(sql);
            return Convert.ToInt64(ro);
        }

        public decimal? GetMax(Condition iwc, string columnName)
        {
            object o = GetMaxObject(iwc, columnName);
            if (o == null) { return null; }
            return Convert.ToDecimal(o);
        }

        public DateTime? GetMaxDate(Condition iwc, string columnName)
        {
            object o = GetMaxObject(iwc, columnName);
            if (o == null) { return null; }
            return Convert.ToDateTime(o);
        }

        public object GetMaxObject(Condition iwc, string columnName)
        {
            TryCreateTable();
            SqlStatement sql = Composer.GetMaxStatement(iwc, columnName);
            object ro = ExecuteScalar(sql);
            if (ro == DBNull.Value) { return null; }
            return ro;
        }

        public decimal? GetMin(Condition iwc, string columnName)
        {
            object o = GetMinObject(iwc, columnName);
            if (o == null) { return null; }
            return Convert.ToDecimal(o);
        }

        public DateTime? GetMinDate(Condition iwc, string columnName)
        {
            object o = GetMinObject(iwc, columnName);
            if (o == null) { return null; }
            return Convert.ToDateTime(o);
        }

        public object GetMinObject(Condition iwc, string columnName)
        {
            TryCreateTable();
            SqlStatement sql = Composer.GetMinStatement(iwc, columnName);
            object ro = ExecuteScalar(sql);
            if (ro == DBNull.Value)
            {
                return null;
            }
            return ro;
        }

        public decimal? GetSum(Condition iwc, string columnName)
        {
            TryCreateTable();
            SqlStatement sql = Composer.GetSumStatement(iwc, columnName);
            object ro = ExecuteScalar(sql);
            if (ro == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDecimal(ro);
        }

        public DbObjectList<GroupByObject<T1>> GetGroupBy<T1>(Condition iwc, OrderBy order, string columnName)
        {
            TryCreateTable();
            SqlStatement sql = Composer.GetGroupByStatement(iwc, order, columnName);
            var list = new DbObjectList<GroupByObject<T1>>();
            IProcessor ip = GetListProcessor(list);
            DataLoadDirect(ip, typeof(GroupByObject<T1>), sql, true, false);
            return list;
        }

        public DbObjectList<GroupBySumObject<T1, T2>> GetGroupBySum<T1, T2>(Condition iwc, OrderBy order, string groupbyColumnName, string sumColumnName)
        {
            TryCreateTable();
            SqlStatement sql = Composer.GetGroupBySumStatement(iwc, order, groupbyColumnName, sumColumnName);
            var list = new DbObjectList<GroupBySumObject<T1, T2>>();
            IProcessor ip = GetListProcessor(list);
            DataLoadDirect(ip, typeof(GroupBySumObject<T1, T2>), sql, true, false);
            return list;
        }

        public DbObjectList<T> ExecuteList<T>(string sqlStr) where T : class, IDbObject
        {
            return ExecuteList<T>(new SqlStatement(sqlStr));
        }

        public DbObjectList<T> ExecuteList<T>(string sqlStr, params object[] os) where T : class, IDbObject
        {
            return ExecuteList<T>(GetSqlStatement(sqlStr, os));
        }

        public DbObjectList<T> ExecuteList<T>(SqlStatement sql) where T : class, IDbObject
        {
            TryCreateTable();
            var ret = new DbObjectList<T>();
            IProcessor ip = GetListProcessor(ret);
            DataLoadDirect(ip, Info.HandleType, sql, false, false);
            return ret;
        }

        protected virtual IProcessor GetListProcessor(IList il)
        {
            if (DataSettings.MaxRecords == 0)
            {
                return new ListInserter(il);
            }
            return new LimitedListInserter(il);
        }

        public void FillCollection(IList list, Type returnType, FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct, bool noLazy = false)
        {
            IProcessor ip = GetListProcessor(list);
            DataLoad(ip, returnType, from, iwc, oc, lc, isDistinct, noLazy);
        }

        public void DataLoad(IProcessor ip, Type returnType, FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct, bool noLazy)
        {
            TryCreateTable();
            SqlStatement sql = Composer.GetSelectStatement(from, iwc, oc, lc, isDistinct, noLazy, returnType);
            DataLoadDirect(ip, returnType, sql, true, noLazy);
        }

        private void DataLoadDirect(IProcessor ip, Type returnType, SqlStatement sql, bool useIndex, bool noLazy)
        {
            long startIndex = sql.StartIndex;
            long endIndex = sql.EndIndex;
            if (Dialect.SupportsRangeStartIndex && endIndex > 0)
            {
                endIndex = endIndex - startIndex + 1;
                startIndex = 1;
            }
            ExecuteDataReader(sql, returnType, delegate(IDataReader dr)
            {
                int count = 0;
                while (dr.Read())
                {
                    count++;
                    if (count >= startIndex)
                    {
                        object di = ModelContext.CreateObject(returnType, dr, useIndex, noLazy);
                        if (!ip.Process(di))
                        {
                            break;
                        }
                    }
                    if (endIndex > 0 && count >= endIndex)
                    {
                        break;
                    }
                }
            });
        }

        public T GetObject<T>(object key) where T : class, IDbObject
        {
            return (T)GetObject(key);
        }

        public T GetObject<T>(Condition c) where T : class, IDbObject
        {
            return (T)GetObject(c, null, null);
        }

        public T GetObject<T>(Condition c, OrderBy ob) where T : class, IDbObject
        {
            return (T)GetObject(c, ob);
        }

        public object GetObject(object key)
        {
            if (Info.HasOnePrimaryKey)
            {
                return InnerGetObject(key);
            }
            throw new DataException("To call this function, the table must have one primary key.");
        }

        protected virtual object InnerGetObject(object key)
        {
            string keyname = Info.KeyFields[0].Name;
            object obj = GetObject(CK.K[keyname] == key, null, null);
            return obj;
        }

        public object GetObject(Condition c, OrderBy ob)
        {
            return GetObject(c, ob, (ob == null) ? null : new Range(1, 1));
        }

        internal object GetObject(Condition c, OrderBy ob, Range r)
        {
            IList il = new ArrayList();
            FillCollection(il, Info.HandleType, null, c, ob, r, false);
            if (il.Count < 1)
            {
                return null;
            }
            return il[0];
        }

        public int Delete(Condition iwc)
        {
            TryCreateTable();
            var sql = Composer.GetDeleteStatement(iwc);
            return ExecuteNonQuery(sql);
        }

        public void DropTable()
        {
            DropTable(true);
        }

        public void DropTable(bool catchException)
        {
            string tn = Info.From.MainTableName;
            DropTable(tn, catchException);
            if (Info.HasSystemKey)
            {
                CommonHelper.CatchAll(() => Dialect.ExecuteDropSequence(this, tn));
            }
            foreach (CrossTable mt in Info.CrossTables.Values)
            {
                DropTable(mt.Name, catchException);
            }
        }

        internal void DropTable(string tableName, bool catchException)
        {
            string s = "DROP TABLE " + Dialect.QuoteForTableName(tableName);
            var sql = new SqlStatement(s);
            CommonHelper.IfCatchException(catchException, () => ExecuteNonQuery(sql));
            if (Driver.AutoCreateTable && Driver.TableNames != null)
            {
                Driver.TableNames.Remove(tableName.ToLower());
            }
        }

        public void DropAndCreate()
        {
            DropTable(true);
            Create();
        }

        public void Create()
        {
            SqlStatement sql = Composer.GetCreateStatement();
            ExecuteNonQuery(sql);
            var descSql = Dialect.GetAddDescriptionSql(Info);
            if(descSql != null)
            {
                CommonHelper.CatchAll(()=> ExecuteNonQuery(descSql));
            }
            if (Driver.AutoCreateTable && Driver.TableNames != null)
            {
                Driver.TableNames.Add(Info.From.MainTableName.ToLower(), 1);
            }
        }

        public void CreateDeleteToTable()
        {
            var sb = Composer.GetCreateTableStatementBuilder();
            sb.TableName = Info.DeleteToTableName;
            sb.Columns.Add(new ColumnInfo("DeletedOn", typeof(DateTime), false, false, false, false, 0, 0));
            var sql = sb.ToSqlStatement(Dialect, Info.AllowSqlLog);
            ExecuteNonQuery(sql);
            if (Driver.AutoCreateTable && Driver.TableNames != null)
            {
                Driver.TableNames.Add(Info.DeleteToTableName.ToLower(), 1);
            }
        }

        public void CreateCrossTable(Type t2)
        {
            ObjectInfo oi2 = ModelContext.GetInstance(t2).Info;
            if (!(Info.CrossTables.ContainsKey(t2) && oi2.CrossTables.ContainsKey(Info.HandleType)))
            {
                throw new DataException("They are not many to many relation ship classes!");
            }
            if (Info.KeyFields.Length <= 0 || oi2.KeyFields.Length <= 0)
            {
                throw new DataException("The relation table must have key column!");
            }
            CrossTable mt1 = Info.CrossTables[t2];
            var cts = new CreateTableStatementBuilder(mt1.Name);
            var ls = new List<string> { mt1.ColumeName1, mt1.ColumeName2 };
            ls.Sort();
            cts.Columns.Add(new ColumnInfo(ls[0], Info.KeyFields[0].FieldType, false, false, false, false, 0, 0));
            cts.Columns.Add(new ColumnInfo(ls[1], oi2.KeyFields[0].FieldType, false, false, false, false, 0, 0));
            // add index
            cts.Indexes.Add(new DbIndex(null, false, (ASC)mt1.ColumeName1));
            cts.Indexes.Add(new DbIndex(null, false, (ASC)mt1.ColumeName2));
            // execute
            SqlStatement sql = cts.ToSqlStatement(Dialect, Info.AllowSqlLog);
            ExecuteNonQuery(sql);
            if (Driver.AutoCreateTable && Driver.TableNames != null)
            {
                Driver.TableNames.Add(mt1.Name.ToLower(), 1);
            }
        }

        #region Linq methods

        public T GetObject<T>(Expression<Func<T, bool>> expr) where T : class, IDbObject
        {
            var wc = Linq.ExpressionParser<T>.Parse(expr);
            return GetObject<T>(wc);
        }

        public int Delete<T>(Expression<Func<T, bool>> expr) where T : class, IDbObject
        {
            var wc = Linq.ExpressionParser<T>.Parse(expr);
            return Delete(wc);
        }

        #endregion

    }
}
