using System.Collections.Generic;
using Lephone.Data.Definition;

namespace Soaring.Biz.Models
{
    public class Project : DbObjectModel<Project>
    {
        [Length(1, 50)]
        public string Name { get; set; }

        [HasMany]
        public IList<Requirement> Requirements { get; private set; }

        [HasMany]
        public IList<User> Users { get; private set; }
    }
}
