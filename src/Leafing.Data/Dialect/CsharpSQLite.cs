using System;
using Leafing.Data.Common;

namespace Leafing.Data.Dialect
{
	public class CsharpSQLite : SQLite
	{
		public override DbStructInterface GetDbStructInterface()
		{
			return new DbStructInterface("TABLES", null, null, null, "tbl_name");
		}
	}
}
