using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Leafing.Data.Model.Handler.Generator
{
    internal static class DataReaderEmitHelper
    {
		private static readonly Dictionary<Type, MethodInfo> drDic;
		private static readonly MethodInfo drGetInt32;

        static DataReaderEmitHelper()
        {
			var drt = typeof(IDataRecord);
			drGetInt32 = drt.GetMethod ("GetInt32");
            // process chars etc.
			drDic = new Dictionary<Type, MethodInfo> {
				{ typeof(long), drt.GetMethod ("GetInt64") },
				{ typeof(int), drGetInt32 },
				{ typeof(short), drt.GetMethod ("GetInt16") },
				{ typeof(byte), drt.GetMethod ("GetByte") },
				{ typeof(bool), drt.GetMethod ("GetBoolean") },
				{ typeof(DateTime), drt.GetMethod ("GetDateTime") },
				{ typeof(Date), drt.GetMethod ("GetDateTime") },
				{ typeof(Time), drt.GetMethod ("GetDateTime") },
				{ typeof(string), drt.GetMethod ("GetString") },
				{ typeof(decimal), drt.GetMethod ("GetDecimal") },
				{ typeof(float), drt.GetMethod ("GetFloat") },
				{ typeof(double),drt.GetMethod ("GetDouble") },
				{ typeof(Guid), drt.GetMethod ("GetGuid") },
				{ typeof(ulong), drt.GetMethod ("GetInt64") },
				{ typeof(uint), drt.GetMethod ("GetInt32") },
				{ typeof(ushort), drt.GetMethod ("GetInt16") }
			};
        }

        public static MethodInfo GetMethodInfo(Type t)
        {
			MethodInfo result;
			if (drDic.TryGetValue(t, out result))
            {
				return result;
            }
            if (t.IsEnum)
            {
				return drGetInt32;
            }
            return null;
        }

        public static MethodInfo GetMethodInfo()
        {
            return GetMethodInfo(false);
        }

        public static MethodInfo GetMethodInfo(bool isInt)
        {
            if (isInt)
            {
                return typeof(IDataRecord).GetMethod("get_Item", new[] { typeof(int) });
            }
            return typeof(IDataRecord).GetMethod("get_Item", new[] { typeof(string) });
        }
    }
}
