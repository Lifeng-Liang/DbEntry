using Leafing.Core.Setting;
using Leafing.Core.Text;

namespace Leafing.Data.Common
{
    public static class DataSettings
    {
        // ReSharper disable RedundantDefaultFieldInitializer
        public static readonly string DefaultContext        = "";

		public static readonly int SqlTimeOut               = 30;

        public static readonly int TimeConsumingSqlTimeOut  = 60;

        public static readonly int MaxRecords               = 0;

        [ShowString("Orm.UsingParameter")]
        public static readonly bool UsingParameter          = true;

        public static readonly int CacheMinutes             = 5;

        public static readonly int CacheSize                = 1000;

        public static readonly bool CacheEnabled            = false;

        public static readonly bool CacheAnySelectedItem    = false;

        public static readonly int DbTimeCheckMinutes       = 10;

        public static readonly bool UsingForeignKey          = false;

        static DataSettings()
        {
            ConfigHelper.LeafingSettings.InitClass(typeof(DataSettings));
        }
        // ReSharper restore RedundantDefaultFieldInitializer
    }
}
