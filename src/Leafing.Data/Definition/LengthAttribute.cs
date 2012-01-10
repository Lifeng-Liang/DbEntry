using System;

namespace Leafing.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class LengthAttribute : Attribute
    {
        public int Min;
        public int Max;
        public string ErrorMessage;

        public LengthAttribute(int max)
        {
            this.Min = 0;
            this.Max = max;
        }

        public LengthAttribute(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }
    }
}
