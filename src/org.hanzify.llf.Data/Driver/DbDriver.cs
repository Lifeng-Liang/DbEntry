using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using Lephone.Util;
using Lephone.Util.Text;
using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Driver
{
	public abstract class DbDriver
	{
        protected static Hashtable SpParamters = Hashtable.Synchronized(new Hashtable());

		protected string m_ConnectionString;

		protected DbDialect m_Dialect;

        protected DbFactory ProviderFactory;

		public DbDialect Dialect
		{
			get { return m_Dialect; }
		}

        public string ConnectionString
        {
            get { return m_ConnectionString; }
        }

        public DbDriver(DbDialect DialectClass, string ConnectionString, string DbProviderFactoryName)
		{
            this.m_ConnectionString = ConnectionString;
            m_Dialect = DialectClass;
            this.ProviderFactory = CreateDbProviderFactory(DbProviderFactoryName);
		}

        private DbFactory CreateDbProviderFactory(string DbProviderFactoryName)
        {
            if (DbProviderFactoryName != "")
            {
                string[] ss = StringHelper.Split(DbProviderFactoryName, ':', 2);
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

		private ArrayList CloneSpParamters(IList eps)
		{
			ArrayList ps = new ArrayList();
			for (int i = 0; i < eps.Count; i++)
			{
				ps.Add( ((ICloneable)eps[i]).Clone() );
			}
			return ps;
		}

		private void RemoveReturnParamter(IDataParameterCollection dpc)
		{
			for ( int i=0; i<dpc.Count; i++ )
			{
				IDataParameter dp = (IDataParameter)dpc[i];
				if ( dp.Direction == ParameterDirection.ReturnValue )
				{
					dpc.RemoveAt(i);
					break;
				}
			}
		}

		protected void FillDbParameters(SqlStatement Sql, IDbCommand e)
		{
			if ( (!Sql.Paramters.UserSetKey) && (e.CommandType == CommandType.StoredProcedure) )
			{
				string sKey = Sql.SqlCommandText + ":" + m_ConnectionString;
				if ( SpParamters.Contains(sKey) )
				{
					ArrayList al = CloneSpParamters( (ArrayList)SpParamters[sKey] );
					foreach ( IDataParameter ip in al )
					{
						e.Parameters.Add( ip );
					}
				}
				else
				{
					DeriveParameters(e);
					RemoveReturnParamter(e.Parameters);
					ArrayList ps = CloneSpParamters(e.Parameters);
                    SpParamters[sKey] = ps;
				}

				for ( int i=0; i<e.Parameters.Count; i++ )
				{
					((IDataParameter)e.Parameters[i]).Value = Sql.Paramters[i].Value;
				}
			}
			else
			{
				// TODO: parse SqlCommandText and fill it to paramters.
				foreach ( DataParamter dp in Sql.Paramters )
				{
					e.Parameters.Add(GetDbParameter(dp));
				}
			}
		}

		public IDbCommand GetDbCommand(SqlStatement Sql, IDbConnection conn)
		{
			IDbCommand c = GetDriverCommand(Sql, conn);
			return c;
		}

        protected object GetDbValue(object DotNetValue)
        {
            if (DotNetValue == null)
            {
                return DBNull.Value;
            }
            if(DotNetValue.GetType().IsEnum)
            {
                return (int) DotNetValue;
            }
            return DotNetValue;
        }

        public virtual IDbDataAdapter GetDbAdapter(IDbCommand command)
        {
            // DbCommand c = (DbCommand)(command is LinesDbCommand ? ((LinesDbCommand)command).InternalCommand : command);
            IDbCommand c = command;
            IDbDataAdapter d = ProviderFactory.CreateDataAdapter();
            d.SelectCommand = c;
            return d;
        }

        public virtual IDbDataAdapter GetUpdateDbAdapter(IDbCommand command)
        {
            // DbCommand c = (DbCommand)(command is LinesDbCommand ? ((LinesDbCommand)command).InternalCommand : command);
            IDbCommand c = command;
            IDbDataAdapter d = ProviderFactory.CreateDataAdapter();
            d.UpdateCommand = c;
            return d;
        }

        public virtual IDbCommand GetDriverCommand(SqlStatement Sql, IDbConnection conn)
        {
            IDbCommand e = ProviderFactory.CreateCommand();
            e.CommandText = Sql.SqlCommandText;

            // for some database not supports CommandTimeout
            try
            {
                e.CommandTimeout = Sql.SqlTimeOut;
            }
            catch { }

            e.CommandType = Sql.SqlCommandType;
            e.Connection = conn;
            FillDbParameters(Sql, e);
            return e;
        }

        public virtual IDbConnection GetDbConnection()
        {
            IDbConnection c = ProviderFactory.CreateConnection();
            c.ConnectionString = this.m_ConnectionString;
            return c;
        }

        protected virtual IDbDataParameter GetDbParameter(DataParamter dp)
        {
            IDbDataParameter odp = ProviderFactory.CreateParameter();
            odp.ParameterName = dp.Key;
            odp.Value = GetDbValue(dp.Value);
            odp.DbType = (DbType)dp.Type;
            odp.Direction = dp.Direction;
            return odp;
        }

        protected virtual void DeriveParameters(IDbCommand e)
        {
            throw new NotSupportedException();
        }
    }
}
