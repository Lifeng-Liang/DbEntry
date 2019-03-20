using System;

namespace Leafing.Core.Ioc {
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class InjectionAttribute : Attribute {
        public readonly int Index;

        public InjectionAttribute() {
            this.Index = 0;
        }

        public InjectionAttribute(int index) {
            this.Index = index;
        }
    }
}
