using System.Collections.Generic;
using Leafing.Data.Builder;
using Leafing.Data.Common;
using Leafing.Data.Driver;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Dialect
{
    public class MySql : DbDialect
    {
        public MySql()
        {
            TypeNames[DataType.Guid] = "CHAR(36)";
            TypeNames[DataType.Double] = "DOUBLE";
            TypeNames[DataType.Single] = "FLOAT";
        }

        protected override string GetBinaryNameWithLength(string baseType, int length)
        {
            if(length == 0)
            {
                return "BLOB";
            }
            return base.GetBinaryNameWithLength(baseType, length);
        }

        public override DbDriver CreateDbDriver(string name, string connectionString, string dbProviderFactoryName, bool autoCreateTable)
        {
            return new MySqlDriver(this, name, connectionString, dbProviderFactoryName, autoCreateTable);
        }

        public override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb, List<string> queryRequiredFields)
        {
            SqlStatement sql = ssb.GetNormalSelectSqlStatement(this, queryRequiredFields);
            sql.SqlCommandText = string.Format("{0} LIMIT {1}, {2}",
                sql.SqlCommandText, ssb.Range.Offset, ssb.Range.Rows);
            return sql;
        }

        protected override string GetStringNameWithLength(string baseType, bool isUnicode, int length)
        {
            return base.GetStringNameWithLength(baseType, false, length);
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(true, null, new[] { null, null, null, "BASE TABLE" }, null, null, null);
        }

        public override string IdentitySelectString
        {
            get { return "SELECT LAST_INSERT_ID();\n"; }
        }

        public override string IdentityColumnString
        {
            get { return "AUTO_INCREMENT NOT NULL"; }
        }

        public override char CloseQuote
        {
            get { return '`'; }
        }

        public override char OpenQuote
        {
            get { return '`'; }
        }

        public override string QuoteParameter(string parameterName)
        {
            return "?" + parameterName;
        }
    }
}
