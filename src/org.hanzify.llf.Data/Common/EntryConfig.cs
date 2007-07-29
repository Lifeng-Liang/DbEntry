
#region usings

using System;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.Driver;
using org.hanzify.llf.Data.SqlEntry;
using org.hanzify.llf.util;
using org.hanzify.llf.util.Text;
using org.hanzify.llf.util.Setting;

#endregion

namespace org.hanzify.llf.Data.Common
{
	public static class EntryConfig
	{
        public static readonly DbDriver Default = GetDriver(DataSetting.DefaultContext);

        public static DbDriver GetDriver(int Index)
		{
			return GetDriver(Index.ToString());
		}

        public static DbDriver GetDriver(string Prefix)
		{
            if (Prefix != "") { Prefix += "."; }
            string pd = Prefix + "DataBase";
            string ds = ConfigHelper.DefaultSettings.GetValue(pd);
            if (ds == "")
            {
                throw new SettingException("DataBase must be set as a valid value. Current get is : " + pd);
            }
            string[] ss = StringHelper.Split(ds, ':', 2);
            ds = ss[0].Trim();
            if (ds[0] == '@')
            {
                ds = "org.hanzify.llf.Data.Dialect." + ds.Substring(1) + ", org.hanzify.llf.Data";
            }
            DbDialect d = (DbDialect)ClassHelper.CreateInstance(ds);
            string cs = d.GetConnectionString(ss[1].Trim());
            string pf = ConfigHelper.DefaultSettings.GetValue(Prefix + "DbProviderFactory");
            string dcn = ConfigHelper.DefaultSettings.GetValue(Prefix + "DbDriver");
            return CreateDbDriver(d, dcn, cs, pf);
		}

        public static DbDriver CreateDbDriver(DbDialect DialectClass, string DriverClassName, string ConnectionString, string DbProviderFactoryName)
        {
            CheckProperty(DialectClass, ConnectionString);
            if (DriverClassName == "")
            {
                return DialectClass.CreateDbDriver(ConnectionString, DbProviderFactoryName);
            }
            return (DbDriver)ClassHelper.CreateInstance(DriverClassName,
                DialectClass, ConnectionString, DbProviderFactoryName);
        }

        private static void CheckProperty(DbDialect DialectClass, string ConnectionString)
		{
            if (DialectClass == null || ConnectionString == "")
			{
                throw new SettingException("DialectClass or ConnectionString not defined in the App.config file.");
			}
		}
	}
}
