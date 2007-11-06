
#region usings

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Lephone.Util;
using Lephone.Util.Text;
using Lephone.Data.Dialect;
using Lephone.Data.Driver;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data.SqlEntry
{
	public class DataProvider : IHasConnection
	{
		internal DbDriver m_Driver;

        ConnectionContext IHasConnection.ConnectionProvider
        {
            get { return ConProvider; }
        }

        internal ConnectionContext ConProvider
        {
            get
            {
                if (Scope<ConnectionContext>.Current != null)
                {
                    return Scope<ConnectionContext>.Current;
                }
                return new ConnectionContext(m_Driver);
            }
        }

		protected DataProvider() {}

        public DataProvider(DbDriver driver)
		{
            m_Driver = driver;
		}

		public DbDialect Dialect
		{
			get { return m_Driver.Dialect; }
		}

        public DbDriver Driver
        {
            get { return m_Driver; }
        }

        #region utils

        public List<DbColumnInfo> GetDbColumnInfoList(string TableName)
        {
            string SqlStr = "select * from " + this.Dialect.QuoteForTableName(TableName) + " where 1<>1";
            SqlStatement sql = new SqlStatement(CommandType.Text, SqlStr);
            List<DbColumnInfo> ret = new List<DbColumnInfo>();
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
            List<string> ret = new List<string>();
            DbStructInterface si = Dialect.GetDbStructInterface();
            string UserId = Dialect.GetUserId(Driver.ConnectionString);
            UsingConnection(delegate()
            {
                DbConnection c = (DbConnection)ConProvider.Connection;
                foreach (DataRow dr in c.GetSchema(si.TablesTypeName, si.TablesParams).Rows)
                {
                    if (si.FiltrateDatabaseName)
                    {
                        if (!dr["TABLE_SCHEMA"].Equals(c.Database)) { continue; }
                    }
                    if (UserId != null)
                    {
                        if (!dr["OWNER"].Equals(UserId)) { continue; }
                    }
                    string s = dr[si.TableNameString].ToString();
                    ret.Add(s);
                }
            });
            return ret;
        }

        public IDbBulkCopy GetDbBulkCopy()
        {
            if (Driver is SqlServerDriver)
            {
                if (Scope<ConnectionContext>.Current != null)
                {
                    SqlConnection c = (SqlConnection)Scope<ConnectionContext>.Current.Connection;
                    return new SqlServerBulkCopy(c);
                }
                throw new DataException("It must have current connection.");
            }
            else
            {
                return new CommonBulkCopy(this);
            }
        }
        
        #endregion

        #region Execute Sql

        public DataSet ExecuteDataset(SqlStatement Sql, Type ReturnType)
		{
			DataSet ds = (DataSet)ClassHelper.CreateInstance(ReturnType);
			ExecuteDataset(Sql, ds);
			return ds;
		}

		public DataSet ExecuteDataset(SqlStatement Sql)
		{
			DataSet ds = new DataSet();
			ExecuteDataset(Sql, ds);
			return ds;
		}

		private void ExecuteDataset(SqlStatement Sql, DataSet ds)
		{
            UsingExistedConnection(delegate()
            {
                using (IDbCommand e = GetDbCommand(Sql))
                {
                    IDbDataAdapter d = m_Driver.GetDbAdapter(e);
                    if (Dialect.ExecuteEachLine)
                    {
                        int i = 0;
                        foreach (string s in split(e.CommandText))
                        {
                            e.CommandText = s;
                            ((DbDataAdapter)d).Fill(ds, 0, DataSetting.MaxRecords, "Table" + i.ToString());
                            i++;
                        }
                    }
                    else
                    {
                        d.Fill(ds);
                    }
                    PopulateOutParams(Sql, e);
                }
            });
        }

		public int UpdateDataset(SqlStatement Sql, DataSet ds)
		{
            int ret = 0;
            UsingExistedConnection(delegate()
            {
                Sql.SqlCommandType = CommandType.Text;
                using (IDbCommand e = GetDbCommand(Sql))
                {
                    IDbDataAdapter d = m_Driver.GetUpdateDbAdapter(e);
                    ret = d.Update(ds);
                }
            });
            return ret;
        }

		public object ExecuteScalar(SqlStatement Sql)
		{
            object obj = null;
            UsingExistedConnection(delegate()
            {
                using (IDbCommand e = GetDbCommand(Sql))
                {
                    if (Dialect.ExecuteEachLine)
                    {
                        ExecuteBeforeLines(e);
                    }
                    obj = e.ExecuteScalar();
                    PopulateOutParams(Sql, e);
                }
            });
            return obj;
        }

		public int ExecuteNonQuery(SqlStatement Sql)
		{
            int i = 0;
            UsingExistedConnection(delegate()
            {
                using (IDbCommand e = GetDbCommand(Sql))
                {
                    if (Dialect.ExecuteEachLine)
                    {
                        i = ExecuteBeforeLines(e);
                    }
                    i += e.ExecuteNonQuery();
                    PopulateOutParams(Sql, e);
                }
            });
            return i;
        }

        public void ExecuteDataReader(SqlStatement Sql, CallbackObjectHandler<IDataReader> callback)
        {
            ExecuteDataReader(Sql, CommandBehavior.Default, callback);
        }

        public void ExecuteDataReader(SqlStatement Sql, CommandBehavior behavior, CallbackObjectHandler<IDataReader> callback)
        {
            UsingExistedConnection(delegate()
            {
                using (IDbCommand e = GetDbCommand(Sql))
                {
                    if (Dialect.ExecuteEachLine)
                    {
                        ExecuteBeforeLines(e);
                    }
                    using (IDataReader r = e.ExecuteReader(behavior))
                    {
                        PopulateOutParams(Sql, e);
                        callback(r);
                    }
                }
            });
        }

        // It's only for stupid oracle
        internal void ExecuteDataReader(SqlStatement Sql, Type ReturnType, CallbackObjectHandler<IDataReader> callback)
        {
            UsingExistedConnection(delegate()
            {
                using (IDbCommand e = GetDbCommand(Sql))
                {
                    if (Dialect.ExecuteEachLine)
                    {
                        ExecuteBeforeLines(e);
                    }
                    using (IDataReader r = e.ExecuteReader(CommandBehavior.Default))
                    {
                        PopulateOutParams(Sql, e);
                        using (IDataReader dr = Dialect.GetDataReader(r, ReturnType))
                        {
                            callback(dr);
                        }
                    }
                }
            });
        }

        protected IDbCommand GetDbCommand(SqlStatement Sql)
        {
            ConnectionContext et = ConProvider;
            IDbCommand e = m_Driver.GetDbCommand(Sql, et.Connection);
            if (et.Transaction != null)
            {
                e.Transaction = et.Transaction;
            }
            return e;
        }

        protected void PopulateOutParams(SqlStatement Sql, IDbCommand e)
        {
            if (Sql.Paramters.UserSetKey && (Sql.SqlCommandType == CommandType.StoredProcedure))
            {
                for (int i = 0; i < Sql.Paramters.Count; i++)
                {
                    DataParamter p = Sql.Paramters[i];
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
            List<string> al = split(e.CommandText);
            int ret = 0;
            for (int i = 0; i < al.Count - 1; i++)
            {
                e.CommandText = al[i];
                ret += e.ExecuteNonQuery();
            }
            e.CommandText = (string)al[al.Count - 1];
            return ret;
        }

        private List<string> split(string cText)
        {
            List<string> ret = new List<string>();
            using (StreamReader sr = new StreamReader(new MemoryStream(Encoding.Unicode.GetBytes(cText)), Encoding.Unicode))
            {
                StringBuilder statement = new StringBuilder();
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
                            if (this.Dialect.NotSupportPostFix) { statement.Length--; }
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

        private static readonly Regex reg = new Regex("'.*'|\\?", RegexOptions.Compiled);

        public SqlStatement GetSqlStatement(string SqlStr, params object[] os)
        {
            CommandType ct = SqlStatement.GetCommandType(SqlStr);
            if (ct == CommandType.StoredProcedure)
            {
                return new SqlStatement(ct, SqlStr, os);
            }
            DataParamterCollection dpc = new DataParamterCollection();
            int start = 0, n = 0;
            StringBuilder sql = new StringBuilder();
            string pp = Dialect.ParamterPrefix + "p";
            foreach (Match m in reg.Matches(SqlStr))
            {
                if (m.Length == 1)
                {
                    string pn = pp + n;
                    sql.Append(SqlStr.Substring(start, m.Index - start));
                    sql.Append(pn);
                    start = m.Index + 1;
                    DataParamter dp = new DataParamter(pn, os[n]);
                    dpc.Add(dp);
                    n++;
                }
            }
            if (start < SqlStr.Length)
            {
                sql.Append(SqlStr.Substring(start));
            }
            SqlStatement ret = new SqlStatement(ct, sql.ToString(), dpc);
            return ret;
        }

        public DataSet ExecuteDataset(string SqlCommandText, params object[] os)
        {
            return ExecuteDataset(GetSqlStatement(SqlCommandText, os));
        }

        public object ExecuteScalar(string SqlCommandText, params object[] os)
        {
            return ExecuteScalar(GetSqlStatement(SqlCommandText, os));
        }

        public int ExecuteNonQuery(string SqlCommandText, params object[] os)
        {
            return ExecuteNonQuery(GetSqlStatement(SqlCommandText, os));
        }

        #endregion

        #region IUsingTransaction

        public void UsingExistedTransaction(CallbackVoidHandler callback)
        {
            if (Scope<ConnectionContext>.Current != null)
            {
                callback();
                return;
            }
            UsingTransaction(callback);
        }

        public void UsingExistedTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            if (Scope<ConnectionContext>.Current != null)
            {
                ConnectionContext et = Scope<ConnectionContext>.Current;
                if (et.IsolationLevel == il)
                {
                    callback();
                    return;
                }
            }
            UsingTransaction(callback);
        }

        public void UsingTransaction(CallbackVoidHandler callback)
        {
            UsingTransaction(IsolationLevel.ReadCommitted, callback);
        }

        public void UsingTransaction(IsolationLevel il, CallbackVoidHandler callback)
        {
            UsingConnection(delegate()
            {
                ConnectionContext cc = ConProvider;
                cc.BeginTransaction(il);
                try
                {
                    callback();
                    cc.Commit();
                }
                catch
                {
                    cc.Rollback();
                    OnTransactionError();
                    throw;
                }
            });
        }

        protected virtual void OnTransactionError()
        {
        }

        public void UsingConnection(CallbackVoidHandler callback)
        {
            using (ConnectionContext cc = new ConnectionContext(m_Driver))
            {
                using (new Scope<ConnectionContext>(cc))
                {
                    try
                    {
                        callback();
                    }
                    catch
                    {
                        OnConnectionError();
                        throw;
                    }
                    finally
                    {
                        cc.Close();
                    }
                }
            }
        }

        protected virtual void OnConnectionError()
        {
        }

        public void UsingExistedConnection(CallbackVoidHandler callback)
        {
            if (Scope<ConnectionContext>.Current != null)
            {
                callback();
                return;
            }
            UsingConnection(callback);
        }

        #endregion
    }
}
