
#region usings

using System;
using System.Collections.Generic;
using Lephone.Data;
using Lephone.Data.Definition;

#endregion

namespace Lephone.UnitTest.Data.Objects
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

    public abstract class GuidKey : DbObjectModel<GuidKey, Guid>
    {
        public abstract string Name { get; set; }
    }
}
