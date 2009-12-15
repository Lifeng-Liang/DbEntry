using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.Definition;

namespace DebugLib
{
    public abstract class User : DbObjectModel<User>
    {
        public abstract string Name { get; set; }

        public abstract User Init(string name);

        [BelongsTo]
        public abstract Books Book { get; set; }
    }

    public abstract class Books : DbObjectModel<Books>
    {
        public abstract string Name { get; set; }

        public abstract Books Init(string name);

        [HasOne]
        public abstract User User { get; set; }
    }
}
