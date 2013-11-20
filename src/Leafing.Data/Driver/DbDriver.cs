using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Collections;
using Leafing.Core;
using Leafing.Core.Text;
using Leafing.Data.Common;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Driver
{
	public abstract class DbDriver
	{
        protected static Hashtable SpParameters = Hashtable.Synchronized(new Hashtable());

	    public readonly string Name;

		public readonly string ConnectionString;

		public readonly DbDialect Dialect;

        protected DbFactory ProviderFactory;

	    public readonly AutoScheme AutoScheme;

        private List<string> _tableNames;

	    internal List<string> TableNames
	    {
	        get
	        {
                if(_tableNames == null)
                {
                    _tableNames = new List<string>();
                    var list = GetTableNames();
                    foreach (var name in list)
                    {
                        _tableNames.Add(name.ToLower());
                    }
                }
                return _tableNames;
	        }
	    }

        protected DbDriver(DbDialect dialect, string name, string connectionString, string dbProviderFactoryName, AutoScheme autoScheme)
        {
            this.Name = name;
            this.ConnectionString = connectionString;
            this.Dialect = dialect;
            this.AutoScheme = autoScheme;
            ProviderFactory = CreateDbProviderFactory(dbProviderFactoryName);
        }

        public List<string> GetTableNames()
        {
            var ret = new List<string>();
            DbStructInterface si = Dialect.GetDbStructInterface();
            string userId = Dialect.GetUserId(ConnectionString);
            using(var c = (DbConnection)GetDbConnection())
            {
                c.Open();
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
            }
            return ret;
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
                    fn = string.Format("Leafing.Data.Driver.{0}, Leafing.Data", fn.Substring(1));
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

            SetCommandTimeOut(e, sql.SqlTimeOut);

            e.CommandType = sql.SqlCommandType;
            e.Connection = conn;
            FillDbParameters(sql, e);
            return e;
        }

        protected virtual void SetCommandTimeOut(IDbCommand e, int timeOut)
        {
            e.CommandTimeout = timeOut;
        }

        public virtual IDbConnection GetDbConnection()
        {
            IDbConnection c = ProviderFactory.CreateConnection();
            c.ConnectionString = ConnectionString;
            return c;
        }

        public virtual IDbDataParameter GetDbParameter(DataParameter dp)
        {
            IDbDataParameter odp = ProviderFactory.CreateParameter();
            odp.ParameterName = dp.Key;
            odp.Value = GetDbValue(dp.Value);
            odp.DbType = (DbType)dp.Type;
            odp.Direction = dp.Direction;
            odp.SourceColumn = dp.SourceColumn;
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
