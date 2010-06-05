using System;
using Lephone.Data.Definition;

namespace Lephone.UnitTest.Data.Objects
{
    [DbTable("People")]
    public class PeopleModel : DbObjectModel<PeopleModel>
    {
        [Length(5)]
        public string Name { get; set; }

        public static PeopleModel FindByName(string name)
        {
            return FindOne(Col["Name"] == name);
        }

        public static long CountName(string name)
        {
            return GetCount(Col["Name"] == name);
        }
    }

    public class GuidKey : DbObjectModel<GuidKey, Guid>
    {
        public string Name { get; set; }
    }

    public class GuidColumn : DbObjectModel<GuidColumn>
    {
        public Guid TheGuid { get; set; }
    }
}
