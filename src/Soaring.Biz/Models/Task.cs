using System;
using System.Collections.Generic;
using Lephone.Data.Definition;
using Soaring.Biz.Models.Enums;

namespace Soaring.Biz.Models
{
    public class Task : DbObjectModel<Task>
    {
        [Length(1, 256)]
        public string Title { get; set; }

        public TaskStatus Status { get; set; }

        public int Score { get; set; }

        [SpecialName]
        public DateTime CreatedOn { get; set; }

        [SpecialName]
        public DateTime? UpdatedOn { get; set; }

        [BelongsTo]
        public Requirement Requirement { get; set; }

        [HasMany]
        public IList<Bug> Bugs { get; set; }

        [HasMany]
        public IList<TaskDescription> Descriptions { get; set; }

        [BelongsTo]
        public User AssignTo { get; set; }
    }
}
