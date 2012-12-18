using Leafing.Core.Text;

namespace Leafing.Core.Setting
{
    public static class CoreSettings
    {
        public static readonly string NameMapper = "Leafing.Core.Text.NameMapper, Leafing.Core";

        public static readonly string LogFileName = "{0}{1}.{2}.log";

        public static readonly int DelayToStart = 30000;

        public static readonly int MinStartTicks = 180000;

        [ShowString("Ioc.EnableAutoLoad")]
        public static readonly bool IocEnableAutoLoad = true;

        [ShowString("Ioc.SearchPath")]
        public static string IocSearchPath = "bin";

        static CoreSettings()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(CoreSettings));
        }
    }
}
