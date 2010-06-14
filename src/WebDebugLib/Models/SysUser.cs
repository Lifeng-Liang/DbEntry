using System;
using System.Collections.Generic;
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
        [DbColumn("TheAge")]
        public int Age { get; set; }
        public Date Birthday { get; set; }
        public bool IsMale { get; set; }

        [HasMany]
        public IList<Book> Books { get; set; }
    }
}
