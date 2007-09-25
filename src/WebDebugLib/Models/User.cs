
using System;
using Lephone.Data.Definition;

namespace DebugLib.Models
{
    public enum Gender
    {
        Male,
        Female,
    }

    public abstract class User : DbObjectModel<User>
    {
        [Length(20)]
        public abstract string Name { get; set; }
        public abstract int Age { get; set; }
        public abstract DateTime Birthday { get; set; }
        public abstract bool IsMale { get; set; }

        public User Init(string Name, int Age, DateTime Birthday, bool IsMale)
        {
            this.Name = Name;
            this.Age = Age;
            this.Birthday = Birthday;
            this.IsMale = IsMale;
            return this;
        }
    }
}
