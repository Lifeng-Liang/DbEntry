using System.Linq.Expressions;

namespace Lephone.Data.Linq
{
    static class LinqHelper
    {
        internal static MemberExpression GetMemberExpression(this LambdaExpression expr)
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
    }
}
