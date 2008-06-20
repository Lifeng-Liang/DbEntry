using Lephone.Util.Setting;
using Lephone.Util.Text;

namespace Lephone.Util.Setting
{
    public static class UtilSetting
    {
        public static readonly string NowProvider = "Lephone.Util.NowProvider, Lephone.Util";

        public static readonly string NameMapper = "Lephone.Util.Text.NameMapper, Lephone.Util";

        public static readonly string LogFileName = "{0}.{1}.log";

        [ShowString("IOC.EnableAutoLoad")]
        public static readonly bool IOC_EnableAutoLoad = true;

        static UtilSetting()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(UtilSetting));
        }
    }
}
