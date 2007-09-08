
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using Lephone.Data.Definition;

#endregion

namespace Lephone.UnitTest.Data.Objects
{
    [DbTable("People")]
    public abstract class PropertyClassBase : DbObject
    {
        public abstract string Name { get; set; }

        public PropertyClassBase() {}

        public PropertyClassBase(string Name)
        {
            this.Name = Name;
        }
    }

    [DbTable("People")]
    public class PropertyClassImpl : PropertyClassBase
    {
        private string _Name;

        public override string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public PropertyClassImpl() {}

        public PropertyClassImpl(string Name)
            : base(Name)
        {
        }
    }

    [DbTable("People")]
    public abstract class PropertyClassWithDbColumn : DbObject
    {
        [DbColumn("Name")]
        public abstract string TheName { get; set; }

        public PropertyClassWithDbColumn() { }

        public PropertyClassWithDbColumn(string Name)
        {
            this.TheName = Name;
        }
    }
}
