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
        //    DeleteStatementBuilder sb = new DeleteStatementBuilder(Info.From.MainTableName);
        //    sb.Where.Conditions = DbObjectHelper.GetKeyWhereClause(obj)
        //        && (CK.K[Info.LockVersion.Name] == Info.LockVersion.GetValue(obj));
        //    return sb.ToSqlStatement(Dialect);
        //}

        public override SqlStatement GetUpdateStatement(DbDialect dialect, object obj, Condition iwc)
        {
            var sb = new UpdateStatementBuilder(Info.From.MainTableName);
            Info.Handler.SetValuesForUpdate(sb, obj);
            var lv = (int)Info.LockVersion.GetValue(obj);
            sb.Where.Conditions = iwc && (CK.K[Info.LockVersion.Name] == lv);
            bool find = false;
            foreach (KeyValue kv in sb.Values)
            {
                if (kv.Key == Info.LockVersion.Name)
                {
                    kv.Value = lv + 1;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                sb.Values.Add(new KeyValue(Info.LockVersion.Name, lv + 1));
            }
            return sb.ToSqlStatement(dialect);
        }

        public override void ProcessAfterSave(object obj)
        {
            var lv = (int)Info.LockVersion.GetValue(obj);
            lv++;
            Info.LockVersion.SetValue(obj, lv);
        }
    }
}
