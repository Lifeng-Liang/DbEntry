using System;
using Lephone.Data.Common;
using System.Linq.Expressions;
using Lephone.Data.Definition;

namespace Lephone.Data.Linq
{
    public class LinqOrderSyntax<T> where T : class, IDbObject
    {
        private OrderBy order;

        public LinqOrderSyntax(Expression<Func<T, object>> expr, bool isAsc)
        {
            AddOrderBy(expr, isAsc);
        }

        public DbObjectList<T> Find(Expression<Func<T, bool>> condition)
        {
            return DbEntry.From<T>().Where(condition).OrderBy(order).Select();
        }

        public LinqOrderSyntax<T> ThenBy(Expression<Func<T, object>> expr)
        {
            AddOrderBy(expr, true);
            return this;
        }

        public LinqOrderSyntax<T> ThenByDescending(Expression<Func<T, object>> expr)
        {
            AddOrderBy(expr, false);
            return this;
        }

        private void AddOrderBy(LambdaExpression expr, bool IsAsc)
        {
            MemberExpression e = expr.GetMemberExpression();
            if (e == null)
            {
                throw new LinqException("Order By error!");
            }
            string n = ExpressionParser<T>.GetColumnName(e.Member.Name);
            AddOrderBy(IsAsc ? new ASC(n) : new DESC(n));
        }

        private void AddOrderBy(ASC item)
        {
            if (order == null)
            {
                order = new OrderBy();
            }
            order.OrderItems.Add(item);
        }
    }
}