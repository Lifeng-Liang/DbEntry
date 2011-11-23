using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class QueryRequiredAttribute : Attribute
    {
    }
}
