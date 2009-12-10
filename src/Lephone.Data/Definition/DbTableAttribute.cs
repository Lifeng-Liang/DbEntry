using System;

namespace Lephone.Data.Definition
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class DbTableAttribute : Attribute
	{
        public string TableName;

		public DbTableAttribute(string tableName)
		{
            this.TableName = tableName;
		}
    }
}
