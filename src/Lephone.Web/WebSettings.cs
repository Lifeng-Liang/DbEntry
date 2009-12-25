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

        public static readonly string ControllerAssembly = "";
        public static readonly string RailsPostfix = "";
        public static readonly double SessionCheckEvery = 5;
        public static readonly double SessionExpire = 20;

        public static readonly string ScaffoldingMasterPage = "";
        public static readonly string ContentPlaceHolderHead = "head";
        public static readonly string ContentPlaceHolderBody = "ContentPlaceHolder1";

        public static readonly string CookiesHandler = "Lephone.Web.Rails.CookiesHandler, Lephone.Web";

        static WebSettings()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(WebSettings));
        }
    }
}
