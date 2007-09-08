
#region usings

using System;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data.Definition
{
    [Serializable]
    public abstract class NamedDbObject
    {
        [DbKey(IsDbGenerate=false), MaxLength(255)]
        public string Name = "";
    }
}
