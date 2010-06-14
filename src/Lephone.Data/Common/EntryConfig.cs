using Lephone.Data.Dialect;
using Lephone.Data.Driver;
using Lephone.Core;
using Lephone.Core.Text;
using Lephone.Core.Setting;

namespace Lephone.Data.Common
{
	public static class EntryConfig
	{
        public static DbContext NewContext(string prefix)
        {
            return new DbContext(prefix);
        }

        public static DbDriver GetDriver(int index)
		{
			return GetDriver(index.ToString());
		}

        public static DbDriver GetDriver(string prefix)
		{
            if (prefix != "") { prefix += "."; }
            string pd = prefix + "DataBase";
            string ds = ConfigHelper.DefaultSettings.GetValue(pd);
            if (ds == "")
            {
                throw new SettingException("DataBase must be set as a valid value. Current get is : " + pd);
            }
            string[] ss = StringHelper.Split(ds, ':', 2);
            ds = ss[0].Trim();
            if (ds[0] == '@')
            {
                ds = "Lephone.Data.Dialect." + ds.Substring(1) + ", Lephone.Data";
            }
            var d = (DbDialect)ClassHelper.CreateInstance(ds);
            string cs = d.GetConnectionString(ss[1].Trim());
            string pf = ConfigHelper.DefaultSettings.GetValue(prefix + "DbProviderFactory");
            string dcn = ConfigHelper.DefaultSettings.GetValue(prefix + "DbDriver");
            string act = ConfigHelper.DefaultSettings.GetValue(prefix + "AutoCreateTable");
            return CreateDbDriver(d, dcn, cs, pf, act);
		}

        public static DbDriver CreateDbDriver(DbDialect dialectClass, string driverClassName, string connectionString, string dbProviderFactoryName, string act)
        {
            bool autoCreateTable = string.IsNullOrEmpty(act) ? false : bool.Parse(act);
            CheckProperty(dialectClass, connectionString);
            if (driverClassName == "")
            {
                return dialectClass.CreateDbDriver(connectionString, dbProviderFactoryName, autoCreateTable);
            }
            return (DbDriver)ClassHelper.CreateInstance(driverClassName,
                dialectClass, connectionString, dbProviderFactoryName, autoCreateTable);
        }

        private static void CheckProperty(DbDialect dialectClass, string connectionString)
		{
            if (dialectClass == null || connectionString == "")
			{
                throw new SettingException("DialectClass or ConnectionString not defined in the App.config file.");
			}
		}
	}
}
