using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Lephone.Core;
using Lephone.Core.Logging;
using Lephone.Data.Dialect;
using Lephone.Data.Driver;
using Lephone.Data.Common;

namespace Lephone.Data.SqlEntry
{
	public partial class DataProvider
	{
		internal DbDriver InnerDriver;

        public readonly DbTimeProvider DbTime;

        public DataProvider(string prefix)
            : this(DbDriverFactory.Instance.GetInstance(prefix))
        {
        }

        public DataProvider(DbDriver driver)
		{
            InnerDriver = driver;
            DbTime = new DbTimeProvider(this);
        }

		public DbDialect Dialect
		{
			get { return InnerDriver.Dialect; }
		}

        public DbDriver Driver
        {
            get { return InnerDriver; }
        }

        #region utils

        public List<DbColumnInfo> GetDbColumnInfoList(string tableName)
        {
            string sqlStr = "SELECT * FROM " + Dialect.QuoteForTableName(tableName) + " WHERE 1<>1";
            var sql = new SqlStatement(CommandType.Text, sqlStr);
            var ret = new List<DbColumnInfo>();
            ExecuteDataReader(sql, CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly, delegate(IDataReader dr)
            {
                DataTable dt = dr.GetSchemaTable();
                foreach (DataRow row in dt.Rows)
                {
                    ret.Add(new DbColumnInfo(row));
                }
            });
            return ret;
        }

        public List<string> GetTableNames()
        {
            var ret = new List<string>();
            DbStructInterface si = Dialect.GetDbStructInterface();
            string userId = Dialect.GetUserId(Driver.ConnectionString);
            DbEntry.NewConnection(delegate
            {
                var c = (DbConnection)ConnectionContext.Current.GetConnection(this);
                foreach (DataRow dr in c.GetSchema(si.TablesTypeName, si.TablesParams).Rows)
                {
                    if (si.FiltrateDatabaseName)
                    {
                        if (!dr["TABLE_SCHEMA"].Equals(c.Database)) { continue; }
                    }
                    if (userId != null)
                    {
                        if (!dr["OWNER"].Equals(userId)) { continue; }
                    }
                    string s = dr[si.TableNameString].ToString();
                    ret.Add(s);
                }
            });
            return ret;
        }

        public IDbBulkCopy GetDbBulkCopy(bool identityInsert)
        {
            if (!identityInsert && Driver is SqlServerDriver)
            {
                if (Scope<ConnectionContext>.Current != null)
                {
                    var c = (SqlConnection)Scope<ConnectionContext>.Current.GetConnection(this);
                    return new SqlServerBulkCopy(c);
                }
                throw new DataException("It must have current connection.");
            }
            return new CommonBulkCopy(this, identityInsert);
        }

	    #endregion

        #region Execute Sql

        public DataSet ExecuteDataset(SqlStatement sql, Type returnType)
		{
			var ds = (DataSet)ClassHelper.CreateInstance(returnType);
			ExecuteDataset(sql, ds);
			return ds;
		}

		public DataSet ExecuteDataset(SqlStatement sql)
		{
			var ds = new DataSet();
			ExecuteDataset(sql, ds);
			return ds;
		}

		private void ExecuteDataset(SqlStatement sql, DataSet ds)
		{
            DbEntry.UsingConnection(delegate
            {
                using (IDbCommand e = GetDbCommand(sql))
                {
                    IDbDataAdapter d = InnerDriver.GetDbAdapter(e);
                    if (Dialect.ExecuteEachLine)
                    {
                        int i = 0;
                        foreach (string s in Split(e.CommandText))
                        {
                            e.CommandText = s;
                            ((DbDataAdapter)d).Fill(ds, 0, DataSettings.MaxRecords, "Table" + i);
                            i++;
                        }
                    }
                    else
                    {
                        d.Fill(ds);
                    }
                    PopulateOutParams(sql, e);
                }
            });
        }

        public int UpdateDataset(SqlStatement selectSql, DataSet ds)
        {
            return UpdateDataset(selectSql, ds, 1);
        }

        public int UpdateDataset(SqlStatement selectSql, DataSet ds, int updateBatchSize)
        {
            int ret = 0;
            DbEntry.UsingConnection(delegate
            {
                var d = (DbDataAdapter)InnerDriver.GetDbAdapter(GetDbCommand(selectSql));
                var cb = InnerDriver.GetCommandBuilder();
                cb.QuotePrefix = Dialect.OpenQuote.ToString();
                cb.QuoteSuffix = Dialect.CloseQuote.ToString();
                cb.DataAdapter = d;
                d.UpdateBatchSize = updateBatchSize;
                ret = d.Update(ds);
                ds.AcceptChanges();
            });
            return ret;
        }

        public int UpdateDataset(SqlStatement insertSql, SqlStatement updateSql, SqlStatement deleteSql, DataSet ds)
		{
            return UpdateDataset(insertSql, updateSql, deleteSql, ds, 1, false);
        }

        public int UpdateDataset(SqlStatement insertSql, SqlStatement updateSql, SqlStatement deleteSql, DataSet ds, int updateBatchSize)
        {
            return UpdateDataset(insertSql, updateSql, deleteSql, ds, updateBatchSize, true);
        }

        private int UpdateDataset(SqlStatement insertSql, SqlStatement updateSql, SqlStatement deleteSql, DataSet ds, int updateBatchSize, bool throwException)
        {
            int ret = 0;
            DbEntry.UsingConnection(delegate
            {
                IDbDataAdapter d = InnerDriver.GetDbAdapter();
                if(insertSql != null)
                {
                    d.InsertCommand = GetDbCommandForUpdate(insertSql);
                }
                if(updateSql != null)
                {
                    d.UpdateCommand = GetDbCommandForUpdate(updateSql);
                }
                if(deleteSql != null)
                {
                    d.DeleteCommand = GetDbCommandForUpdate(deleteSql);
                }
                if (d is DbDataAdapter)
                {
                    ((DbDataAdapter) d).UpdateBatchSize = updateBatchSize;
                }
                else if(throwException)
                {
                    throw new DataException("The DbDataAdapter doesn't support UpdateBatchSize feature.");
                }
                ret = d.Update(ds);
                ds.AcceptChanges();
            });
            return ret;
        }

        private IDbCommand GetDbCommandForUpdate(SqlStatement sql)
        {
            IDbCommand c = GetDbCommand(sql);
            return c;
        }

        public object ExecuteScalar(SqlStatement sql)
		{
            object obj = null;
            DbEntry.UsingConnection(delegate
            {
                using (IDbCommand e = GetDbCommand(sql))
                {
                    if (Dialect.ExecuteEachLine)
                    {
                        ExecuteBeforeLines(e);
                    }
                    obj = e.ExecuteScalar();
                    PopulateOutParams(sql, e);
                }
            });
            return obj;
        }

		public int ExecuteNonQuery(SqlStatement sql)
		{
            int i = 0;
            DbEntry.UsingConnection(delegate
            {
                using (IDbCommand e = GetDbCommand(sql))
                {
                    if (Dialect.ExecuteEachLine)
                    {
                        i = ExecuteBeforeLines(e);
                    }
                    i += e.ExecuteNonQuery();
                    PopulateOutParams(sql, e);
                }
            });
            return i;
        }

        public void ExecuteDataReader(SqlStatement sql, CallbackObjectHandler<IDataReader> callback)
        {
            ExecuteDataReader(sql, CommandBehavior.Default, callback);
        }

        public void ExecuteDataReader(SqlStatement sql, CommandBehavior behavior, CallbackObjectHandler<IDataReader> callback)
        {
            DbEntry.UsingConnection(delegate
            {
                using (IDbCommand e = GetDbCommand(sql))
                {
                    if (Dialect.ExecuteEachLine)
                    {
                        ExecuteBeforeLines(e);
                    }
                    using (IDataReader r = e.ExecuteReader(behavior))
                    {
                        PopulateOutParams(sql, e);
                        callback(r);
                    }
                }
            });
        }

        // It's only for stupid oracle
        internal void ExecuteDataReader(SqlStatement sql, Type returnType, CallbackObjectHandler<IDataReader> callback)
        {
            DbEntry.UsingConnection(delegate
            {
                using (IDbCommand e = GetDbCommand(sql))
                {
                    if (Dialect.ExecuteEachLine)
                    {
                        ExecuteBeforeLines(e);
                    }
                    using (IDataReader r = e.ExecuteReader(CommandBehavior.Default))
                    {
                        PopulateOutParams(sql, e);
                        using (IDataReader dr = Dialect.GetDataReader(r, returnType))
                        {
                            callback(dr);
                        }
                    }
                }
            });
        }

        public IDbCommand GetDbCommand(SqlStatement sql)
        {
            if(sql.NeedLog)
            {
                Logger.SQL.Trace(sql);
            }
            return ConnectionContext.Current.GetDbCommand(sql, this);
        }

        protected void PopulateOutParams(SqlStatement sql, IDbCommand e)
        {
            if (sql.Parameters.UserSetKey && (sql.SqlCommandType == CommandType.StoredProcedure))
            {
                for (int i = 0; i < sql.Parameters.Count; i++)
                {
                    DataParameter p = sql.Parameters[i];
                    if (p.Direction != ParameterDirection.Input)
                    {
                        p.Value = ((IDbDataParameter)e.Parameters[i]).Value;
                    }
                }
            }
        }

        #endregion

        #region Lines plus

        protected int ExecuteBeforeLines(IDbCommand e)
        {
            List<string> al = Split(e.CommandText);
            int ret = 0;
            for (int i = 0; i < al.Count - 1; i++)
            {
                e.CommandText = al[i];
                ret += e.ExecuteNonQuery();
            }
            e.CommandText = al[al.Count - 1];
            return ret;
        }

        private List<string> Split(string cText)
        {
            var ret = new List<string>();
            using (var sr = new StreamReader(new MemoryStream(Encoding.Unicode.GetBytes(cText)), Encoding.Unicode))
            {
                var statement = new StringBuilder();
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    s = s.Trim();
                    if (s.Length > 0)
                    {
                        if (s.Length > 1 && s.Substring(0, 2) == "--") { continue; }
                        if (s[s.Length - 1] == ';')
                        {
                            statement.Append(s.Substring(0, s.Length));
                            if (Dialect.NotSupportPostFix) { statement.Length--; }
                            if (statement.Length != 0)
                            {
                                ret.Add(statement.ToString());
                            }
                            statement = new StringBuilder();
                        }
                        else
                        {
                            statement.Append(s);
                        }
                    }
                }
                if (statement.Length != 0)
                {
                    ret.Add(statement.ToString());
                }
            }
            return ret;
        }

        #endregion

        #region Shortcut

        private static readonly Regex Reg = new Regex("'.*'|\\?", RegexOptions.Compiled);

        public SqlStatement GetSqlStatement(string sqlStr, params object[] os)
        {
            CommandType ct = SqlStatement.GetCommandType(sqlStr);
            if (ct == CommandType.StoredProcedure)
            {
                return new SqlStatement(ct, sqlStr, os);
            }
            var dpc = new DataParameterCollection();
            int start = 0, n = 0;
            var sql = new StringBuilder();
            foreach (Match m in Reg.Matches(sqlStr))
            {
                if (m.Length == 1)
                {
                    string pn = Dialect.QuoteParameter("p" + n);
                    sql.Append(sqlStr.Substring(start, m.Index - start));
                    sql.Append(pn);
                    start = m.Index + 1;
                    var dp = new DataParameter(pn, os[n]);
                    dpc.Add(dp);
                    n++;
                }
            }
            if (start < sqlStr.Length)
            {
                sql.Append(sqlStr.Substring(start));
            }
            var ret = new SqlStatement(ct, sql.ToString(), dpc);
            return ret;
        }

        public DataSet ExecuteDataset(string sqlCommandText, params object[] os)
        {
            return ExecuteDataset(GetSqlStatement(sqlCommandText, os));
        }

        public object ExecuteScalar(string sqlCommandText, params object[] os)
        {
            return ExecuteScalar(GetSqlStatement(sqlCommandText, os));
        }

        public int ExecuteNonQuery(string sqlCommandText, params object[] os)
        {
            return ExecuteNonQuery(GetSqlStatement(sqlCommandText, os));
        }

        #endregion

        public DateTime GetDatabaseTime()
        {
            string sqlstr = "SELECT " + Dialect.DbNowString;
            DateTime dt = Convert.ToDateTime(ExecuteScalar(sqlstr));
            return dt;
        }
	}
}
