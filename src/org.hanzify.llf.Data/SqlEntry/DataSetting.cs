
#region usings

using System;
using Lephone.Util;
using Lephone.Util.Text;
using Lephone.Util.Setting;

#endregion

namespace Lephone.Data.SqlEntry
{
    public enum HandlerType
    {
        Emit,
        Reflection,
        Both,
    }

	public static class DataSetting
	{
        public static readonly HandlerType ObjectHandlerType = HandlerType.Emit;

        public static readonly bool AutoCreateTable = false;

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
