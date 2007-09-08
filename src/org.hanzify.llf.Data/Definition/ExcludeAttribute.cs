
using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class ExcludeAttribute : Attribute
	{
		public ExcludeAttribute()
		{
		}
	}
}
