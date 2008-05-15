using Lephone.Data.Definition;

namespace Lephone.DemoObject
{
    [DbTable("People")]
    public class Person : DbObject
    {
        [DbColumn("FullName"), Length(20), AllowNull, Index(ASC = false)]
        public string Name;

        [Index("LinkedIndex", ASC = false)]
        public int Age;

        [Index("LinkedIndex", ASC = false)]
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
