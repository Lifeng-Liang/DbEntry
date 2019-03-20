using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Leafing.Data.Model.Handler;

namespace Leafing.Data.Model.Linq {
    public class LinqExpressionParser<T> {
        public Condition Condition;
        public OrderBy Orderby;

        public LinqExpressionParser(Expression expr) {
            Parse(expr);
        }

        private void Parse(Expression expr) {
            switch (expr.NodeType) {
                case ExpressionType.Call:
                    ParseCall((MethodCallExpression)expr);
                    break;
            }
        }

        private void ParseCall(MethodCallExpression expr) {
            switch (expr.Method.Name) {
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
                    Parse(expr.Arguments[0]);
                    ProcessSelectColumns(expr.Arguments[1]);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AddOrderBy(MethodCallExpression expression, bool isAsc) {
            var expr = (LambdaExpression)((UnaryExpression)expression.Arguments[1]).Operand;
            var e = expr.Body as MemberExpression;
            if (e != null) {
                string n = ExpressionParser<T>.GetColumnName(e.Member.Name);
                AddOrderBy(isAsc ? new ASC(n) : new DESC(n));
            }
        }

        private void AddOrderBy(ASC item) {
            if (Orderby == null) {
                Orderby = new OrderBy();
            }
            Orderby.OrderItems.Add(item);
        }

        public static void ProcessSelectColumns(Expression expr) {
            var ex = GetUnQuoteExpression(expr);
            if (ex.Body.NodeType != ExpressionType.New) return;

            var exNew = (NewExpression)ex.Body;
            var type = exNew.Constructor.DeclaringType;
            var handler = DynamicLinqObjectHandler.Factory.GetInstance(type);
            if (handler.IsJarNotInitialized) {
                var list = new List<string>(exNew.Arguments.Count);
                foreach (MemberExpression member in exNew.Arguments) {
                    list.Add(member.Member.Name);
                }
                handler.Init(list);
            }
        }

        private static LambdaExpression GetUnQuoteExpression(Expression expr) {
            if (expr.NodeType == ExpressionType.Quote) {
                return GetUnQuoteExpression(((UnaryExpression)expr).Operand);
            }
            return (LambdaExpression)expr;
        }
    }
}