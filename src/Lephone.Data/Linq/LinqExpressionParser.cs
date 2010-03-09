using System;
using System.Linq.Expressions;
using Lephone.Data.Definition;

namespace Lephone.Data.Linq
{
    public class LinqExpressionParser<T> where T : class, IDbObject
    {
        public Condition Condition;
        public OrderBy Orderby;

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
                    AddOrderBy(expr, true);
                    break;
                case "OrderByDescending":
                case "ThenByDescending":
                    Parse(expr.Arguments[0]);
                    AddOrderBy(expr, false);
                    break;
                case "Where":
                    Condition = ExpressionParser<T>.Parse(
                        (Expression<Func<T, bool>>)((UnaryExpression)expr.Arguments[1]).Operand
                        );
                    break;
                case "Select":
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AddOrderBy(MethodCallExpression expression, bool isAsc)
        {
            var expr = (LambdaExpression)((UnaryExpression)expression.Arguments[1]).Operand;
            var e = expr.Body as MemberExpression;
            if (e != null)
            {
                string n = ExpressionParser<T>.GetColumnName(e.Member.Name);
                AddOrderBy(isAsc ? new ASC(n) : new DESC(n));
            }
        }

        private void AddOrderBy(ASC item)
        {
            if (Orderby == null)
            {
                Orderby = new OrderBy();
            }
            Orderby.OrderItems.Add(item);
        }
    }
}