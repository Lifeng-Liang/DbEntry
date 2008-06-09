using System.Collections.Generic;
using Lephone.Data.Definition;

namespace Lephone.Web.Common
{
    public abstract class DbEntryRole : DbObjectModel<DbEntryRole>
    {
        [Length(1, 30), Index(UNIQUE = true)]
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public abstract IList<DbEntryMembershipUser> Users { get; set; }

        public DbEntryRole Init(string name)
        {
            this.Name = name;
            return this;
        }
    }
}
