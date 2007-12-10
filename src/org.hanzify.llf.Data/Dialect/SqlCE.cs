
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder;

namespace Lephone.Data.Dialect
{
    public class SqlCE : SqlServer2000
    {
        protected override SqlStatement GetPagedSelectSqlStatement(SelectStatementBuilder ssb)
        {
            return base.GetNormalSelectSqlStatement(ssb);
        }

        public override string GetConnectionString(string ConnectionString)
        {
            string s = base.ProcessConnectionnString(ConnectionString);
            if (s[0] == '@')
            {
                return "Data Source=" + s.Substring(1);
            }
            return s;
        }

        public override string IdentitySelectString
        {
            get { return "select @@identity;\n"; }
        }

        public override bool ExecuteEachLine
        {
            get { return true; }
        }
    }
}
