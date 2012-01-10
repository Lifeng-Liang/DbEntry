using System;
using System.Collections.Generic;
using Leafing.Core;
using Leafing.Data.Definition;
using Soaring.Biz.Helpers;
using Soaring.Biz.Models.Enums;

namespace Soaring.Biz.Models
{
    public class User : DbObjectModel<User>
    {
        public UserType Type { get; set; }

        [Length(50), Index(UNIQUE = true)]
        public string Email { get; set; }

        [Index(UNIQUE = true), Length(2, 30)]
        public string Nick { get; set; }

        [Length(1, 128)]
        public string Password { get; set; }

        [Length(30)]
        public string Mobile { get; set; }

        [Length(35), Index(UNIQUE = true)]
        public string SessionId { get; set; }

        public DateTime SessionValidUntil { get; set; }

        [HasMany]
        public IList<Bug> Bugs { get; private set; }

        [HasMany]
        public IList<Task> Tasks { get; private set; }

        [HasMany]
        public IList<TaskDescription> TaskDescriptions { get; private set; }

        [HasMany]
        public IList<BugDescription> BugDescriptions { get; private set; }

        [BelongsTo]
        public Project Project { get; set; }

        protected override void OnInserting()
        {
            ProcessPassword();
        }

        protected override void OnUpdating()
        {
            ProcessPassword();
        }

        private void ProcessPassword()
        {
            if (Password != null && Password.Length < 100)
            {
                Password = LoginHelper.GetHashedPassword(Password);
            }
        }

        public static User FindByNick(string nick)
        {
            return FindOne(p => p.Nick == nick);
        }

        public static User GetUserForLogin(string email, string password)
        {
            if (email == null || password == null)
            {
                return null;
            }

            var pass = LoginHelper.GetHashedPassword(password);
            var u = FindOne(p => p.Email == email);
            if (u != null && u.Password != pass)
            {
                return null;
            }
            return u;
        }

        public static User FindBySessionId(string sessionId)
        {
            var user = FindOne(p => p.SessionId == sessionId);
            if(user != null && user.SessionValidUntil > Util.Now)
            {
                return user;
            }
            return null;
        }

        public void ResetSessionId()
        {
            SessionId = Util.NewGuid().ToBase32String();
            SessionValidUntil = Util.Now.AddDays(30);
        }
    }
}
