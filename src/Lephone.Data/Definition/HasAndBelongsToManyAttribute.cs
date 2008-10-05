using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HasAndBelongsToManyAttribute : OrderByAttribute
    {
        public string CrossTableName;
    }
}
