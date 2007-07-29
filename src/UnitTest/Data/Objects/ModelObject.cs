
#region usings

using System;
using System.Collections.Generic;
using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.UnitTest.Data.Objects
{
    [DbTable("People")]
    public abstract class PeopleModel : DbObjectModel<PeopleModel>
    {
        [MaxLength(5)]
        public abstract string Name { get; set; }

        public static PeopleModel FindByName(string Name)
        {
            return FindOne(Col["Name"] == Name);
        }

        public static long CountName(string Name)
        {
            return GetCount(Col["Name"] == Name);
        }
    }
}
