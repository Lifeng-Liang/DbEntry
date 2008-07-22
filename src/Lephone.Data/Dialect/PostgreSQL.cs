using Lephone.Data.Builder;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Dialect
{
    public class PostgreSQL : DbDialect
    {
        public PostgreSQL()
        {
            TypeNames[DataType.DateTime] = "timestamp";
        }

        protected override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            return base.GetNormalSelectSqlStatement(ssb);
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, new[] { null, null, null, "BASE TABLE" }, null, null, null);
        }

        public override string UnicodeTypePrefix
        {
            get { return ""; }
        }

        public override string IdentitySelectString
        {
            get { return "select lastval();\n"; }
        }

        public override string IdentityColumnString
        {
            get { return ""; }
        }

        public override string IdentityTypeString
        {
            get { return "bigserial"; }
        }
    }
}
