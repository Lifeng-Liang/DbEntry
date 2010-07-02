using System;
using Lephone.Data.Definition;

namespace Soaring.Biz.Models
{
    public enum WorkitemStage
    {
        Proposed,
        Working,
        ReadyForTest,
        Complated,
    }

    public class Workitem : DbObjectModel<Workitem>
    {
        [Length(1, 256)]
        public string Title { get; set; }

        [AllowNull]
        public string Content { get; set; }

        [DbColumn("Stage")]
        public WorkitemStage Status { get; set; }

        [SpecialName]
        public DateTime CreatedOn { get; set; }

        [SpecialName]
        public DateTime? UpdatedOn { get; set; }
    }
}
