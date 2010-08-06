using System.Collections.Generic;
using System.Text;
using Lephone.Core.Logging;
using Lephone.Core.Text;
using Lephone.Data.Definition;
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

        [HasMany]
        public IList<Bug> Bugs { get; set; }

        [HasMany]
        public IList<Task> Tasks { get; set; }

        [HasMany]
        public IList<TaskDescription> TaskDescriptions { get; set; }

        [HasMany]
        public IList<BugDescription> BugDescriptions { get; set; }

        [BelongsTo]
        public Project Project { get; set; }

        public override void Save()
        {
            if (Password != null && Password.Length < 100)
            {
                Password = CommonHelper.GetHashedPassword(Password);
            }
            base.Save();
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

            var pass = CommonHelper.GetHashedPassword(password);
            var u = FindOne(p => p.Email == email);
            if (u != null)
            {
                if (u.Password != pass)
                {
                    return null;
                }
            }
            return u;
        }

        public static string SerializeToString(string email, string password)
        {
            var s = string.Format("{0}\n{1}", email, password);
            var bs = Encoding.UTF8.GetBytes(s);
            return Base32StringCoding.Decode(bs);
        }

        public static User DeserializeFromString(string source)
        {
            var bs = Base32StringCoding.Encode(source);
            var s = Encoding.UTF8.GetString(bs);
            var ss = s.Split('\n');
            User user = null;
            if (ss.Length == 2)
            {
                user = GetUserForLogin(ss[0], ss[1]);
            }
            if (user == null)
            {
                Logger.System.Trace(string.Format("CAN NOT FOUND USER : {0},{1}", source, s));
            }
            return user;
        }
    }
}
