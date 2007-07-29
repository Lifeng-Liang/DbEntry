
using System;

namespace org.hanzify.llf.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class DbKeyAttribute : Attribute
	{
		public bool IsSystemGeneration;
        public object UnsavedValue;

		public DbKeyAttribute()
		{
            IsSystemGeneration = true;
            UnsavedValue = null;
		}
	}
}
