using System;

namespace Lephone.Data.Definition
{
    [Serializable]
    public abstract class NamedDbObject : IDbObject
    {
        [DbKey(IsDbGenerate = false), Length(1, 255)]
        public string Name = "";
    }
}
