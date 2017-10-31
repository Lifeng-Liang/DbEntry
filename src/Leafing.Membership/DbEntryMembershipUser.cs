using System;
using System.Collections.Generic;
using System.Web.Security;
using Leafing.Data.Definition;

namespace Leafing.Membership
{
    public class DbEntryMembershipUser : DbObjectModel<DbEntryMembershipUser>
    {
        [Length(255), Index(UNIQUE = true),Description("用户名")]
        public string UserName { get; set; }

        [Length(128)]
        [Description("密码")]
        public string Password { get; set; }

        [Description("加密方式")]
        public int PasswordFormat { get; set; }

        [Length(128)]
        [Description("密钥")]
        public string PasswordSalt { get; set; }

        [Length(255)]
        [AllowNull]
        [Description("Email")]
        public string Email { get; set; }

        [Length(255)]
        [AllowNull]
        [Description("LoweredEmail")]
        public string LoweredEmail { get; set; }

        [Length(255)]
        [AllowNull]
        [Description("密码问题")]
        public string PasswordQuestion { get; set; }

        [Length(128)]
        [AllowNull]
        [Description("密码答案")]
        public string PasswordAnswer { get; set; }

        [Description("审核")]
        public bool IsApproved { get; set; }

        [Description("锁定")]
        public bool IsLockedOut { get; set; }

        [Description("最后登录时间")]
        public DateTime LastLoginDate { get; set; }

        [Description("最后修改密码时间")]
        public DateTime LastPasswordChangedDate { get; set; }

        [Description("最后锁定时间")]
        public DateTime LastLockoutDate { get; set; }

        [Description("密码错误次数")]
        public int FailedCount { get; set; }

        [Description("密码错误开始时间")]
        public DateTime FailedStart { get; set; }

        [Description("密码问题错误次数")]
        public int FailedAnswerCount { get; set; }

        [Description("密码问题错误开始时间")]
        public DateTime FailedAnswerStart { get; set; }

        [Length(3000)]
        [AllowNull]
        [Description("备注")]
        public string Comment { get; set; }

        [Description("最后活动时间")]
        public DateTime LastActivityDate { get; set; }

        [Exclude]
        public bool IsOnline 
        {
            get
            {
                var onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
                var compareTime = DateTime.Now.Subtract(onlineSpan);
                return LastActivityDate > compareTime;
            }
        }

        [Description("创建日期")]
        [SpecialName]
        public DateTime CreatedOn { get; set; }

        [CrossTableName("DbEntryMembershipUser_Role")]
		public HasAndBelongsToMany<DbEntryRole> Roles { get; private set; }

        public DbEntryMembershipUser ()
        {
            Roles = new HasAndBelongsToMany<DbEntryRole> (this, "Id", "DbEntryMembershipUser_Id");
        }

        public MembershipUser ToMembershipUser()
        {
            return new MembershipUser("DbEntryMembershipProvider", UserName, Id, Email, PasswordQuestion, Comment, IsApproved, IsLockedOut, CreatedOn, LastLoginDate,
                            LastActivityDate, LastPasswordChangedDate, LastLockoutDate);
        }
    }
}
