
#region usings

using System;
using System.Data;
using Lephone.Data.Common;
using Lephone.Util;
using Lephone.Util.Text;

#endregion

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public class SqlStatement
	{
		public string SqlCommandText;
		public DataParamterCollection Paramters;
		public int SqlTimeOut = DataSetting.SqlTimeOut;
		public CommandType SqlCommandType;
        internal int StartIndex = 0;
        internal int EndIndex = 0;

		internal protected static CommandType GetCommandType(string SqlCommandText)
		{
			if ( StringHelper.IsIndentityName(SqlCommandText) )
			{
				return CommandType.StoredProcedure;
			}
			else
			{
				return CommandType.Text;
			}
		}

		public SqlStatement(string SqlCommandText)
			: this( GetCommandType(SqlCommandText), SqlCommandText )
		{
		}

		public SqlStatement(CommandType SqlCommandType, string SqlCommandText)
		{
			this.SqlCommandType = SqlCommandType;
			this.SqlCommandText = SqlCommandText;
			Paramters = new DataParamterCollection();
		}

		public SqlStatement(string SqlCommandText, params object[] os)
			: this( GetCommandType(SqlCommandText), SqlCommandText, os )
		{
		}

		public SqlStatement(CommandType SqlCommandType, string SqlCommandText, params object[] os)
			: this(SqlCommandType, SqlCommandText)
		{
			Paramters.Add(os);
		}

		public SqlStatement(string SqlCommandText, params DataParamter[] dps)
			: this( GetCommandType(SqlCommandText), SqlCommandText, dps )
		{
		}

		public SqlStatement(CommandType SqlCommandType, string SqlCommandText, params DataParamter[] dps)
			: this(SqlCommandType, SqlCommandText)
		{
			Paramters.Add(dps);
		}

		public SqlStatement(string SqlCommandText, DataParamterCollection dpc)
			: this( GetCommandType(SqlCommandText), SqlCommandText, dpc )
		{
		}

		public SqlStatement(CommandType SqlCommandType, string SqlCommandText, DataParamterCollection dpc)
			: this(SqlCommandType, SqlCommandText)
		{
			Paramters.Add(dpc);
		}

		public override bool Equals(object obj)
		{
			SqlStatement Sql = (SqlStatement)obj;
			bool b = (this.SqlCommandText == Sql.SqlCommandText)
				&& (this.SqlTimeOut == Sql.SqlTimeOut)
				&& (this.Paramters.Equals(Sql.Paramters)
				&& (this.SqlCommandType == Sql.SqlCommandType));
			return b;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}<{1}><{2}>({3})", SqlCommandText, SqlCommandType, SqlTimeOut, Paramters);
		}
    }
}
