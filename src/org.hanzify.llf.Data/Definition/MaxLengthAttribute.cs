
#region usings

using System;

#endregion

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class MaxLengthAttribute : Attribute
    {
        public int Value = 0;

        public MaxLengthAttribute(int Value)
        {
            this.Value = Value;
        }
    }
}
