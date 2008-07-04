using System;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class DbColumnAttribute : Attribute
	{
		private readonly string _Name;

		public string Name
		{
			get { return _Name; }
		}

		public DbColumnAttribute(string Name)
		{
			_Name = Name;
		}
	}
}
