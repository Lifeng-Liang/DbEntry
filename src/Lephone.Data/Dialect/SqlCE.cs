using System.Collections.Generic;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder;

namespace Lephone.Data.Dialect
{
    public class SqlCE : SqlServer2000
    {
        public override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb, List<string> queryRequiredFields)
        {
            return ssb.GetNormalSelectSqlStatement(this, queryRequiredFields);
        }

        public override string GetConnectionString(string connectionString)
        {
            string s = ProcessConnectionnString(connectionString);
            if (s[0] == '@')
            {
                return "Data Source=" + s.Substring(1);
            }
            return s;
        }

        public override string IdentitySelectString
        {
            get { return "SELECT @@IDENTITY;\n"; }
        }

        public override string IdentityColumnString
        {
            get { return "IDENTITY NOT NULL"; }
        }

        public override bool ExecuteEachLine
        {
            get { return true; }
        }
    }
}
