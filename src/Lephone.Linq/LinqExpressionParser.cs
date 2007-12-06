
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Lephone.Data;
using System.Reflection;
using Lephone.Data.Definition;

namespace Lephone.Linq
{
    internal class LinqExpressionParser<T> where T : IDbObject
    {
        public WhereCondition condition = null;
        public OrderBy orderby = null;

        public LinqExpressionParser(Expression expr)
        {
            Parse(expr);
        }

        private void Parse(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Call:
                    ParseCall((MethodCallExpression)expr);
                    break;
            }
        }

        private void ParseCall(MethodCallExpression expr)
        {
            switch (expr.Method.Name)
            {
                case "OrderBy":
                case "ThenBy":
                    Parse(expr.Arguments[0]);
                    AddOrderBy(
                        (LambdaExpression)((UnaryExpression)expr.Arguments[1]).Operand, true
                        );
                    break;
                case "OrderByDescending":
                case "ThenByDescending":
                    Parse(expr.Arguments[0]);
                    AddOrderBy(
                        (LambdaExpression)((UnaryExpression)expr.Arguments[1]).Operand, false
                        );
                    break;
                case "Where":
                    condition = ExpressionParser<T>.Parse(
                        (Expression<Func<T, bool>>)((UnaryExpression)expr.Arguments[1]).Operand
                        );
                    break;
                default:
                    throw new NotImplementedException();
            }
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
            if (orderby == null)
            {
                orderby = new OrderBy();
            }
            orderby.OrderItems.Add(item);
        }
    }
}
