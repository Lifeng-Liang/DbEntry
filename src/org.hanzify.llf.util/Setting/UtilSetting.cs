using Lephone.Util.Setting;

namespace Lephone.Util.Setting
{
    public static class UtilSetting
    {
        public static readonly string NowProvider = "Lephone.Util.NowProvider, Lephone.Util";

        public static readonly string NameMapper = "Lephone.Util.Text.NameMapper, Lephone.Util";

        public static readonly string LogFileName = "{0}.{1}.log";

        static UtilSetting()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(UtilSetting));
        }
    }
}
