using Lephone.Data.Dialect;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder;

namespace Lephone.Data.Common
{
    internal class OptimisticLockingQueryComposer : QueryComposer
    {
        public OptimisticLockingQueryComposer(ObjectInfo oi) : base(oi) { }

        //public override SqlStatement GetDeleteStatement(DbDialect Dialect, object obj)
        //{
        //    DeleteStatementBuilder sb = new DeleteStatementBuilder(oi.From.GetMainTableName());
        //    sb.Where.Conditions = DbObjectHelper.GetKeyWhereClause(obj)
        //        && (CK.K[oi.LockVersion.Name] == oi.LockVersion.GetValue(obj));
        //    return sb.ToSqlStatement(Dialect);
        //}

        public override SqlStatement GetUpdateStatement(DbDialect Dialect, object obj, WhereCondition iwc)
        {
            UpdateStatementBuilder sb = new UpdateStatementBuilder(oi.From.GetMainTableName());
            oi.Handler.SetValuesForUpdate(sb, obj);
            int lv = (int)oi.LockVersion.GetValue(obj);
            sb.Where.Conditions = iwc && (CK.K[oi.LockVersion.Name] == lv);
            bool find = false;
            foreach (KeyValue kv in sb.Values)
            {
                if (kv.Key == oi.LockVersion.Name)
                {
                    kv.Value = lv + 1;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                sb.Values.Add(new KeyValue(oi.LockVersion.Name, lv + 1));
            }
            return sb.ToSqlStatement(Dialect);
        }
    }
}
