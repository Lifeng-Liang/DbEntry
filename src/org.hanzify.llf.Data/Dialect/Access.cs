
#region usings

using System;
using org.hanzify.llf.Data.Driver;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Dialect
{
	public class Access : SqlServer2000
	{
        public Access()
        {
            TypeNames[DataType.Int64] = "Decimal";
        }

        public override DbDriver CreateDbDriver(string ConnectionString, string DbProviderFactoryName)
        {
            return new OleDbDriver(this, ConnectionString, DbProviderFactoryName);
        }

        public override string NewGuidString()
        {
            throw new NotImplementedException();
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, new string[] { null, null, null, "TABLE" }, null, null, null);
        }

        public override string GetConnectionString(string ConnectionString)
        {
            string s = base.ProcessConnectionnString(ConnectionString);
            if (s[0] == '@')
            {
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + s.Substring(1);
            }
            return s;
        }

		public override string IdentitySelectString
		{
			get { return "select @@identity;\n"; }
		}

        public override string IdentityColumnString
        {
            get { return ""; }
        }

        public override string IdentityTypeString
        {
            get { return "counter"; }
        }

		public override bool ExecuteEachLine
		{
			get { return true; }
		}

        public override string QuoteDateTimeValue(string theDate)
        {
            return "#" + theDate + "#";
        }
	}
}
