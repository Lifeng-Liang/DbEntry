using System;
using System.Collections.Generic;
using System.Web.Security;
using Lephone.Data.Definition;
using Lephone.Util.Text;

namespace Lephone.Web
{
    public class DbEntryMembershipUser : DbObjectModel<DbEntryMembershipUser>
    {
        [Length(1, 30), Index(UNIQUE = true)]
        public string UserName { get; set; }

        [Length(64)]
        public byte[] Password { get; set; }

        [Length(1, 50), Index(UNIQUE = true)]
        public string Email { get; set; }

        [Length(50)]
        public string PasswordQuestion { get; set; }

        [Length(64)]
        public byte[] PasswordAnswer { get; set; }

        public bool IsApproved { get; set; }

        [AllowNull]
        public string Comment { get; set; }

        [SpecialName]
        public DateTime CreatedOn { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id", CrossTableName = "DbEntryMembershipUser_Role")]
        public IList<DbEntryRole> Roles { get; set; }

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
