using System;

namespace Leafing.Data.Definition
{
    public class OrderByAttribute : Attribute
    {
        public string OrderBy;

		public OrderByAttribute(string orderBy)
		{
			this.OrderBy = orderBy;
		}
    }
}
