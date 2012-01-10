using System.Collections.Generic;
using Leafing.Data.Definition;

namespace Leafing.Web
{
    public class DbEntryRole : DbObjectModel<DbEntryRole>
    {
        [Length(1, 30), Index(UNIQUE = true)]
        public string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id", CrossTableName = "DbEntryMembershipUser_Role")]
        public IList<DbEntryMembershipUser> Users { get; private set; }
    }
}
