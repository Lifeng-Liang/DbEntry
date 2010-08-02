using System.Collections.Generic;
using Lephone.Data.Definition;
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

        [Length(100)]
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

        public static User FindByNick(string nick)
        {
            return FindOne(p => p.Nick == nick);
        }
    }
}
