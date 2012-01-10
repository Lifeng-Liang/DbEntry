using Leafing.Core;
using Leafing.Data.Common;
using Leafing.Data.Driver;

namespace Leafing.Data.SqlEntry
{
    public class DataProviderFactory : FlyweightBase<string, DataProvider>
    {
        public static readonly DataProviderFactory Instance = new DataProviderFactory();

        protected override DataProvider GetInst(string tk)
        {
            var name = tk.IsNullOrEmpty() ? DataSettings.DefaultContext : tk;
            return base.GetInst(name);
        }

        protected override DataProvider CreateInst(string t)
        {
            return new DataProvider(DbDriverFactory.Instance.GetInstance(t));
        }
    }
}
