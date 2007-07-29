
#region usings

using System;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.DemoObject
{
    [DbTable("Departments")]
    public class Department : DbObject
    {
        [MaxLength(30)]
        public string Name;

        public Department(string Name)
        {
            this.Name = Name;
        }
    }
}
