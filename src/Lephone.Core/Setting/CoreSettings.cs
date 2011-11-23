using Lephone.Core.Text;

namespace Lephone.Core.Setting
{
    public static class CoreSettings
    {
        public static readonly string MiscProvider = "Lephone.Core.MiscProvider, Lephone.Core";

        public static readonly string NameMapper = "Lephone.Core.Text.NameMapper, Lephone.Core";

        public static readonly string LogFileName = "{0}{1}.{2}.log";

        public static readonly int DelayToStart = 30000;

        public static readonly int MinStartTicks = 180000;

        [ShowString("Ioc.EnableAutoLoad")]
        public static readonly bool IocEnableAutoLoad = true;

        static CoreSettings()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(CoreSettings));
        }
    }
}
