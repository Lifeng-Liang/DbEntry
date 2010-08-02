using System.Collections.Generic;
using Lephone.Data.Definition;
using Soaring.Biz.Models.Enums;

namespace Soaring.Biz.Models
{
    public class Bug : DbObjectModel<Bug>
    {
        public BugStatus Status { get; set; }

        [BelongsTo]
        public Task Task { get; set; }

        [HasMany]
        public IList<BugDescription> Descriptions { get; set; }

        [BelongsTo]
        public User AssignTo { get; set; }
    }
}
