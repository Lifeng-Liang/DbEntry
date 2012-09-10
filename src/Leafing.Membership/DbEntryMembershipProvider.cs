using System;
using System.Collections.Specialized;
using System.Web.Security;
using Leafing.Data;
using Leafing.Core.Setting;
using System.Globalization;
using System.Configuration.Provider;
using System.Text;
using System.Security.Cryptography;
using System.Web.Configuration;

namespace Leafing.Membership
{
    public class DbEntryMembershipProvider : MembershipProvider
    {
        private string _sHashAlgorithm;
        private MembershipPasswordCompatibilityMode _legacyPasswordCompatibilityMode ;// = MembershipPasswordCompatibilityMode.Framework20;

        private const int SaltSize = 64;
        private string _encryptionKey = "AE09F72BA97CBBB5EEAAFF";

        private const int NewPasswordLength = 8;
        //    private string eventSource = "DbEntryMembershipProvider";
        //     private string eventLog = "Application";
        //     private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        //     private string connectionString;

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool _pWriteExceptionsToEventLog;

        public bool WriteExceptionsToEventLog
        {
            get { return _pWriteExceptionsToEventLog; }
            set { _pWriteExceptionsToEventLog = value; }
        }


        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        private bool _pEnablePasswordReset;
        private bool _pEnablePasswordRetrieval;
        private bool _pRequiresQuestionAndAnswer;
        private bool _pRequiresUniqueEmail;
        private int _pMaxInvalidPasswordAttempts;
        private int _pPasswordAttemptWindow;
        private MembershipPasswordFormat _pPasswordFormat;

        public override bool EnablePasswordReset
        {
            get { return _pEnablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return _pEnablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return _pRequiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return _pRequiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return _pMaxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return _pPasswordAttemptWindow; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _pPasswordFormat; }
        }

        private int _pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _pMinRequiredNonAlphanumericCharacters; }
        }

        private int _pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength
        {
            get { return _pMinRequiredPasswordLength; }
        }

        private string _pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression
        {
            get { return _pPasswordStrengthRegularExpression; }
        }


        public override void Initialize(string name, NameValueCollection config)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "DbEntryMembershipProvider";
            }
            base.Initialize(name, config);
            var reader = new CollectionConfigHelper(config);
            reader.GetValue("requiresQuestionAndAnswer", true);

            _pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "6"), CultureInfo.InvariantCulture);//6次错误锁定
            _pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"), CultureInfo.InvariantCulture);//10分钟
            _pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "0"), CultureInfo.InvariantCulture);
            _pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "4"), CultureInfo.InvariantCulture);
            _pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""), CultureInfo.InvariantCulture);
            _pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"), CultureInfo.InvariantCulture);
            _pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "false"), CultureInfo.InvariantCulture);
            _pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"), CultureInfo.InvariantCulture);
            _pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "false"), CultureInfo.InvariantCulture);
            _pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"), CultureInfo.InvariantCulture);//未实现
            _encryptionKey = GetConfigValue(config["encryptionKey"],
                                            _encryptionKey);
            string str2 = config["passwordCompatMode"];
            if (!string.IsNullOrEmpty(str2))
            {
                this._legacyPasswordCompatibilityMode = (MembershipPasswordCompatibilityMode)Enum.Parse(typeof(MembershipPasswordCompatibilityMode), str2);
            }

            string tempFormat = config["passwordFormat"] ?? "Hashed";

            switch (tempFormat)
            {
                case "Hashed":
                    _pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    _pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    _pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }
        }

        public override string ApplicationName
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null && this.CheckPassword(oldPassword, u))
            {
                u.Password = this.EncodePassword(newPassword,int.Parse(u.PasswordSalt), u.PasswordSalt); //StringHelper.Hash(newPassword);
                u.Save();
                return true;
            }
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            var u = FindUser(username, password);
            if (u != null)
            {
                u.PasswordQuestion = newPasswordQuestion;
                u.PasswordAnswer = this.EncodePassword(newPasswordAnswer, int.Parse(u.PasswordSalt), u.PasswordSalt); //StringHelper.Hash(newPasswordAnswer);
                u.Save();
                return true;
            }
            return false;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && !string.IsNullOrWhiteSpace(GetUserNameByEmail(email)))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = GetUser(username, false);
            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                passwordQuestion = string.IsNullOrWhiteSpace(passwordQuestion) ? string.Empty : passwordQuestion;
                passwordAnswer = string.IsNullOrWhiteSpace(passwordAnswer) ? string.Empty : passwordAnswer;
                string salt = GenerateSalt();
                var user = new DbEntryMembershipUser
                               {
                                   UserName = username,
                                   Password = EncodePassword(password, (int)this.PasswordFormat, salt),
                                   PasswordFormat = PasswordFormat.GetHashCode(),
                                   PasswordSalt = salt,
                                   Email = email,
                                   LoweredEmail = email == null ? string.Empty : email.ToLowerInvariant(),
                                   PasswordQuestion = passwordQuestion,
                                   PasswordAnswer = passwordAnswer,
                                   IsApproved = isApproved,
                                   IsLockedOut = false,
                                   LastLoginDate = createDate,
                                   LastPasswordChangedDate = createDate,
                                   LastLockoutDate = createDate,
                                   FailedCount = 0,
                                   FailedStart = createDate,
                                   FailedAnswerCount = 0,
                                   FailedAnswerStart = createDate,
                                   LastActivityDate = createDate,
                                   Comment = string.Empty
                               };
                    //.Init(username, password, email, passwordQuestion, passwordAnswer, isApproved, null);
                //     user.CreateDate = createDate;
                user.Save();
                status = MembershipCreateStatus.Success;
                return GetUser(username, false);
            }
            status = MembershipCreateStatus.DuplicateUserName;

            return null;
        }

        /// <summary>
        /// 快速创建用户
        /// </summary>
        public MembershipUser CreateUser(string username, string password)
        {
            MembershipCreateStatus status;
            return CreateUser(username, password, username, username, GenerateSalt(), true, null, out status);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (deleteAllRelatedData)
            {
                throw new DataException("Not support deleteAllRelatedData");
            }
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null)
            {
                u.Delete();
                return true;
            }
            return false;
        }

        //public override bool EnablePasswordReset
        //{
        //    get { return true; }
        //}

        //public override bool EnablePasswordRetrieval
        //{
        //    get { return true; }
        //}

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return GetMembershipUserCollection(CK.K["Email"].MiddleLike(emailToMatch), pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return GetMembershipUserCollection(CK.K["UserName"].MiddleLike(usernameToMatch), pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return GetMembershipUserCollection(null, pageIndex, pageSize, out totalRecords);
        }

        private static MembershipUserCollection GetMembershipUserCollection(Condition c, int pageIndex, int pageSize, out int totalRecords)
        {
            var ps = DbEntry.From<DbEntryMembershipUser>().Where(c)
                .OrderBy("Id DESC").PageSize(pageSize).GetPagedSelector();

            totalRecords = (int)ps.GetResultCount();

            var muc = new MembershipUserCollection();
            foreach (DbEntryMembershipUser u in ps.GetCurrentPage(pageIndex))
            {
                muc.Add(u.ToMembershipUser());
            }
            return muc;
        }

        //
        // MembershipProvider.GetNumberOfUsersOnline
        //

        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            long numOnline = DbEntryMembershipUser.GetCount(p => p.LastActivityDate > compareTime);

            return Convert.ToInt32(numOnline);
        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }

            string password = "";

            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u == null)
            {
                throw new MembershipPasswordException("Can not find the user.");
            }
            if (u.IsLockedOut)
            {
                throw new MembershipPasswordException("The supplied user is locked out.");
            }

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, u))
            {
                UpdateFailureCount(u, "passwordAnswer");

                throw new MembershipPasswordException("Incorrect password answer.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }

            return password;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null)
            {
                if (userIsOnline)
                {
                    u.LastActivityDate = DateTime.Now;
                    u.Save();
                }
                return u.ToMembershipUser();
            }
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            var user = DbEntryMembershipUser.FindById((long)providerUserKey);
            user.LastActivityDate = DateTime.Now;
            user.Save();
            return user.ToMembershipUser();
        }

        public override string GetUserNameByEmail(string email)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["Email"] == email);
            if (u != null)
            {
                return u.UserName;
            }
            return null;
        }

        //public override int MaxInvalidPasswordAttempts
        //{
        //    get { return WebSettings.MaxInvalidPasswordAttempts; }
        //}

        //public override int MinRequiredNonAlphanumericCharacters
        //{
        //    get { return WebSettings.MinRequiredNonAlphanumericCharacters; }
        //}

        //public override int MinRequiredPasswordLength
        //{
        //    get { return WebSettings.MinRequiredPasswordLength; }
        //}

        //public override int PasswordAttemptWindow
        //{
        //    get { return WebSettings.PasswordAttemptWindow; }
        //}

        //public override MembershipPasswordFormat PasswordFormat
        //{
        //    get { return MembershipPasswordFormat.Hashed; }
        //}

        //public override string PasswordStrengthRegularExpression
        //{
        //    get { return WebSettings.PasswordStrengthRegularExpression; }
        //}

        //public override bool RequiresQuestionAndAnswer
        //{
        //    get { return _requiresQuestionAndAnswer; }
        //}

        //public override bool RequiresUniqueEmail
        //{
        //    get { return true; }
        //}

        public override string ResetPassword(string username, string answer)
        {

            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(p => p.UserName == username);

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(u, "passwordAnswer");

                throw new ProviderException("Password answer required for password reset.");
            }

            string newPassword =
             System.Web.Security.Membership.GeneratePassword(NewPasswordLength, MinRequiredNonAlphanumericCharacters);

            //    var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null)
            {
                if (!RequiresQuestionAndAnswer || this.CheckPassword(u.PasswordAnswer, u))   //还有错误
                {
                    u.Password = this.EncodePassword(newPassword,u.PasswordFormat, u.PasswordSalt); ////StringHelper.Hash(p);
                    u.Save();
                    return newPassword;
                }
            }
            return null;
        }

        /// <summary>
        /// 快速重置密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>新密码</returns>
        public string ResetPassword(string username)
        {
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(p => p.UserName == username);

            string newPassword =
             System.Web.Security.Membership.GeneratePassword(NewPasswordLength, MinRequiredNonAlphanumericCharacters);

            if (u != null)
            {
                u.Password = this.EncodePassword(newPassword,u.PasswordFormat, u.PasswordSalt); ////StringHelper.Hash(p);
                u.Save();
                return newPassword;
            }
            return null;
        }

        /// <summary>
        /// 快速重置密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password"> </param>
        /// <returns>成功返回true,否则返回false</returns>
        public bool ResetPasswordX(string username,string password)
        {
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(p => p.UserName == username);

            if (u != null)
            {
                u.Password = this.EncodePassword(password,u.PasswordFormat, u.PasswordSalt); ////StringHelper.Hash(p);
                u.Save();
                return true;
            }
            return false;
        }

        public override bool UnlockUser(string userName)
        {
            //    throw new Exception("The method or operation is not implemented.");
            var u = DbEntryMembershipUser.FindOne(p => p.UserName == userName);
            if (u != null)
            {
                u.IsLockedOut = false;
                u.FailedAnswerCount = 0;
                u.FailedCount = 0;
                u.Save();
                return true;
            }
            return false;
        }

        public override void UpdateUser(MembershipUser user)
        {
            var u = DbEntryMembershipUser.FindById((long)user.ProviderUserKey);
            if (u != null)
            {
                u.Email = user.Email;
                u.PasswordQuestion = user.PasswordQuestion;
                u.IsApproved = user.IsApproved;
                u.Comment = user.Comment;
                u.Save();
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;
            var u = FindUser(username);
            if (u == null)
                return false;
            if (u.IsLockedOut)
            {
                return false;
            }
            if (CheckPassword(password, u))
            {
                if (u.IsApproved)
                {
                    isValid = true;

                    u.LastLoginDate = DateTime.Now;
                    u.LastActivityDate = DateTime.Now;
                    u.Save();
                }
            }
            else
            {
                UpdateFailureCount(u, "password");
            }
            return isValid;
        }

        private DbEntryMembershipUser FindUser(string username, string password)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null && CheckPassword(password, u))
            {
                return u;
            }
            return null;
        }

        private DbEntryMembershipUser FindUser(string username)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null)
            {
                return u;
            }
            return null;
        }

        private string GenerateSalt()
        {
            var data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);
            return Convert.ToBase64String(data);

        }

        private void UpdateFailureCount(DbEntryMembershipUser user, string failureType)
        {
            var windowStart = new DateTime();
            int failureCount = 0;

            if (failureType == "password")
            {
                failureCount = user.FailedCount;
                try
                {
                    windowStart = user.FailedStart;
                }
                catch
                {
                    windowStart = DateTime.Now;
                }
            }

            if (failureType == "passwordAnswer")
            {
                failureCount = user.FailedAnswerCount;
                windowStart = user.FailedAnswerStart;
            }

            var windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);
            var utcNow = DateTime.Now;
            if (failureCount == 0 || utcNow > windowEnd)
            {
                if (failureType == "password")
                {
                    user.FailedCount = 1;
                    user.FailedStart = DateTime.Now;
                    user.Save();
                }
                if (failureType == "passwordAnswer")
                {
                    user.FailedAnswerCount = 1;
                    user.FailedAnswerStart = DateTime.Now;
                    user.Save();
                }
            }
            else
            {
                if (failureCount++ >= MaxInvalidPasswordAttempts)
                {
                    user.IsLockedOut = true;
                    user.LastLockoutDate = DateTime.Now;
                    user.Save();
                }
                else
                {
                    if (failureType == "password")
                        user.FailedCount = failureCount;

                    if (failureType == "passwordAnswer")
                        user.FailedAnswerCount = failureCount;
                    user.Save();
                }
            }
        }

        private bool CheckPassword(string password, DbEntryMembershipUser user)
        {
            return user.Password.Equals(EncodePassword(password,user.PasswordFormat,user.PasswordSalt));
        }

        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    byte[] allBytes = Convert.FromBase64String(password);
                    byte[] decryptedBytes = DecryptPassword(allBytes);
                    password = (decryptedBytes == null) ? null :
                        Encoding.Unicode.GetString(decryptedBytes, SaltSize, decryptedBytes.Length - SaltSize);
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot decode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        private string EncodePassword(string pass, int passwordFormat, string salt)
        {
            if (passwordFormat == 0)
            {
                return pass;
            }
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Convert.FromBase64String(salt);
            byte[] inArray;
            if (passwordFormat == 1)
            {
                HashAlgorithm hashAlgorithm = this.GetHashAlgorithm();
                if (hashAlgorithm is KeyedHashAlgorithm)
                {
                    var algorithm2 = (KeyedHashAlgorithm)hashAlgorithm;
                    if (algorithm2.Key.Length == src.Length)
                    {
                        algorithm2.Key = src;
                    }
                    else if (algorithm2.Key.Length < src.Length)
                    {
                        var dst = new byte[algorithm2.Key.Length];
                        Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
                        algorithm2.Key = dst;
                    }
                    else
                    {
                        int num2;
                        var buffer5 = new byte[algorithm2.Key.Length];
                        for (int i = 0; i < buffer5.Length; i += num2)
                        {
                            num2 = Math.Min(src.Length, buffer5.Length - i);
                            Buffer.BlockCopy(src, 0, buffer5, i, num2);
                        }
                        algorithm2.Key = buffer5;
                    }
                    inArray = algorithm2.ComputeHash(bytes);
                }
                else
                {
                    var buffer6 = new byte[src.Length + bytes.Length];
                    Buffer.BlockCopy(src, 0, buffer6, 0, src.Length);
                    Buffer.BlockCopy(bytes, 0, buffer6, src.Length, bytes.Length);
                    inArray = hashAlgorithm.ComputeHash(buffer6);
                }
            }
            else
            {
                var buffer7 = new byte[src.Length + bytes.Length];
                Buffer.BlockCopy(src, 0, buffer7, 0, src.Length);
                Buffer.BlockCopy(bytes, 0, buffer7, src.Length, bytes.Length);
                inArray = this.EncryptPassword(buffer7, this._legacyPasswordCompatibilityMode);
            }
            return Convert.ToBase64String(inArray);
        }

        //private string EncodePassword(string pass , string salt)
        //{
        //    return EncodePassword(pass, 1, salt);
        //}

        private HashAlgorithm GetHashAlgorithm()
        {
            if (this._sHashAlgorithm != null)
            {
                return HashAlgorithm.Create(this._sHashAlgorithm);
            } 
            string hashAlgorithmType = System.Web.Security.Membership.HashAlgorithmType;
            if (((this._legacyPasswordCompatibilityMode == MembershipPasswordCompatibilityMode.Framework20)/* && !Membership.IsHashAlgorithmFromMembershipConfig*/ ) &&(hashAlgorithmType != "MD5"))
            {
                hashAlgorithmType = "SHA1";
            }
            HashAlgorithm algorithm = HashAlgorithm.Create(hashAlgorithmType);
            if (algorithm == null)
            {
                throw new Exception("Membership.ThrowHashAlgorithmException()");
            //    RuntimeConfig.GetAppConfig().Membership.ThrowHashAlgorithmException();
            }
            this._sHashAlgorithm = hashAlgorithmType;
            return algorithm;
        }

    }
}
