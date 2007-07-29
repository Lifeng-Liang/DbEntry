
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using org.hanzify.llf.Data.Driver;
using org.hanzify.llf.Data.Common;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    public interface ILazyLoading
    {
        object Read();
        void Write(object item, bool IsLoad);
        void SetOwner(object owner, string ColumnName);
        void Init(DbDriver driver, string ForeignKeyName);
        void Load();
        bool IsLoaded { get; set; }
    }
}
