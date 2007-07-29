
#region usings

using System;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.DemoObject
{
    [DbTable("People")]
    public class Person : DbObject
    {
        [DbColumn("FullName"), MaxLength(20), AllowNull, Index(ASC = false)]
        public string Name;

        [Index("LinkedIndex", ASC = false)]
        public int Age;

        [Index("LinkedIndex")]
        public bool? Male;

        public int DepartmentId;

        public Person() { }

        public Person(int Age, bool Male, string Name)
        {
            this.Age = Age;
            this.Male = Male;
            this.Name = Name;
        }
    }
}
