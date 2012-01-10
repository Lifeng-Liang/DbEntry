using System;
using System.Reflection;

namespace Leafing.Core.Ioc
{
    internal class PropertyInjector
    {
        public readonly PropertyInfo Property;
        public readonly Type Type;
        public readonly int Index;

        public PropertyInjector(PropertyInfo property, int index)
        {
            this.Property = property;
            this.Type = property.PropertyType;
            this.Index = index;
        }

        public void SetValue(object obj, object value)
        {
            Property.SetValue(obj, value, null);
        }
    }
}
