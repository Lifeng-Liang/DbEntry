using Lephone.Data.Definition;

namespace Soaring.Biz.Models
{
    public class TaskDescription : DbObjectModel<TaskDescription>
    {
        public string Content { get; set; }

        [BelongsTo]
        public User User { get; set; }

        [BelongsTo]
        public Task Task { get; set; }
    }
}
