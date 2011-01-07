using System;
using System.Data;
using Lephone.Data.Common;
using Lephone.Core.Text;

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public class SqlStatement
	{
		public string SqlCommandText;
		public DataParameterCollection Parameters;
		public int SqlTimeOut = DataSettings.SqlTimeOut;
		public CommandType SqlCommandType;
        internal long StartIndex;
        internal long EndIndex;
	    public bool NeedLog = true;

		internal protected static CommandType GetCommandType(string sqlCommandText)
		{
		    if ( StringHelper.IsSpName(sqlCommandText) )
			{
				return CommandType.StoredProcedure;
			}
		    return CommandType.Text;
		}

	    public SqlStatement(string sqlCommandText)
			: this( GetCommandType(sqlCommandText), sqlCommandText )
		{
		}

		public SqlStatement(CommandType sqlCommandType, string sqlCommandText)
		{
			this.SqlCommandType = sqlCommandType;
			this.SqlCommandText = sqlCommandText;
			Parameters = new DataParameterCollection();
		}

		public SqlStatement(string sqlCommandText, params object[] os)
			: this( GetCommandType(sqlCommandText), sqlCommandText, os )
		{
		}

		public SqlStatement(CommandType sqlCommandType, string sqlCommandText, params object[] os)
			: this(sqlCommandType, sqlCommandText)
		{
			Parameters.Add(os);
		}

		public SqlStatement(string sqlCommandText, params DataParameter[] dps)
			: this( GetCommandType(sqlCommandText), sqlCommandText, dps )
		{
		}

		public SqlStatement(CommandType sqlCommandType, string sqlCommandText, params DataParameter[] dps)
			: this(sqlCommandType, sqlCommandText)
		{
			Parameters.Add(dps);
		}

		public SqlStatement(string sqlCommandText, DataParameterCollection dpc)
			: this( GetCommandType(sqlCommandText), sqlCommandText, dpc )
		{
		}

		public SqlStatement(CommandType sqlCommandType, string sqlCommandText, DataParameterCollection dpc)
			: this(sqlCommandType, sqlCommandText)
		{
			Parameters.Add(dpc);
		}

		public override bool Equals(object obj)
		{
			var sql = (SqlStatement)obj;
			var b = (this.SqlCommandText == sql.SqlCommandText)
				&& (this.SqlTimeOut == sql.SqlTimeOut)
				&& (this.Parameters.Equals(sql.Parameters)
				&& (this.SqlCommandType == sql.SqlCommandType));
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
