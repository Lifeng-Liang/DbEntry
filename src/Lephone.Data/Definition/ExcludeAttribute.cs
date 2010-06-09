using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
	public class ExcludeAttribute : Attribute
	{
	}
}
