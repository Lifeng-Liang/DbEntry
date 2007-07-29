
#region usings

using System;
using org.hanzify.llf.util;
using org.hanzify.llf.util.Text;
using org.hanzify.llf.util.Setting;

#endregion

namespace org.hanzify.llf.Data.SqlEntry
{
	public static class DataSetting
	{
        public static readonly bool AutoCreateTable         = false;

        public static readonly string DefaultContext        = "";

		public static readonly int SqlTimeOut               = 30;

        public static readonly int TimeConsumingSqlTimeOut  = 60;

        public static readonly int MaxRecords               = 0;

        [ShowString("Orm.UsingParamter")]
        public static readonly bool UsingParamter           = true;

        static DataSetting()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(DataSetting));
        }
	}
}
