using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class IndexAttribute : Attribute
    {
        public string IndexName;
        public bool ASC = true;
        public bool UNIQUE;

        public IndexAttribute() { }

        public IndexAttribute(string IndexName)
        {
            this.IndexName = IndexName;
        }

        public IndexAttribute(string IndexName, bool ASC)
        {
            this.IndexName = IndexName;
            this.ASC = ASC;
        }

        public IndexAttribute(bool ASC)
        {
            this.ASC = ASC;
        }
    }
}

