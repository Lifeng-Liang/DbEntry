using System;
using System.Data;
using Lephone.Data.Common;
using Lephone.Util.Text;

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public class SqlStatement
	{
		public string SqlCommandText;
		public DataParameterCollection Parameters;
		public int SqlTimeOut = DataSetting.SqlTimeOut;
		public CommandType SqlCommandType;
        internal int StartIndex;
        internal int EndIndex;

		internal protected static CommandType GetCommandType(string SqlCommandText)
		{
		    if ( StringHelper.IsIndentityName(SqlCommandText) )
			{
				return CommandType.StoredProcedure;
			}
		    return CommandType.Text;
		}

	    public SqlStatement(string SqlCommandText)
			: this( GetCommandType(SqlCommandText), SqlCommandText )
		{
		}

		public SqlStatement(CommandType SqlCommandType, string SqlCommandText)
		{
			this.SqlCommandType = SqlCommandType;
			this.SqlCommandText = SqlCommandText;
			Parameters = new DataParameterCollection();
		}

		public SqlStatement(string SqlCommandText, params object[] os)
			: this( GetCommandType(SqlCommandText), SqlCommandText, os )
		{
		}

		public SqlStatement(CommandType SqlCommandType, string SqlCommandText, params object[] os)
			: this(SqlCommandType, SqlCommandText)
		{
			Parameters.Add(os);
		}

		public SqlStatement(string SqlCommandText, params DataParameter[] dps)
			: this( GetCommandType(SqlCommandText), SqlCommandText, dps )
		{
		}

		public SqlStatement(CommandType SqlCommandType, string SqlCommandText, params DataParameter[] dps)
			: this(SqlCommandType, SqlCommandText)
		{
			Parameters.Add(dps);
		}

		public SqlStatement(string SqlCommandText, DataParameterCollection dpc)
			: this( GetCommandType(SqlCommandText), SqlCommandText, dpc )
		{
		}

		public SqlStatement(CommandType SqlCommandType, string SqlCommandText, DataParameterCollection dpc)
			: this(SqlCommandType, SqlCommandText)
		{
			Parameters.Add(dpc);
		}

		public override bool Equals(object obj)
		{
			SqlStatement Sql = (SqlStatement)obj;
			bool b = (this.SqlCommandText == Sql.SqlCommandText)
				&& (this.SqlTimeOut == Sql.SqlTimeOut)
				&& (this.Parameters.Equals(Sql.Parameters)
				&& (this.SqlCommandType == Sql.SqlCommandType));
			return b;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}<{1}><{2}>({3})", SqlCommandText, SqlCommandType, SqlTimeOut, Parameters);
		}
    }
}
