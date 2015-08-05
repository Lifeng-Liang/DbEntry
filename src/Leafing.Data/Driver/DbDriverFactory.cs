using System;
using Leafing.Core;
using Leafing.Core.Setting;
using Leafing.Core.Text;
using Leafing.Data.Common;
using Leafing.Data.Dialect;

namespace Leafing.Data.Driver
{
    public class DbDriverFactory : FlyweightBase<string, DbDriver>
    {
        public static readonly DbDriverFactory Instance = new DbDriverFactory();

        protected override DbDriver GetInst(string tk)
        {
            var name = tk.IsNullOrEmpty() ? DataSettings.DefaultContext : tk;
            return base.GetInst(name);
        }

        protected override DbDriver CreateInst(string t)
        {
            return GetDriver(t);
        }

        private static DbDriver GetDriver(string prefix)
        {
            var name = prefix;
            if (prefix != "") { prefix += "."; }
            string pd = prefix + "DataBase";
            string ds = ConfigHelper.LeafingSettings.GetValue(pd);
            if (ds == "")
            {
                throw new SettingException("DataBase must be set as a valid value. Current get is : " + pd);
            }
            string[] ss = StringHelper.Split(ds, ':', 2);
            ds = ss[0].Trim();
            if (ds[0] == '@')
            {
                ds = "Leafing.Data.Dialect." + ds.Substring(1) + ", Leafing.Data";
            }
            var d = (DbDialect)ClassHelper.CreateInstance(ds);
            string cs = d.GetConnectionString(ss[1].Trim());
            string pf = ConfigHelper.LeafingSettings.GetValue(prefix + "DbProviderFactory");
            string dcn = ConfigHelper.LeafingSettings.GetValue(prefix + "DbDriver");
            string act = ConfigHelper.LeafingSettings.GetValue(prefix + "AutoCreateTable");
            string auto = ConfigHelper.LeafingSettings.GetValue(prefix + "AutoScheme");
            return CreateDbDriver(d, name, dcn, cs, pf, act, auto);
        }

        private static DbDriver CreateDbDriver(DbDialect dialectClass, string name, string driverClassName, 
            string connectionString, string dbProviderFactoryName, string act, string auto)
        {
            var autoScheme = AutoScheme.None;
            if(auto.IsNullOrEmpty())
            {
                if(!string.IsNullOrEmpty(act))
                {
                    if(bool.Parse(act))
                    {
                        autoScheme = AutoScheme.CreateTable;
                    }
                }
            }
            else
            {
                autoScheme = (AutoScheme)Enum.Parse(typeof(AutoScheme), auto);
            }
            CheckProperty(dialectClass, connectionString);
            if (driverClassName == "")
            {
                return dialectClass.CreateDbDriver(name, connectionString, dbProviderFactoryName, autoScheme);
            }
            return (DbDriver)ClassHelper.CreateInstance(driverClassName,
                dialectClass, name, connectionString, dbProviderFactoryName, autoScheme);
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
