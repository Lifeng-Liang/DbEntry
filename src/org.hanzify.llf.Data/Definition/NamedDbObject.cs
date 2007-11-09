
#region usings

using System;
using Lephone.Data.Common;

#endregion

namespace Lephone.Data.Definition
{
    [Serializable]
    public abstract class NamedDbObject : IDbObject
    {
        [DbKey(IsDbGenerate = false), Length(1, 255)]
        public string Name = "";
    }
}
