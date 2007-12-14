
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Lephone.Data;
using Lephone.Data.QuerySyntax;
using Lephone.Data.Definition;
using Lephone.Util;

namespace Lephone.Linq
{
    public static class QueryExtends
    {
        public static IAfterWhere<T> Where<T>(this IWhere<T> t, Expression<Func<T, bool>> expr) where T : IDbObject
        {
            var me = (QueryContent<T>)t;
            me.m_where = ExpressionParser<T>.Parse(expr);
            return me;
        }

        public static IRangeable<T> OrderBy<T>(this IAfterWhere<T> t, Expression<Func<T, object>> expr) where T : IDbObject
        {
            return AddOrderBy<T>((QueryContent<T>)t, expr, true);
        }

        public static IRangeable<T> OrderByDescending<T>(this IAfterWhere<T> t, Expression<Func<T, object>> expr) where T : IDbObject
        {
            return AddOrderBy<T>((QueryContent<T>)t, expr, false);
        }

        public static IRangeable<T> ThenBy<T>(this IRangeable<T> t, Expression<Func<T, object>> expr) where T : IDbObject
        {
            return AddOrderBy<T>((QueryContent<T>)t, expr, true);
        }

        public static IRangeable<T> ThenByDescending<T>(this IRangeable<T> t, Expression<Func<T, object>> expr) where T : IDbObject
        {
            return AddOrderBy<T>((QueryContent<T>)t, expr, false);
        }

        private static IRangeable<T> AddOrderBy<T>(QueryContent<T> me, LambdaExpression expr, bool isAsc) where T : IDbObject
        {
            MemberExpression e = GetMemberExpression(expr);
            if (e != null)
            {
                string n = ExpressionParser<T>.GetColumnName(e.Member.Name);
                if (me.m_order == null)
                {
                    me.m_order = new OrderBy();
                }
                me.m_order.OrderItems.Add(isAsc ? new ASC(n) : new DESC(n));
                return me;
            }
            throw new LinqException("OrderBy error!");
        }

        internal static MemberExpression GetMemberExpression(LambdaExpression expr)
        {
            if (expr.Body is MemberExpression)
            {
                return (MemberExpression)expr.Body;
            }
            if (expr.Body is UnaryExpression)
            {
                return (MemberExpression)((UnaryExpression)expr.Body).Operand;
            }
            return null;
        }

        public static T GetObject<T>(this DbContext c, Expression<Func<T, bool>> expr) where T : IDbObject
        {
            var wc = ExpressionParser<T>.Parse(expr);
            return c.GetObject<T>(wc);
        }
    }
}
