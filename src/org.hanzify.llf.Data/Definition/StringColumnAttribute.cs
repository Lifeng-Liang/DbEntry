
#region usings

using System;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    public static class CommonRegular
    {
        public const string EmailRegular = @"^\w+((-\w+)|(\.\w+))*\@\w+((\.|-)\w+)*\.\w+$";
        public const string UrlRegular = @"^http([s])?://(\w+[.])*\w+((\\|\/)[\w+.?%&=]*)*$";
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class StringColumnAttribute : Attribute
    {
        public bool IsUnicode = true;
        public string Regular = null;
    }
}
