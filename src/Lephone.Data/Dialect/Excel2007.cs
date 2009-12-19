namespace Lephone.Data.Dialect
{
    public class Excel2007 : Excel
    {
        public override string GetConnectionString(string connectionString)
        {
            string s = ProcessConnectionnString(connectionString);
            if (s[0] == '@')
            {
                return string.Format(
                        @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES'",
                        s.Substring(1));
            }
            return s;
        }
    }
}
