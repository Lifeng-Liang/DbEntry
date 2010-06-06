using System;
using Lephone.Data.Definition;

namespace DebugLib.Models
{
    public enum Gender
    {
        Male,
        Female,
    }

    public class SysUser : DbObjectModel<SysUser>
    {
        [Length(20)]
        public string Name { get; set; }
        public int Age { get; set; }
        public Date Birthday { get; set; }
        public bool IsMale { get; set; }

        [HasOne(OrderBy = "Id")]
        public Book Book { get; set; }
    }
}
