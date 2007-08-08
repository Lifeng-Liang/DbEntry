
using System;
using org.hanzify.llf.Data.Definition;

namespace test
{
    public enum Gender
    {
        Male,
        Female,
    }

    public abstract class User : DbObjectModel<User>
    {
        [MaxLength(20)]
        public abstract string Name { get; set; }
        public abstract int Age { get; set; }
        public abstract DateTime Birthday { get; set; }
        public abstract Gender Gender { get; set; }

        public User() { }

        public User(string Name, int Age, DateTime Birthday, Gender Gender)
        {
            this.Name = Name;
            this.Age = Age;
            this.Birthday = Birthday;
            this.Gender = Gender;
        }
    }
}
