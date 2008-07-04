using System;
using Lephone.Data.Builder;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder.Clause;

namespace Lephone.Data
{
    [Serializable]
	public abstract class WhereCondition : IClause
	{
        public static readonly WhereCondition EmptyCondition = new Common.EmptyCondition();
        public static readonly ConstCondition TrueCondition = new ConstCondition("(1=1)");
        public static readonly ConstCondition FalseCondition = new ConstCondition("(1<>1)");

        public abstract bool SubClauseNotEmpty { get; }

        protected WhereCondition() { }

        public static bool operator true(WhereCondition kv)
        {
            return false;
        }

        public static bool operator false(WhereCondition kv)
        {
            return false;
        }

        public static WhereCondition operator &(WhereCondition Condition1, WhereCondition Condition2)
		{
            return GetConditionClause(Condition1, Condition2, new AndClause(Condition1, Condition2));
		}

        public WhereCondition And(WhereCondition Condition2)
        {
            return GetConditionClause(this, Condition2, new AndClause(this, Condition2));
        }

        public static WhereCondition operator |(WhereCondition Condition1, WhereCondition Condition2)
		{
            return GetConditionClause(Condition1, Condition2, new OrClause(Condition1, Condition2));
		}

        public WhereCondition Or(WhereCondition Condition2)
        {
            return GetConditionClause(this, Condition2, new OrClause(this, Condition2));
        }

        private static WhereCondition GetConditionClause(WhereCondition Condition1, WhereCondition Condition2, WhereCondition NotNullCondition)
        {
            if (IsNullOrEmpty(Condition1) && IsNullOrEmpty(Condition2))
            {
                return EmptyCondition;
            }
            if (!IsNullOrEmpty(Condition1) && !IsNullOrEmpty(Condition2))
            {
                return NotNullCondition;
            }
            if (!IsNullOrEmpty(Condition1))
            {
                return Condition1;
            }
            return Condition2;
        }

        public static WhereCondition operator !(WhereCondition Condition)
		{
            if (IsNullOrEmpty(Condition))
            {
                return EmptyCondition;
            }
			return new NotClause(Condition);
		}

        public WhereCondition Not()
        {
            if (IsNullOrEmpty(this))
            {
                return this;
            }
            return new NotClause(this);
        }

        private static bool IsNullOrEmpty(WhereCondition Condition)
        {
            return (Condition == null || (Condition is Common.EmptyCondition));
        }

        public abstract string ToSqlText(DataParamterCollection dpc, Dialect.DbDialect dd);
	}
}
