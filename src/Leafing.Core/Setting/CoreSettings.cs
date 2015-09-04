using Leafing.Core.Text;
using Leafing.Core.Logging;

namespace Leafing.Core.Setting
{
    public static class CoreSettings
    {
        public static readonly string NameMapper = "@Default";

        public static readonly string LogFileName = "{0}{1}.{2}.log";

		public static readonly LogLevel LogLevel = LogLevel.All;

        public static readonly int DelayToStart = 30000;

        public static readonly int MinStartTicks = 180000;

        static CoreSettings()
        {
            ConfigHelper.LeafingSettings.InitClass(typeof(CoreSettings));
        }
    }
}
