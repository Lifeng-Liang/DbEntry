using Leafing.Data.Definition;

namespace Leafing.UnitTest.Data.Objects
{
    [DbTable("People")]
    public class PropertyClassBase : DbObject
    {
        public virtual string Name { get; set; }
    }

    [DbTable("People")]
    public class PropertyClassImpl : PropertyClassBase
    {
        public override string Name { get; set; }
    }

    [DbTable("People")]
    public class PropertyClassWithDbColumn : DbObject
    {
        [DbColumn("Name")]
        public string TheName { get; set; }
    }

    [DbTable("People"), DbContext("SqlServerMock")]
    public class PropertyClassWithDbColumnSql : DbObject
    {
        [DbColumn("Name")]
        public string TheName { get; set; }
    }
}
