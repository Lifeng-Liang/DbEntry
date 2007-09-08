
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.Driver;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data.Definition
{
    public interface ILazyLoading
    {
        object Read();
        void Write(object item, bool IsLoad);
        void Init(DbContext context, string ForeignKeyName);
        void Load();
        bool IsLoaded { get; set; }
    }
}
