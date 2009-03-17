using System;

namespace Lephone.Util.Text
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ShowStringAttribute : Attribute
	{
		private readonly string _ShowString;

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
