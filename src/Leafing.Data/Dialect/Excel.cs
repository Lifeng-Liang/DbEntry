namespace Leafing.Data.Dialect
{
	public class Excel : Access
	{
        public override string GetConnectionString(string connectionString)
        {
            string s = ProcessConnectionnString(connectionString);
            if (s[0] == '@')
            {
                return string.Format(
                    "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES';",
                    s.Substring(1));
            }
            return s;
        }

		public override string QuoteForTableName(string tableName)
		{
			if ( tableName.IndexOf("$") < 0 )
			{
				tableName = tableName + "$";
			}
			return OpenQuote + tableName + CloseQuote;
		}
	}
}
