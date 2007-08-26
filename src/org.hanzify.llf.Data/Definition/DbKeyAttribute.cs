
using System;

namespace org.hanzify.llf.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class DbKeyAttribute : Attribute
	{
		public bool IsDbGenerate;
        public object UnsavedValue;

		public DbKeyAttribute()
		{
            IsDbGenerate = true;
            UnsavedValue = null;
		}
	}
}
