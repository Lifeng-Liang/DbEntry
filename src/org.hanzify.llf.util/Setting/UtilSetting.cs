
using System;
using Lephone.Util.Setting;

namespace Lephone.Util.Setting
{
    public static class UtilSetting
    {
        public static readonly string NowProvider = "Lephone.Util.NowProvider, Lephone.Util";

        static UtilSetting()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(UtilSetting));
        }
    }
}
