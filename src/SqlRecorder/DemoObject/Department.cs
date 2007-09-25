
#region usings

using System;
using Lephone.Data.Definition;

#endregion

namespace Lephone.DemoObject
{
    [DbTable("Departments")]
    public class Department : DbObject
    {
        [Length(30)]
        public string Name;

        public Department(string Name)
        {
            this.Name = Name;
        }
    }
}
