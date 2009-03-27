using System;
using System.Collections.Generic;
using System.Web.Security;
using Lephone.Data.Definition;
using Lephone.Util.Text;

namespace Lephone.Web
{
    public abstract class DbEntryMembershipUser : DbObjectModel<DbEntryMembershipUser>
    {
        [Length(1, 30), Index(UNIQUE = true)]
        public abstract string UserName { get; set; }

        [Length(64)]
        public abstract byte[] Password { get; set; }

        [Length(1, 50), Index(UNIQUE = true)]
        public abstract string Email { get; set; }

        [Length(50)]
        public abstract string PasswordQuestion { get; set; }

        [Length(64)]
        public abstract byte[] PasswordAnswer { get; set; }

        public abstract bool IsApproved { get; set; }

        [AllowNull]
        public abstract string Comment { get; set; }

        [SpecialName]
        public abstract DateTime CreatedOn { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id", CrossTableName = "DbEntryMembershipUser_Role")]
        public abstract IList<DbEntryRole> Roles { get; set; }

        public DbEntryMembershipUser Init(string userName, string password, string email, 
            string passwordQuestion, string passwordAnswer, bool isApproved, string comment)
        {
            this.UserName = userName;
            this.Password = StringHelper.Hash(password);
            this.Email = email;
            this.PasswordQuestion = passwordQuestion;
            this.PasswordAnswer = StringHelper.Hash(passwordAnswer);
            this.IsApproved = isApproved;
            this.Comment = comment;
            return this;
        }

        public MembershipUser ToMembershipUser()
        {
            return new MembershipUser("DbEntryMembershipProvider", UserName, Id, Email, PasswordQuestion, Comment, IsApproved, false,
                CreatedOn, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
        }
    }
}
