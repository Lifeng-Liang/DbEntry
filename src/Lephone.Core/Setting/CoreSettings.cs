using Lephone.Core.Text;

namespace Lephone.Core.Setting
{
    public static class CoreSettings
    {
        public static readonly string MiscProvider = "Lephone.Core.MiscProvider, Lephone.Core";

        public static readonly string NameMapper = "Lephone.Core.Text.NameMapper, Lephone.Core";

        public static readonly string LogFileName = "{0}{1}.{2}.log";

        [ShowString("Ioc.EnableAutoLoad")]
        public static readonly bool IocEnableAutoLoad = true;

        static CoreSettings()
        {
            typeof(CoreSettings).Initalize();
        }
    }
}
