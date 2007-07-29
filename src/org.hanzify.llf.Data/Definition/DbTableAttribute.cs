
using System;

namespace org.hanzify.llf.Data.Definition
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class DbTableAttribute : Attribute
	{
        public string TableName;
        public string[] LinkNames;

		public DbTableAttribute(string TableName)
		{
            this.TableName = TableName;
		}

        public DbTableAttribute(params string[] LinkNames)
        {
            this.LinkNames = LinkNames;
        }
    }
}
