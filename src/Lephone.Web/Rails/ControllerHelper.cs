using System;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    public static class ControllerHelper
    {
        public static object ChangeType(object value, Type t)
        {
            if (t.IsEnum)
            {
                return Enum.Parse(t, value.ToString());
            }
            if (t == typeof(bool))
            {

                if (value == null) return false;
                switch (value.ToString().ToLower())
                {
                    case "on":
                    case "true":
                    case "1":
                        return true;
                    case "off":
                    case "false":
                    case "0":
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return ClassHelper.ChangeType(value, t);
        }
    }
}
