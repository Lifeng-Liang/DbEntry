using Leafing.Core;
using Leafing.Core.Setting;
using Leafing.Core.Text;
using Leafing.Data.Dialect;
using Leafing.Core.Ioc;

namespace Leafing.Data.Driver {
    public class DbDriverFactory : FlyweightBase<string, DbDriver> {
        private static ConnectionStringCoder coder = SimpleContainer.Get<ConnectionStringCoder>();
        public static readonly DbDriverFactory Instance = new DbDriverFactory();

        protected override DbDriver GetInst(string tk) {
            var name = tk.IsNullOrEmpty() ? ConfigReader.Config.Database.Default : tk;
            return base.GetInst(name);
        }

        protected override DbDriver CreateInst(string t) {
            return GetDriver(t);
        }

        private static DbDriver GetDriver(string name) {
            var ctx = ConfigReader.Config.Database.Context[name];
            if (ctx.DataBase == "") {
                throw new SettingException("DataBase must be set as a valid value. Current get is : " + ctx.DataBase);
            }
            string[] ss = StringHelper.Split(ctx.DataBase, ':', 2);
            var ds = ss[0].Trim();
            if (ds[0] == '@') {
                ds = "Leafing.Data.Dialect." + ds.Substring(1) + ", Leafing.Data";
            }
            var d = (DbDialect)ClassHelper.CreateInstance(ds);
            var scs = coder.Decode(ss[1].Trim());
            string cs = d.GetConnectionString(scs);
            string pf = ctx.ProviderFactory;
            return CreateDbDriver(d, name, ctx.Driver, cs, ctx.ProviderFactory, ctx.AutoScheme);
        }

        private static DbDriver CreateDbDriver(DbDialect dialectClass, string name, string driverClassName,
            string connectionString, string dbProviderFactoryName, string das) {
            var autoScheme = das.ToEnum<AutoScheme>();
            CheckProperty(dialectClass, connectionString);
            if (driverClassName == "") {
                return dialectClass.CreateDbDriver(name, connectionString, dbProviderFactoryName, autoScheme);
            }
            return (DbDriver)ClassHelper.CreateInstance(driverClassName,
                dialectClass, name, connectionString, dbProviderFactoryName, autoScheme);
        }

        private static void CheckProperty(DbDialect dialectClass, string connectionString) {
            if (dialectClass == null || connectionString == "") {
                throw new SettingException("DialectClass or ConnectionString not defined in the App.config file.");
            }
        }
    }
}
