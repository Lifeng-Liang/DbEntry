using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.Builder;
using Lephone.Data.Common;
using Lephone.Data.SqlEntry;

namespace Lephone.Data
{
	[Serializable]
	public class OrderBy : IClause
	{
		public readonly List<ASC> OrderItems;

        public OrderBy(string OrderName)
        {
            OrderItems = new List<ASC>(ParseClause(OrderName));
        }

		public OrderBy(params ASC[] OrderItems)
		{
			this.OrderItems = new List<ASC>(OrderItems);
		}

        public string ToSqlText(DataParameterCollection dpc, Dialect.DbDialect dd)
		{
            if (OrderItems != null && OrderItems.Count > 0)
            {
                var sb = new StringBuilder(" ORDER BY ");
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
            return new OrderBy(ParseClause(OrderByString));
        }

        public static OrderBy Parse(string OrderByString, Type t)
        {
            if (string.IsNullOrEmpty(OrderByString))
            {
                return null;
            }
            return new OrderBy(ParseClause(OrderByString, t));
        }

        private static ASC[] ParseClause(string OrderByString)
        {
            string[] ss = OrderByString.Split(',');
            var ret = new List<ASC>();
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
            return ret.ToArray();
        }

        private static ASC[] ParseClause(string OrderByString, Type t)
        {
            ObjectInfo oi = ObjectInfo.GetInstance(t);
            string[] ss = OrderByString.Split(',');
            var ret = new List<ASC>();
            foreach (string s in ss)
            {
                if (s.ToLower().EndsWith(" desc"))
                {
                    ret.Add(new DESC(GetColumnName(oi, s.Substring(0, s.Length - 5).Trim())));
                }
                else
                {
                    ret.Add(new ASC(GetColumnName(oi, s.Trim())));
                }
            }
            return ret.ToArray();
        }

        private static string GetColumnName(ObjectInfo oi, string Name)
        {
            foreach(MemberHandler mh in oi.Fields)
            {
                if(mh.MemberInfo.Name == Name)
                {
                    return mh.Name;
                }
            }
            throw new DataException("Can not find field [" + Name + "] on [" + oi.HandleType.Name + "]");
        }
    }
}
