
#region usings

using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;

#endregion

namespace Lephone.Data
{
	[Serializable]
	public class OrderBy : IClause
	{
		public ASC[] OrderItems;

        public OrderBy(string OrderName)
            : this(new ASC(OrderName))
        {
        }

		public OrderBy(params ASC[] OrderItems)
		{
			this.OrderItems = OrderItems;
		}

		public string ToSqlText(DataParamterCollection dpc, Dialect.DbDialect dd)
		{
            if (OrderItems != null && OrderItems.Length > 0)
            {
                StringBuilder sb = new StringBuilder(" Order By ");
                foreach (ASC oi in OrderItems)
                {
                    sb.Append(oi.ToString(dd));
                    sb.Append(",");
                }
                if (sb.Length > 10) { sb.Length--; }
                return sb.ToString();
            }
            return "";
		}

        public static OrderBy Parse(string OrderByString)
        {
            if(string.IsNullOrEmpty(OrderByString))
            {
                return null;
            }
            string[] ss = OrderByString.Split(',');
            List<ASC> ret = new List<ASC>();
            foreach (string s in ss)
            {
                if (s.ToLower().EndsWith(" desc"))
                {
                    ret.Add(new DESC(s.Substring(0, s.Length - 5).Trim()));
                }
                else
                {
                    ret.Add(new ASC(s.Trim()));
                }
            }
            return new OrderBy(ret.ToArray());
        }
	}
}
