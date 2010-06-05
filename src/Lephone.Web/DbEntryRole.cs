using System.Collections.Generic;
using Lephone.Data.Definition;

namespace Lephone.Web
{
    public class DbEntryRole : DbObjectModel<DbEntryRole>
    {
        [Length(1, 30), Index(UNIQUE = true)]
        public string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id", CrossTableName = "DbEntryMembershipUser_Role")]
        public IList<DbEntryMembershipUser> Users { get; set; }
    }
}
