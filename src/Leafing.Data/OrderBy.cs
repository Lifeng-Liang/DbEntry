using System;
using System.Collections.Generic;
using System.Text;
using Leafing.Data.Builder;
using Leafing.Data.Model;
using Leafing.Data.Model.Member;
using Leafing.Data.SqlEntry;

namespace Leafing.Data
{
	[Serializable]
	public class OrderBy : IClause
	{
		public readonly List<ASC> OrderItems;

        public OrderBy(string orderName)
        {
            OrderItems = new List<ASC>(ParseClause(orderName));
        }

		public OrderBy(params ASC[] orderItems)
		{
			this.OrderItems = new List<ASC>(orderItems);
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

        public static OrderBy Parse(string orderByString)
        {
            if(string.IsNullOrEmpty(orderByString))
            {
                return null;
            }
            return new OrderBy(ParseClause(orderByString));
        }

        public static OrderBy Parse(string orderByString, Type t)
        {
            if (string.IsNullOrEmpty(orderByString))
            {
                return null;
            }
            return new OrderBy(ParseClause(orderByString, t));
        }

        private static ASC[] ParseClause(string orderByString)
        {
            string[] ss = orderByString.Split(',');
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

        private static ASC[] ParseClause(string orderByString, Type t)
        {
            var ctx = ModelContext.GetInstance(t);
            string[] ss = orderByString.Split(',');
            var ret = new List<ASC>();
            foreach (string s in ss)
            {
                if (s.ToLower().EndsWith(" desc"))
                {
                    ret.Add(new DESC(GetColumnName(ctx.Info, s.Substring(0, s.Length - 5).Trim())));
                }
                else
                {
                    ret.Add(new ASC(GetColumnName(ctx.Info, s.Trim())));
                }
            }
            return ret.ToArray();
        }

        private static string GetColumnName(ObjectInfo oi, string name)
        {
            foreach(MemberHandler mh in oi.Members)
            {
                if(mh.MemberInfo.Name == name)
                {
                    return mh.Name;
                }
            }
            throw new DataException("Can not find field [" + name + "] on [" + oi.HandleType.Name + "]");
        }
    }
}
