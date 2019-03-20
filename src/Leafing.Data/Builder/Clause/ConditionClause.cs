using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Leafing.Data.Dialect;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Builder.Clause {
    [Serializable]
    public abstract class ConditionClause : Condition {
        private readonly string _condition;
        private readonly ArrayList _list = new ArrayList();

        protected ConditionClause(string condition) {
            this._condition = condition;
        }

        protected ConditionClause(string condition, params Condition[] ics) : this(condition) {
            foreach (Condition ic in ics) {
                if (ic != null) {
                    Add(ic);
                }
            }
        }

        public override bool SubClauseNotEmpty {
            get {
                foreach (Condition ic in _list) {
                    if (ic.SubClauseNotEmpty) {
                        return true;
                    }
                }
                return false;
            }
        }

        public void Add(Condition ic) {
            _list.Add(ic);
        }

        public Condition this[int index] {
            get { return (Condition)_list[index]; }
            set { _list[index] = value; }
        }

        public override string ToSqlText(DataParameterCollection dpc, DbDialect dd, List<string> queryRequiredFields) {
            var sb = new StringBuilder();
            foreach (Condition ic in _list) {
                if (ic.SubClauseNotEmpty) {
                    sb.Append("(");
                    sb.Append(ic.ToSqlText(dpc, dd, queryRequiredFields));
                    sb.Append(") ");
                    sb.Append(_condition);
                    sb.Append(" ");
                }
            }
            string s = sb.ToString();
            return (s.Length > 5) ? s.Substring(0, s.Length - _condition.Length - 2) : "";
        }
    }
}
