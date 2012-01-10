using System;

namespace Leafing.Data.Definition
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class DbTableAttribute : Attribute
	{
        public string TableName;
	    public Type PartOf;

		public DbTableAttribute(string tableName)
		{
            this.TableName = tableName;
		}

        public DbTableAttribute(Type partOf)
        {
            this.PartOf = partOf;
        }
    }
}
