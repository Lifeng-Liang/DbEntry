using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Collections;
using Lephone.Core;
using Lephone.Core.Text;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Driver
{
	public abstract class DbDriver
	{
        protected static Hashtable SpParameters = Hashtable.Synchronized(new Hashtable());

		public readonly string ConnectionString;

		public readonly DbDialect Dialect;

        protected DbFactory ProviderFactory;

	    public readonly bool AutoCreateTable;

        internal Dictionary<string, int> TableNames;

	    protected DbDriver(DbDialect dialect, string connectionString, string dbProviderFactoryName, bool autoCreateTable)
		{
            this.ConnectionString = connectionString;
            this.Dialect = dialect;
	        this.AutoCreateTable = autoCreateTable;
            ProviderFactory = CreateDbProviderFactory(dbProviderFactoryName);
		}

        private DbFactory CreateDbProviderFactory(string dbProviderFactoryName)
        {
            if (dbProviderFactoryName != "")
            {
                string[] ss = StringHelper.Split(dbProviderFactoryName, ':', 2);
                string fn = ss[0].Trim();
                string addon = ss[1].Trim();
                if (fn[0] == '@')
                {
                    fn = string.Format("Lephone.Data.Driver.{0}, Lephone.Data", fn.Substring(1));
                }
                object f = ClassHelper.CreateInstance(fn);
                if (f is SmartDbFactory)
                {
                    ((SmartDbFactory)f).Init(addon);
                    return (SmartDbFactory)f;
                }
                return new DbFactory((DbProviderFactory)f);
            }
            return new DbFactory(GetDefaultProviderFactory());
        }

	    protected abstract DbProviderFactory GetDefaultProviderFactory();

		private static ArrayList CloneSpParameters(IList eps)
		{
			var ps = new ArrayList();
			foreach (object t in eps)
			{
			    ps.Add( ((ICloneable)t).Clone() );
			}
		    return ps;
		}

		private static void RemoveReturnParameter(IList dpc)
		{
			for ( int i=0; i<dpc.Count; i++ )
			{
				var dp = (IDataParameter)dpc[i];
				if ( dp.Direction == ParameterDirection.ReturnValue )
				{
					dpc.RemoveAt(i);
					break;
				}
			}
		}

		protected void FillDbParameters(SqlStatement sql, IDbCommand e)
		{
			if ( (!sql.Parameters.UserSetKey) && (e.CommandType == CommandType.StoredProcedure) )
			{
				string sKey = sql.SqlCommandText + ":" + ConnectionString;
				if ( SpParameters.Contains(sKey) )
				{
					ArrayList al = CloneSpParameters( (ArrayList)SpParameters[sKey] );
					foreach ( IDataParameter ip in al )
					{
						e.Parameters.Add( ip );
					}
				}
				else
				{
					DeriveParameters(e);
					RemoveReturnParameter(e.Parameters);
					ArrayList ps = CloneSpParameters(e.Parameters);
                    SpParameters[sKey] = ps;
				}

				for ( int i=0; i<sql.Parameters.Count; i++ )
				{
					((IDataParameter)e.Parameters[i]).Value = sql.Parameters[i].Value;
				}
			}
			else
			{
				// TODO: parse SqlCommandText and fill it to Parameters.
				foreach ( DataParameter dp in sql.Parameters )
				{
					e.Parameters.Add(GetDbParameter(dp));
				}
			}
		}

		public IDbCommand GetDbCommand(SqlStatement sql, IDbConnection conn)
		{
			var c = GetDriverCommand(sql, conn);
			return c;
		}

        protected static object GetDbValue(object dotNetValue)
        {
            if (dotNetValue == null)
            {
                return DBNull.Value;
            }
            if(dotNetValue.GetType().IsEnum)
            {
                return (int) dotNetValue;
            }
            return dotNetValue;
        }

        public virtual IDbDataAdapter GetDbAdapter(IDbCommand command)
        {
            // DbCommand c = (DbCommand)(command is LinesDbCommand ? ((LinesDbCommand)command).InternalCommand : command);
            IDbCommand c = command;
            IDbDataAdapter d = ProviderFactory.CreateDataAdapter();
            d.SelectCommand = c;
            return d;
        }

        public virtual IDbDataAdapter GetDbAdapter()
        {
            IDbDataAdapter d = ProviderFactory.CreateDataAdapter();
            return d;
        }

        public virtual IDbCommand GetDriverCommand(SqlStatement sql, IDbConnection conn)
        {
            IDbCommand e = ProviderFactory.CreateCommand();
            e.CommandText = sql.SqlCommandText;

            // for some database not supports CommandTimeout
            CommonHelper.CatchAll(() => e.CommandTimeout = sql.SqlTimeOut);

            e.CommandType = sql.SqlCommandType;
            e.Connection = conn;
            FillDbParameters(sql, e);
            return e;
        }

        public virtual IDbConnection GetDbConnection()
        {
            IDbConnection c = ProviderFactory.CreateConnection();
            c.ConnectionString = ConnectionString;
            return c;
        }

        public virtual IDbDataParameter GetDbParameter(DataParameter dp)
        {
            return GetDbParameter(dp, false);
        }

        public virtual IDbDataParameter GetDbParameter(DataParameter dp, bool includeSourceColumn)
        {
            IDbDataParameter odp = ProviderFactory.CreateParameter();
            odp.ParameterName = dp.Key;
            odp.Value = GetDbValue(dp.Value);
            odp.DbType = (DbType)dp.Type;
            odp.Direction = dp.Direction;
            if (includeSourceColumn)
            {
                odp.SourceColumn = dp.Key[0] == Dialect.ParameterPrefix ? dp.Key.Substring(1) : dp.Key;
            }
            return odp;
        }

        public virtual DbCommandBuilder GetCommandBuilder()
        {
            return ProviderFactory.CreateCommandBuilder();
        }

        protected virtual void DeriveParameters(IDbCommand e)
        {
            throw new NotSupportedException();
        }
    }
}
