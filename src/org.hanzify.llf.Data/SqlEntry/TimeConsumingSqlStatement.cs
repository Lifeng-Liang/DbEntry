
#region usings

using System;
using System.Data;
using org.hanzify.llf.util;

#endregion

namespace org.hanzify.llf.Data.SqlEntry
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

		public TimeConsumingSqlStatement(CommandType SqlCommandType, string SqlCommandText, params DataParamter[] dps) : base(SqlCommandType, SqlCommandText, dps)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(string SqlCommandText, params DataParamter[] dps) : base(SqlCommandText, dps)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(CommandType SqlCommandType, string SqlCommandText, DataParamterCollection dpc) : base(SqlCommandType, SqlCommandText, dpc)
		{
			InitSqlTimeOut();
		}

		public TimeConsumingSqlStatement(string SqlCommandText, DataParamterCollection dpc) : base(SqlCommandText, dpc)
		{
			InitSqlTimeOut();
		}

		private void InitSqlTimeOut()
		{
			SqlTimeOut = DataSetting.TimeConsumingSqlTimeOut;
		}
	}
}
