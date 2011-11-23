using System.Collections.Generic;
using Lephone.Data.Builder;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Dialect
{
    public class PostgreSQL : DbDialect
    {
        public PostgreSQL()
        {
            TypeNames[DataType.DateTime] = "TIMESTAMP";
            TypeNames[DataType.Time] = "TIME";
        }

        public override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb, List<string> queryRequiredFields)
        {
            SqlStatement sql = ssb.GetNormalSelectSqlStatement(this, queryRequiredFields);
            sql.SqlCommandText = string.Format("{0} LIMIT {2} OFFSET {1}",
                sql.SqlCommandText, ssb.Range.Offset, ssb.Range.Rows);
            return sql;
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, new[] { null, null, null, "BASE TABLE" }, null, null, null);
        }

        protected override string GetStringNameWithLength(string baseType, bool isUnicode, int length)
        {
            return base.GetStringNameWithLength(baseType, false, length);
        }

        public override string IdentitySelectString
        {
            get { return "SELECT LASTVAL();\n"; }
        }

        public override string IdentityColumnString
        {
            get { return ""; }
        }

        public override string IdentityTypeString
        {
            get { return "BIGSERIAL"; }
        }
    }
}
