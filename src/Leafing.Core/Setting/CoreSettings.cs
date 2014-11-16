using Leafing.Core.Text;

namespace Leafing.Core.Setting
{
    public static class CoreSettings
    {
        public static readonly string NameMapper = "@Default";

        public static readonly string LogFileName = "{0}{1}.{2}.log";

        public static readonly int DelayToStart = 30000;

        public static readonly int MinStartTicks = 180000;

        static CoreSettings()
        {
            ConfigHelper.LeafingSettings.InitClass(typeof(CoreSettings));
        }
    }
}
