using System;

namespace Leafing.Core.Text
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
	public class ShowStringAttribute : Attribute
	{
		private readonly string _showString;

		public string ShowString
		{
			get { return _showString; }
		}

		public ShowStringAttribute(string showString)
		{
			_showString = showString; 
		}
	}
}
