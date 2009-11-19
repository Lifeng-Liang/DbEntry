using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Lephone.Data
{
    internal class DataReaderEmitHelper
    {
        readonly Dictionary<Type, string> _dic;

        public DataReaderEmitHelper()
        {
            // process chars etc.
            _dic = new Dictionary<Type, string>
                      {
                          {typeof (long), "GetInt64"},
                          {typeof (int), "GetInt32"},
                          {typeof (short), "GetInt16"},
                          {typeof (byte), "GetByte"},
                          {typeof (bool), "GetBoolean"},
                          {typeof (DateTime), "GetDateTime"},
                          {typeof (Date), "GetDateTime"},
                          {typeof (Time), "GetDateTime"},
                          {typeof (string), "GetString"},
                          {typeof (decimal), "GetDecimal"},
                          {typeof (float), "GetFloat"},
                          {typeof (double), "GetDouble"},
                          {typeof (Guid), "GetGuid"},
                          {typeof (ulong), "GetInt64"},
                          {typeof (uint), "GetInt32"},
                          {typeof (ushort), "GetInt16"}
                      };
        }

        public MethodInfo GetMethodInfo(Type t)
        {
            Type drt = typeof(IDataRecord);
            if (_dic.ContainsKey(t))
            {
                string n = _dic[t];
                MethodInfo mi = drt.GetMethod(n);
                return mi;
            }
            if (t.IsEnum)
            {
                return drt.GetMethod("GetInt32");
            }
            return null;
        }

        public MethodInfo GetMethodInfo()
        {
            return GetMethodInfo(false);
        }

        public MethodInfo GetMethodInfo(bool isInt)
        {
            if (isInt)
            {
                return typeof(IDataRecord).GetMethod("get_Item", new[] { typeof(int) });
            }
            return typeof(IDataRecord).GetMethod("get_Item", new[] { typeof(string) });
        }
    }
}
