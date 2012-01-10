using System;

namespace Leafing.Web
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class HttpParameterAttribute : Attribute
    {
        public bool AllowEmpty;
    }
}
