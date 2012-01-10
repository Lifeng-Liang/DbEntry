using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using Leafing.Core;
using Leafing.Data.Builder;
using Leafing.Data.Builder.Clause;
using Leafing.Data.Common;
using Leafing.Data.Definition;
using Leafing.Data.Model.Composer;
using Leafing.Data.Model.Creator;
using Leafing.Data.Model.Deleter;
using Leafing.Data.Model.Handler;
using Leafing.Data.Model.Inserter;
using Leafing.Data.Model.Saver;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Model
{
    public class ModelOperator
    {
        protected readonly ObjectInfo Info;
        internal readonly QueryComposer Composer;
        protected DataProvider Provider;
        private readonly SimpleObjectSaver _saver;
        private readonly SimpleDeleter _deleter;

        internal ModelOperator(ObjectInfo info, QueryComposer composer, DataProvider provider, IDbObjectHandler handler)
        {
            this.Info = info;
            this.Composer = composer;
            this.Provider = provider;
            this._saver = SaverFactory.CreateSaver(info, composer, provider, handler);
            this._deleter = DeleterFactory.CreateDeleter(info, composer, provider, handler);
        }

        public virtual int Delete(IDbObject obj)
        {
            TryCreateTable();
            return _deleter.Delete(obj);
        }

        public virtual void Save(IDbObject obj)
        {
            TryCreateTable();
            _saver.Save(obj);
        }

        public virtual void Insert(IDbObject obj)
        {
            TryCreateTable();
            _saver.Insert(obj);
        }

        public virtual void Update(IDbObject obj)
        {
            TryCreateTable();
            _saver.Update(obj);
        }

        internal void TryCreateTable()
        {
            if (Provider.Driver.AutoCreateTable)
            {
                if (Provider.Driver.TableNames == null)
                {
                    InitTableNames();
                }
                Debug.Assert(Provider.Driver.TableNames != null);
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
            if (name != null && !Provider.Driver.TableNames.ContainsKey(name.ToLower()))
            {
                CreateTableAndRelations(oi, op, mt => !Provider.Driver.TableNames.ContainsKey(mt.Name.ToLower()));
            }
        }

        internal void CreateTableAndRelations(ObjectInfo oi, ModelOperator op, Func<CrossTable, bool> callback)
        {
            IfUsingTransaction(Provider.Dialect.NeedCommitCreateFirst, delegate
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

        private static void IfUsingTransaction(bool isUsing, Action callback)
        {
            if (isUsing)
            {
                DbEntry.NewTransaction(callback);
            }
            else
            {
                callback();
            }
        }

        private void InitTableNames()
        {
            Provider.Driver.TableNames = new Dictionary<string, int>();
            foreach (string s in Provider.GetTableNames())
            {
                Provider.Driver.TableNames.Add(s.ToLower(), 1);
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
            object ro = Provider.ExecuteScalar(sql);
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
            object ro = Provider.ExecuteScalar(sql);
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
            object ro = Provider.ExecuteScalar(sql);
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
            object ro = Provider.ExecuteScalar(sql);
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
            object ro = Provider.ExecuteScalar(sql);
            if (ro == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDecimal(ro);
        }

        public List<GroupByObject<T1>> GetGroupBy<T1>(Condition iwc, OrderBy order, string columnName)
        {
            TryCreateTable();
            SqlStatement sql = Composer.GetGroupByStatement(iwc, order, columnName);
            var list = new List<GroupByObject<T1>>();
            IProcessor ip = GetListProcessor(list);
            DataLoadDirect(ip, typeof(GroupByObject<T1>), sql, true, false);
            return list;
        }

        public List<GroupBySumObject<T1, T2>> GetGroupBySum<T1, T2>(Condition iwc, OrderBy order, string groupbyColumnName, string sumColumnName)
        {
            TryCreateTable();
            SqlStatement sql = Composer.GetGroupBySumStatement(iwc, order, groupbyColumnName, sumColumnName);
            var list = new List<GroupBySumObject<T1, T2>>();
            IProcessor ip = GetListProcessor(list);
            DataLoadDirect(ip, typeof(GroupBySumObject<T1, T2>), sql, true, false);
            return list;
        }

        public List<T> ExecuteList<T>(string sqlStr) where T : class, IDbObject
        {
            return ExecuteList<T>(new SqlStatement(sqlStr));
        }

        public List<T> ExecuteList<T>(string sqlStr, params object[] os) where T : class, IDbObject
        {
            return ExecuteList<T>(Provider.GetSqlStatement(sqlStr, os));
        }

        public List<T> ExecuteList<T>(SqlStatement sql) where T : class, IDbObject
        {
            TryCreateTable();
            var ret = new List<T>();
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
            if (Provider.Dialect.SupportsRangeStartIndex && endIndex > 0)
            {
                endIndex = endIndex - startIndex + 1;
                startIndex = 1;
            }
            var creator = ModelCreator.GetCreator(returnType, useIndex, noLazy);
            Provider.ExecuteDataReader(sql, returnType, delegate(IDataReader dr)
            {
                int count = 0;
                while (dr.Read())
                {
                    count++;
                    if (count >= startIndex)
                    {
                        object di = creator.CreateObject(dr);
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
            string keyname = Info.KeyMembers[0].Name;
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
            return Provider.ExecuteNonQuery(sql);
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
                Util.CatchAll(() => Provider.Dialect.ExecuteDropSequence(Provider, tn));
            }
            foreach (CrossTable mt in Info.CrossTables.Values)
            {
                DropTable(mt.Name, catchException);
            }
        }

        internal void DropTable(string tableName, bool catchException)
        {
            string s = "DROP TABLE " + Provider.Dialect.QuoteForTableName(tableName);
            var sql = new SqlStatement(s);
            Util.IfCatchException(catchException, () => Provider.ExecuteNonQuery(sql));
            if (Provider.Driver.AutoCreateTable && Provider.Driver.TableNames != null)
            {
                Provider.Driver.TableNames.Remove(tableName.ToLower());
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
            Provider.ExecuteNonQuery(sql);
            var descSql = Provider.Dialect.GetAddDescriptionSql(Info);
            if(descSql != null)
            {
                Util.CatchAll(() => Provider.ExecuteNonQuery(descSql));
            }
            if (Provider.Driver.AutoCreateTable && Provider.Driver.TableNames != null)
            {
                Provider.Driver.TableNames.Add(Info.From.MainTableName.ToLower(), 1);
            }
        }

        public void CreateDeleteToTable()
        {
            var sb = Composer.GetCreateTableStatementBuilder();
            sb.TableName = Info.DeleteToTableName;
            sb.Columns.Add(new ColumnInfo("DeletedOn", typeof(DateTime), null));
            var sql = sb.ToSqlStatement(Provider.Dialect, null, Info.AllowSqlLog);
            Provider.ExecuteNonQuery(sql);
            if (Provider.Driver.AutoCreateTable && Provider.Driver.TableNames != null)
            {
                Provider.Driver.TableNames.Add(Info.DeleteToTableName.ToLower(), 1);
            }
        }

        public void CreateCrossTable(Type t2)
        {
            var ctx2 = ModelContext.GetInstance(t2).Info;
            if (!(Info.CrossTables.ContainsKey(t2) && ctx2.CrossTables.ContainsKey(Info.HandleType)))
            {
                throw new DataException("They are not many to many relationship classes!");
            }
            if (Info.KeyMembers.Length <= 0 || ctx2.KeyMembers.Length <= 0)
            {
                throw new DataException("The relation table must have key column!");
            }
            var mt = Info.CrossTables[t2];
            var cs = mt.GetSortedColumns();
            var cts = new CreateTableStatementBuilder(mt.Name);
            cts.Columns.Add(new ColumnInfo(cs[0].Column, Info.KeyMembers[0].MemberType, cs[0].Table));
            cts.Columns.Add(new ColumnInfo(cs[1].Column, Info.KeyMembers[0].MemberType, cs[1].Table));
            // add index
            cts.Indexes.Add(new DbIndex(null, false, (ASC)cs[0].Column));
            cts.Indexes.Add(new DbIndex(null, false, (ASC)cs[1].Column));
            // execute
            var sql = cts.ToSqlStatement(Provider.Dialect, null, Info.AllowSqlLog);
            Provider.ExecuteNonQuery(sql);
            if (Provider.Driver.AutoCreateTable && Provider.Driver.TableNames != null)
            {
                Provider.Driver.TableNames.Add(mt.Name.ToLower(), 1);
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
