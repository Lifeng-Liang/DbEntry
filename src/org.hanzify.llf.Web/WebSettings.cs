
using Lephone.Util.Setting;

namespace Lephone.Web
{
    public static class WebSettings
    {
        public static readonly int MaxInvalidPasswordAttempts = 5;
        public static readonly int MinRequiredNonAlphanumericCharacters = 3;
        public static readonly int MinRequiredPasswordLength = 6;
        public static readonly int PasswordAttemptWindow = 5;
        public static readonly string PasswordStrengthRegularExpression = "*";

        public static readonly int DefaultPageSize = 10;

        public static readonly bool UsingAspxPostfix = false;
        public static readonly double SessionCheckEvery = 5;
        public static readonly double SessionExpire = 20;

        static WebSettings()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(WebSettings));
        }
    }
}
