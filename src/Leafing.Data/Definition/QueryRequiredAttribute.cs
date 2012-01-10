using System;

namespace Leafing.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class QueryRequiredAttribute : Attribute
    {
    }
}
