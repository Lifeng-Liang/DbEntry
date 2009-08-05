using System;
using Lephone.Data.Definition;

namespace Lephone.UnitTest.Data.Objects
{
    [DbTable("People")]
    public abstract class PeopleModel : DbObjectModel<PeopleModel>
    {
        [Length(5)]
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

    public abstract class GuidColumn : DbObjectModel<GuidColumn>
    {
        public abstract Guid TheGuid { get; set; }

        public abstract GuidColumn Init(Guid theGuid);
    }
}
