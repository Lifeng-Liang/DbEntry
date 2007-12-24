
#region usings

using System;
using Lephone.Util;
using Lephone.Util.Text;
using Lephone.Util.Setting;

#endregion

namespace Lephone.Data.Common
{
    internal enum HandlerType
    {
        Emit,
        Reflection,
        Both,
    }

    internal static class DataSetting
	{
        public static readonly HandlerType ObjectHandlerType = HandlerType.Emit;

        public static readonly bool AutoCreateTable = false;

        public static readonly string DefaultContext        = "";

		public static readonly int SqlTimeOut               = 30;

        public static readonly int TimeConsumingSqlTimeOut  = 60;

        public static readonly int MaxRecords               = 0;

        [ShowString("Orm.UsingParamter")]
        public static readonly bool UsingParamter           = true;

        public static readonly string CacheKeyGenerator     = "Lephone.Data.Caching.KeyGenerator, Lephone.Data";

        public static readonly string CacheProvider         = "Lephone.Data.Caching.StaticHashCacheProvider, Lephone.Data";

        public static readonly int CacheMinutes             = 5;

        public static readonly int CacheSize                = 1000;

        public static readonly bool CacheEnabled            = false;

        public static readonly bool CacheAnySelectedItem    = false;

        public static readonly bool CacheClearWhenError     = false;

        static DataSetting()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(DataSetting));
        }
	}
}
