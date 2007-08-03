
#region usings

using System;
using System.IO;
using System.Text;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using org.hanzify.llf.util;
using org.hanzify.llf.util.Text;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.Driver;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.SqlEntry
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

        public List<string> GetTableNames()
        {
            List<string> ret = new List<string>();
            DbStructInterface si = Dialect.GetDbStructInterface();
            UsingConnection(delegate()
            {
                DbConnection c = (DbConnection)ConProvider.Connection;
                foreach (DataRow dr in c.GetSchema(si.TablesTypeName, si.TablesParams).Rows)
                {
                    if (si.FiltrateDatabaseName)
                    {
                        if (!dr["TABLE_SCHEMA"].Equals(c.Database)) { continue; }
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
                throw new DbEntryException("It must have current connection.");
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
                IDbCommand e = GetDbCommand(Sql);
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
            });
        }

		public int UpdateDataset(SqlStatement Sql, DataSet ds)
		{
            int ret = 0;
            UsingExistedConnection(delegate()
            {
                Sql.SqlCommandType = CommandType.Text;
                IDbDataAdapter d = m_Driver.GetUpdateDbAdapter(GetDbCommand(Sql));
                ret = d.Update(ds);
            });
            return ret;
        }

		public object ExecuteScalar(SqlStatement Sql)
		{
            object obj = null;
            UsingExistedConnection(delegate()
            {
                IDbCommand e = GetDbCommand(Sql);
                if (Dialect.ExecuteEachLine)
                {
                    ExecuteBeforeLines(e);
                }
                obj = e.ExecuteScalar();
                PopulateOutParams(Sql, e);
            });
            return obj;
        }

		public int ExecuteNonQuery(SqlStatement Sql)
		{
            int i = 0;
            UsingExistedConnection(delegate()
            {
                IDbCommand e = GetDbCommand(Sql);
                if (Dialect.ExecuteEachLine)
                {
                    i = ExecuteBeforeLines(e);
                }
                i += e.ExecuteNonQuery();
                PopulateOutParams(Sql, e);
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
                IDbCommand e = GetDbCommand(Sql);
                IDataReader r = e.ExecuteReader(behavior);
                PopulateOutParams(Sql, e);
                callback(r);
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

        public static List<string> split(string cText)
        {
            List<string> ret = new List<string>();
            using (StreamReader sr = new StreamReader(new MemoryStream(Encoding.Unicode.GetBytes(cText)), Encoding.Unicode))
            {
                string statement = "";
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    s = s.Trim();
                    if (s.Length > 0)
                    {
                        if (s.Length > 1 && s.Substring(0, 2) == "--") { continue; }
                        if (s[s.Length - 1] == ';')
                        {
                            statement += s.Substring(0, s.Length);
                            if (statement != "")
                            {
                                ret.Add(statement);
                            }
                            statement = "";
                        }
                        else
                        {
                            statement += s;
                        }
                    }
                }
                if (statement != "")
                {
                    ret.Add(statement);
                }
            }
            return ret;
        }

        #endregion

        #region Shortcut

        public DataSet ExecuteDataset(string SqlCommandText, params object[] os)
        {
            return ExecuteDataset(new SqlStatement(SqlCommandText, os));
        }

        public object ExecuteScalar(string SqlCommandText, params object[] os)
        {
            return ExecuteScalar(new SqlStatement(SqlCommandText, os));
        }

        public int ExecuteNonQuery(string SqlCommandText, params object[] os)
        {
            return ExecuteNonQuery(new SqlStatement(SqlCommandText, os));
        }

        #endregion

        #region IUsingTransaction ≥…‘±

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
                finally
                {
                    cc.Rollback();
                }
            });
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
                    finally
                    {
                        cc.Close();
                    }
                }
            }
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
