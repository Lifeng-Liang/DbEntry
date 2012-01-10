using System;
using System.Data;
using Leafing.Data.Common;
using Leafing.Data.Driver;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Dialect
{
	public class Access : SqlServer2000
	{
        public Access()
        {
            TypeNames[DataType.Int64] = "DECIMAL";
            TypeNames[DataType.UInt64] = "DECIMAL";
        }

        public override IDataReader GetDataReader(IDataReader dr, Type returnType)
        {
            return new AccessDataReader(dr, returnType);
        }

        public override DbDriver CreateDbDriver(string name, string connectionString, string dbProviderFactoryName, bool autoCreateTable)
        {
            return new OleDbDriver(this, name, connectionString, dbProviderFactoryName, autoCreateTable);
        }

        public override DbStructInterface GetDbStructInterface()
        {
            return new DbStructInterface(null, new[] { null, null, null, "TABLE" }, null, null, null);
        }

        public override string GetConnectionString(string connectionString)
        {
            string s = ProcessConnectionnString(connectionString);
            if (s[0] == '@')
            {
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + s.Substring(1);
            }
            return s;
        }

		public override string IdentitySelectString
		{
			get { return "SELECT @@IDENTITY;\n"; }
		}

        public override string IdentityColumnString
        {
            get { return ""; }
        }

        public override string IdentityTypeString
        {
            get { return "COUNTER"; }
        }

		public override bool ExecuteEachLine
		{
			get { return true; }
		}

        public override string QuoteDateTimeValue(string theDate)
        {
            return "#" + theDate + "#";
        }

        public override string DbNowString
        {
            get
            {
                return "Now()";
            }
        }
	}
}
