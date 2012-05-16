using System;
using System.Linq.Expressions;
using Leafing.Data.Definition;
using Leafing.Data.Model.Linq;

namespace Leafing.Data
{
    public class ConditionBuilder<T> where T : IDbObject, new()
    {
        private Condition _condition;

        public ConditionBuilder()
        {
        }

        public ConditionBuilder(Expression<Func<T, bool>> expr)
        {
            _condition = ExpressionParser<T>.Parse(expr);
        }

        public ConditionBuilder<T> And(Expression<Func<T, bool>> expr)
        {
            _condition &= ExpressionParser<T>.Parse(expr);
            return this;
        }

        public ConditionBuilder<T> And(bool check, Expression<Func<T, bool>> expr)
        {
            if (check)
            {
                _condition &= ExpressionParser<T>.Parse(expr);
            }
            return this;
        }

        public ConditionBuilder<T> Or(Expression<Func<T, bool>> expr)
        {
            _condition |= ExpressionParser<T>.Parse(expr);
            return this;
        }

        public ConditionBuilder<T> Or(bool check, Expression<Func<T, bool>> expr)
        {
            if(check)
            {
                _condition |= ExpressionParser<T>.Parse(expr);
            }
            return this;
        }

        public static ConditionBuilder<T> operator &(ConditionBuilder<T> builder, Expression<Func<T, bool>> expr)
        {
            builder._condition &= ExpressionParser<T>.Parse(expr);
            return builder;
        }

        public static ConditionBuilder<T> operator |(ConditionBuilder<T> builder, Expression<Func<T, bool>> expr)
        {
            builder._condition |= ExpressionParser<T>.Parse(expr);
            return builder;
        }

        public Condition ToCondition()
        {
            return _condition;
        }
    }
}
