using System;
using System.Linq.Expressions;
using Lephone.Data.Definition;

namespace Lephone.Data.Linq
{
    public class LinqExpressionParser<T> where T : class, IDbObject
    {
        public Condition condition;
        public OrderBy orderby;

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
                case "Select":
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AddOrderBy(LambdaExpression expr, bool IsAsc)
        {
            var e = expr.Body as MemberExpression;
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