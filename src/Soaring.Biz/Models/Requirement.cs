using System;
using System.Collections.Generic;
using Lephone.Data.Definition;

namespace Soaring.Biz.Models
{
    public class Requirement : DbObjectModel<Requirement>
    {
        [Length(1, 256)]
        public string Title { get; set; }

        [AllowNull]
        public string Content { get; set; }

        [SpecialName]
        public DateTime CreatedOn { get; set; }

        [SpecialName]
        public DateTime? UpdatedOn { get; set; }

        [HasMany]
        public IList<Task> Tasks { get; set; }

        [BelongsTo]
        public Project Project { get; set; }
    }
}
