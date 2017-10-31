using System.Collections.Generic;
using Leafing.Data.Definition;

namespace Leafing.Membership
{
    public class DbEntryRole : DbObjectModel<DbEntryRole>
    {
        [Length(1, 30), Index(UNIQUE = true)]
        public string Name { get; set; }

        [CrossTableName("DbEntryMembershipUser_Role")]
		public HasAndBelongsToMany<DbEntryMembershipUser> Users { get; private set; }
    }
}
