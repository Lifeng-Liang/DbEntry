﻿namespace Lephone.Data.Dialect
{
    public class Access2007 : Access
    {
        public override string GetConnectionString(string connectionString)
        {
            string s = ProcessConnectionnString(connectionString);
            if (s[0] == '@')
            {
                return "Provider=Microsoft.ACE.OLEDB.12.0; Persist Security Info=False; Data Source=" + s.Substring(1);
            }
            return s;
        }
    }
}
