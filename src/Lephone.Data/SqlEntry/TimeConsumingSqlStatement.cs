using System;
using System.Data;
using Lephone.Data.Common;

namespace Lephone.Data.SqlEntry
{
	[Serializable]
	public class TimeConsumingSqlStatement : SqlStatement
	{
		public TimeConsumingSqlStatement(CommandType SqlCommandType, string SqlCommandText) : base(SqlCommandType, SqlCommandText)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(string SqlCommandText) : base(SqlCommandText)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(CommandType SqlCommandType, string SqlCommandText, params object[] os) : base(SqlCommandType, SqlCommandText, os)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(string SqlCommandText, params object[] os) : base(SqlCommandText, os)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(CommandType SqlCommandType, string SqlCommandText, params DataParameter[] dps) : base(SqlCommandType, SqlCommandText, dps)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(string SqlCommandText, params DataParameter[] dps) : base(SqlCommandText, dps)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(CommandType SqlCommandType, string SqlCommandText, DataParameterCollection dpc) : base(SqlCommandType, SqlCommandText, dpc)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(string SqlCommandText, DataParameterCollection dpc) : base(SqlCommandText, dpc)
		{
			InitSqlTimeOut();
		}

		private void InitSqlTimeOut()
		{
			SqlTimeOut = DataSettings.TimeConsumingSqlTimeOut;
		}
	}
}
