
using System;
using Lephone.Data.Definition;

namespace DebugLib.Models
{
    public enum Gender
    {
        Male,
        Female,
    }

    public abstract class SysUser : DbObjectModel<SysUser>
    {
        [Length(20)]
        public abstract string Name { get; set; }
        public abstract int Age { get; set; }
        public abstract Date Birthday { get; set; }
        public abstract bool IsMale { get; set; }

        public SysUser Init(string Name, int Age, Date Birthday, bool IsMale)
        {
            this.Name = Name;
            this.Age = Age;
            this.Birthday = Birthday;
            this.IsMale = IsMale;
            return this;
        }
    }
}
