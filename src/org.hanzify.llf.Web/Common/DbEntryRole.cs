
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.Definition;

namespace Lephone.Web.Common
{
    public abstract class DbEntryRole : DbObjectModel<DbEntryRole>
    {
        [Length(1, 30), Index(UNIQUE = true)]
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public abstract IList<DbEntryMembershipUser> Users { get; set; }

        public DbEntryRole Init(string Name)
        {
            this.Name = Name;
            return this;
        }
    }
}
