using System;
using System.Data;
using Leafing.Core.Setting;

namespace Leafing.Data.SqlEntry {
    [Serializable]
    public class TimeConsumingSqlStatement : SqlStatement {
        public TimeConsumingSqlStatement(CommandType sqlCommandType, string sqlCommandText) : base(sqlCommandType, sqlCommandText) {
            InitSqlTimeOut();
        }

        public TimeConsumingSqlStatement(string sqlCommandText) : base(sqlCommandText) {
            InitSqlTimeOut();
        }

        public TimeConsumingSqlStatement(CommandType sqlCommandType, string sqlCommandText, params object[] os) : base(sqlCommandType, sqlCommandText, os) {
            InitSqlTimeOut();
        }

        public TimeConsumingSqlStatement(string sqlCommandText, params object[] os) : base(sqlCommandText, os) {
            InitSqlTimeOut();
        }

        public TimeConsumingSqlStatement(CommandType sqlCommandType, string sqlCommandText, params DataParameter[] dps) : base(sqlCommandType, sqlCommandText, dps) {
            InitSqlTimeOut();
        }

        public TimeConsumingSqlStatement(string sqlCommandText, params DataParameter[] dps) : base(sqlCommandText, dps) {
            InitSqlTimeOut();
        }

        public TimeConsumingSqlStatement(CommandType sqlCommandType, string sqlCommandText, DataParameterCollection dpc) : base(sqlCommandType, sqlCommandText, dpc) {
            InitSqlTimeOut();
        }

        public TimeConsumingSqlStatement(string sqlCommandText, DataParameterCollection dpc) : base(sqlCommandText, dpc) {
            InitSqlTimeOut();
        }

        private void InitSqlTimeOut() {
            SqlTimeOut = ConfigReader.Config.Database.TimeConsumingSqlTimeOut;
        }
    }
}
