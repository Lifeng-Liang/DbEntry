using System;

namespace Leafing.Data.Definition {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class PrecisionAttribute : Attribute {
        public int IntegralPart;
        public int DecimalPart;

        public PrecisionAttribute(int integralPart, int decimalPart) {
            this.IntegralPart = integralPart;
            this.DecimalPart = decimalPart;
        }
    }
}
