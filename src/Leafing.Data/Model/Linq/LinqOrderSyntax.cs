using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Leafing.Data.Definition;

namespace Leafing.Data.Model.Linq
{
    public class LinqOrderSyntax<T> where T : class, IDbObject
    {
        private OrderBy _order;

        public LinqOrderSyntax(Expression<Func<T, object>> expr, bool isAsc)
        {
            AddOrderBy(expr, isAsc);
        }

        public List<T> Find(Expression<Func<T, bool>> condition)
        {
            return DbEntry.From<T>().Where(condition).OrderBy(_order).Select();
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

        private void AddOrderBy(LambdaExpression expr, bool isAsc)
        {
            MemberExpression e = expr.GetMemberExpression();
            if (e == null)
            {
                throw new LinqException("Order By error!");
            }
            string n = ExpressionParser<T>.GetColumnName(e.Member.Name);
            AddOrderBy(isAsc ? new ASC(n) : new DESC(n));
        }

        private void AddOrderBy(ASC item)
        {
            if (_order == null)
            {
                _order = new OrderBy();
            }
            _order.OrderItems.Add(item);
        }
    }
}