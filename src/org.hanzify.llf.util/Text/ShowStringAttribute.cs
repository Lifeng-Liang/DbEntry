
using System;

namespace org.hanzify.llf.util.Text
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ShowStringAttribute : Attribute
	{
		private string _ShowString;

		public string ShowString
		{
			get { return _ShowString; }
		}

		public ShowStringAttribute(string ShowString)
		{
			_ShowString = ShowString; 
		}
	}
}
