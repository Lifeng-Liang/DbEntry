
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using Lephone.Data.Definition;

namespace Lephone.Web.Common
{
    public abstract class DbEntryMembershipUser : DbObjectModel<DbEntryMembershipUser>
    {
        [Length(1, 30), Index(UNIQUE = true)]
        public abstract string UserName { get; set; }

        [Length(1, 30), Index(UNIQUE = true)]
        public abstract string Password { get; set; }

        [Length(1, 50), Index(UNIQUE = true)]
        public abstract string Email { get; set; }

        [Length(50)]
        public abstract string PasswordQuestion { get; set; }

        [Length(50)]
        public abstract string PasswordAnswer { get; set; }

        public abstract bool IsApproved { get; set; }

        [AllowNull]
        public abstract string Comment { get; set; }

        [SpecialName]
        public abstract DateTime CreatedOn { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public abstract IList<DbEntryRole> Roles { get; set; }

        public DbEntryMembershipUser Init(string UserName, string Password, string Email, string PasswordQuestion, string PasswordAnswer, bool IsApproved, string Comment)
        {
            this.UserName = UserName;
            this.Password = Password;
            this.Email = Email;
            this.PasswordQuestion = PasswordQuestion;
            this.PasswordAnswer = PasswordAnswer;
            this.IsApproved = IsApproved;
            this.Comment = Comment;
            return this;
        }

        public MembershipUser ToMembershipUser()
        {
            return new MembershipUser("DbEntryMembershipProvider", UserName, Id, Email, PasswordQuestion, Comment, IsApproved, false,
                CreatedOn, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
        }
    }
}
