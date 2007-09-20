
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Util.Setting;

namespace Lephone.Web.Common
{
    public static class WebSettings
    {
        public static readonly int MaxInvalidPasswordAttempts = 5;
        public static readonly int MinRequiredNonAlphanumericCharacters = 3;
        public static readonly int MinRequiredPasswordLength = 6;
        public static readonly int PasswordAttemptWindow = 5;
        public static readonly string PasswordStrengthRegularExpression = "*";

        public static readonly bool UsingAspxPostfix = false;

        static WebSettings()
        {
            ConfigHelper.DefaultSettings.InitClass(typeof(WebSettings));
        }
    }
}
