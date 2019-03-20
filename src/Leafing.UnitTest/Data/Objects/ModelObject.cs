using System;
using Leafing.Data.Definition;

namespace Leafing.UnitTest.Data.Objects {
    [DbTable("People")]
    public class PeopleModel : DbObjectModel<PeopleModel> {
        [Length(5)]
        public string Name { get; set; }

        public static PeopleModel FindByName(string name) {
            return FindOne(Col["Name"] == name);
        }

        public static long CountName(string name) {
            return GetCount(Col["Name"] == name);
        }
    }

    [DbTable("People"), DbContext("Firebird")]
    public class PeopleModel2 : DbObjectModel<PeopleModel2> {
        [Length(5)]
        public string Name { get; set; }
    }

    public class GuidKey : DbObjectModel<GuidKey, Guid> {
        public string Name { get; set; }
    }

    [DbContext("SQLite")]
    public class GuidKeySqlite : DbObjectModel<GuidKeySqlite, Guid> {
        public string Name { get; set; }
    }

    public class GuidColumn : DbObjectModel<GuidColumn> {
        public Guid TheGuid { get; set; }
    }
}