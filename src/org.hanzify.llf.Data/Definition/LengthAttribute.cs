using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class LengthAttribute : Attribute
    {
        public int Min;
        public int Max;

        public LengthAttribute(int Max)
        {
            this.Min = 0;
            this.Max = Max;
        }

        public LengthAttribute(int Min, int Max)
        {
            this.Min = Min;
            this.Max = Max;
        }
    }
}
