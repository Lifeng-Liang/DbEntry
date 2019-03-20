using System;

namespace Leafing.Data.Definition {
    public static class CommonRegular {
        public const string EmailRegular = @"^\w+((-\w+)|(\.\w+))*\@\w+((\.|-)\w+)*\.\w+$";
        public const string UrlRegular = @"^http([s])?://(\w+[.])*\w+((\\|\/)[\w+.?%&=]*)*$";
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class StringColumnAttribute : Attribute {
        public bool IsUnicode = true;
        public string Regular;
        public string ErrorMessage;
    }
}
