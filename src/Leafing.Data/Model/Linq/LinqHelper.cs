using System;
using System.Linq.Expressions;
using Leafing.Data.Common;
using Leafing.Data.Definition;

namespace Leafing.Data.Model.Linq
{
    public static class LinqHelper
    {
        public static MemberExpression GetMemberExpression(this LambdaExpression expr)
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

        public static string GetColumnName<T>(this Expression<Func<T, object>> expr) where T : class, IDbObject
        {
            ColumnFunction function;
            MemberExpression obj;
            var key = ExpressionParser<T>.GetMemberName(expr.Body, out function, out obj);
            return key;
        }
    }
}
