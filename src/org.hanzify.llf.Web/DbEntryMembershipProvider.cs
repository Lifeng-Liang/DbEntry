
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Util;
using Lephone.Web.Common;

namespace Lephone.Web
{
    public class DbEntryMembershipProvider : MembershipProvider
    {
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
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null && u.Password == oldPassword)
            {
                u.Password = newPassword;
                u.Save();
                return true;
            }
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username && CK.K["Password"] == password);
            if (u != null)
            {
                u.PasswordQuestion = newPasswordQuestion;
                u.PasswordAnswer = newPasswordAnswer;
                u.Save();
                return true;
            }
            return false;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            try
            {
                DbEntryMembershipUser u = DbEntryMembershipUser.New().Init(username, password, email, passwordQuestion, passwordAnswer, isApproved, null);
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
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
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

        private MembershipUserCollection GetMembershipUserCollection(WhereCondition c, int pageIndex, int pageSize, out int totalRecords)
        {
            IPagedSelector ps = DbEntry.From<DbEntryMembershipUser>().Where(c)
                .OrderBy("Id DESC").PageSize(pageSize).GetPagedSelector();

            totalRecords = (int)ps.GetResultCount();

            MembershipUserCollection muc = new MembershipUserCollection();
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
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null)
            {
                if (u.PasswordAnswer == answer)
                {
                    return u.Password;
                }
            }
            return null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
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
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["Email"] == email);
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
            get { return MembershipPasswordFormat.Clear; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return WebSettings.PasswordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return true; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            if (u != null)
            {
                if (u.PasswordAnswer == answer)
                {
                    u.Password = Rand.Next(10000000, 2147483647).ToString();
                    return u.Password;
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
            DbEntryMembershipUser u = DbEntryMembershipUser.FindById((long)user.ProviderUserKey);
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
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username && CK.K["Password"] == password);
            return (u != null);
        }
    }
}
