using Leafing.Core;
using Leafing.Core.Setting;
using Leafing.Data.Driver;

namespace Leafing.Data.SqlEntry {
    public class DataProviderFactory : FlyweightBase<string, DataProvider> {
        public static readonly DataProviderFactory Instance = new DataProviderFactory();

        protected override DataProvider GetInst(string tk) {
            var name = tk.IsNullOrEmpty() ? ConfigReader.Config.Database.Default : tk;
            return base.GetInst(name);
        }

        protected override DataProvider CreateInst(string t) {
            return new DataProvider(DbDriverFactory.Instance.GetInstance(t));
        }
    }
}