using System;
using System.Linq.Expressions;
using Leafing.Data.Definition;
using Leafing.Data.Model.Linq;

namespace Leafing.Data {
    public class ConditionBuilder<T> where T : IDbObject, new() {
        private Condition _condition;
        private int _count;

        public ConditionBuilder() {
        }

        public ConditionBuilder(Expression<Func<T, bool>> expr) {
            _condition = ExpressionParser<T>.Parse(expr);
            _count++;
        }

        public ConditionBuilder<T> And(Expression<Func<T, bool>> expr) {
            _condition &= ExpressionParser<T>.Parse(expr);
            _count++;
            return this;
        }

        public ConditionBuilder<T> And(bool check, Expression<Func<T, bool>> expr) {
            if (check) {
                _condition &= ExpressionParser<T>.Parse(expr);
                _count++;
            }
            return this;
        }

        public ConditionBuilder<T> Or(Expression<Func<T, bool>> expr) {
            _condition |= ExpressionParser<T>.Parse(expr);
            _count++;
            return this;
        }

        public ConditionBuilder<T> Or(bool check, Expression<Func<T, bool>> expr) {
            if (check) {
                _condition |= ExpressionParser<T>.Parse(expr);
                _count++;
            }
            return this;
        }

        public static ConditionBuilder<T> operator &(ConditionBuilder<T> builder, Expression<Func<T, bool>> expr) {
            builder._condition &= ExpressionParser<T>.Parse(expr);
            builder._count++;
            return builder;
        }

        public static ConditionBuilder<T> operator |(ConditionBuilder<T> builder, Expression<Func<T, bool>> expr) {
            builder._condition |= ExpressionParser<T>.Parse(expr);
            builder._count++;
            return builder;
        }

        public int Count {
            get { return _count; }
        }

        public Condition ToCondition() {
            return _condition;
        }
    }
}