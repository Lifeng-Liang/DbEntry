using Lephone.Data.Definition;

namespace Soaring.Biz.Models
{
    public class BugDescription : DbObjectModel<BugDescription>
    {
        public string Content { get; set; }

        [BelongsTo]
        public User User { get; set; }

        [BelongsTo]
        public Bug Bug { get; set; }
    }
}
