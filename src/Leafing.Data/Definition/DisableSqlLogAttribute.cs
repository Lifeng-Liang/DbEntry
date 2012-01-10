using System;

namespace Leafing.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class DisableSqlLogAttribute : Attribute
    {
    }
}
