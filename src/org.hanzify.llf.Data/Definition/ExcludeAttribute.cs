
using System;

namespace org.hanzify.llf.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class ExcludeAttribute : Attribute
	{
		public ExcludeAttribute()
		{
		}
	}
}
