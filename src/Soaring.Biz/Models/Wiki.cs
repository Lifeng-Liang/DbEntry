using Lephone.Data.Definition;

namespace Soaring.Biz.Models
{
    public class Wiki : DbObjectModel<Wiki>
    {
        [Length(1,256)]
        public string Title { get; set; }

        public string Content { get; set; }
    }
}
