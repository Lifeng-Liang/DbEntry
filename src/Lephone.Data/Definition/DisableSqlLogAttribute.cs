using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class DisableSqlLogAttribute : Attribute
    {
    }
}
