
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lephone.Data.Common;
using System.Linq.Expressions;
using Lephone.Data;
using Lephone.Data.Definition;

namespace Lephone.Linq
{
    public class LinqOrderSytax<T> where T : IDbObject
    {
        private OrderBy order = null;

        public LinqOrderSytax(Expression<Func<T, object>> expr)
        {
            AddOrderBy(expr, true);
        }

        public DbObjectList<T> Find(Expression<Func<T, bool>> condition)
        {
            return DbEntry.From<T>().Where(condition).OrderBy(order).Select();
        }

        public LinqOrderSytax<T> ThenBy(Expression<Func<T, object>> expr)
        {
            // TODO: DESC
            AddOrderBy(expr, true);
            return this;
        }

        private void AddOrderBy(LambdaExpression expr, bool IsAsc)
        {
            MemberExpression e = expr.Body as MemberExpression;
            if (e != null)
            {
                string n = ExpressionParser<T>.GetColumnName(e.Member.Name);
                AddOrderBy(IsAsc ? new ASC(n) : new DESC(n));
            }
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
