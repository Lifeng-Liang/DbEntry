
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
            var me = (QueryContent<T>)t;
            UnaryExpression e1 = expr.Body as UnaryExpression;
            if (e1 != null)
            {
                MemberExpression e = e1.Operand as MemberExpression;
                if (e != null)
                {
                    string n = ExpressionParser<T>.GetColumnName(e.Member.Name);
                    me.m_order = new OrderBy(n);
                    return me;
                }
            }
            throw new LinqException("OrderBy error!");
        }

        public static T GetObject<T>(this DbContext c, Expression<Func<T, bool>> expr) where T : IDbObject
        {
            var wc = ExpressionParser<T>.Parse(expr);
            return c.GetObject<T>(wc);
        }
    }
}
