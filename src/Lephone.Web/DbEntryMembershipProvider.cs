using System;
using System.Collections.Specialized;
using System.Web.Security;
using Lephone.Data;
using Lephone.Core;
using Lephone.Core.Setting;
using Lephone.Core.Text;

namespace Lephone.Web
{
    public class DbEntryMembershipProvider : MembershipProvider
    {
        private bool _requiresQuestionAndAnswer = true;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "DbEntryMembershipProvider";
            }
            base.Initialize(name, config);
            var reader = new CollectionConfigHelper(config);
            _requiresQuestionAndAnswer = reader.GetValue("requiresQuestionAndAnswer", true);
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
            if (u != null && CommonHelper.AreEqual(u.Password, StringHelper.Hash(oldPassword)))
            {
                u.Password = StringHelper.Hash(newPassword);
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
                u.PasswordAnswer = StringHelper.Hash(newPasswordAnswer);
                u.Save();
                return true;
            }
            return false;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            try
            {
                var u = new DbEntryMembershipUser().Init(username, password, email, passwordQuestion, passwordAnswer, isApproved, null);
                u.Save();
                status = MembershipCreateStatus.Success;
                return u.ToMembershipUser();
            }
            catch (Exception)
            {
                status = MembershipCreateStatus.UserRejected;
                return null;
            }
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

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return true; }
        }

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

        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetPassword(string username, string answer)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null)
            {
                if (CommonHelper.AreEqual(u.PasswordAnswer, StringHelper.Hash(answer)))
                {
                    return "";
                }
            }
            return null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null)
            {
                return u.ToMembershipUser();
            }
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return DbEntryMembershipUser.FindById((long)providerUserKey).ToMembershipUser();
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

        public override int MaxInvalidPasswordAttempts
        {
            get { return WebSettings.MaxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return WebSettings.MinRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return WebSettings.MinRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return WebSettings.PasswordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return WebSettings.PasswordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null)
            {
                if (!RequiresQuestionAndAnswer || CommonHelper.AreEqual(u.PasswordAnswer, StringHelper.Hash(answer)))
                {
                    var p = Rand.Next(10000000, 2147483647).ToString();
                    u.Password = StringHelper.Hash(p);
                    return p;
                }
            }
            return null;
        }

        public override bool UnlockUser(string userName)
        {
            throw new Exception("The method or operation is not implemented.");
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
            var u = FindUser(username, password);
            return (u != null);
        }

        private static DbEntryMembershipUser FindUser(string username, string password)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if(u != null && CommonHelper.AreEqual(u.Password, StringHelper.Hash(password)))
            {
                return u;
            }
            return null;
        }
    }
}
