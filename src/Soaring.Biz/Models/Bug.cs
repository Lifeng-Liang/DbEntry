using System.Collections.Generic;
using Lephone.Data.Definition;
using Soaring.Biz.Models.Enums;

namespace Soaring.Biz.Models
{
    public class Bug : DbObjectModel<Bug>
    {
        [Length(1, 256)]
        public string Title { get; set; }

        public BugStatus Status { get; set; }

        [BelongsTo]
        public Task Task { get; set; }

        [HasMany]
        public IList<BugDescription> Descriptions { get; private set; }

        [BelongsTo]
        public User AssignTo { get; set; }
    }
}
