using Leafing.Data.Builder.Clause;
using Leafing.Data.SqlEntry;
using Leafing.Data.Builder;

namespace Leafing.Data.Model.Composer
{
    internal class OptimisticLockingQueryComposer : QueryComposer
    {
        public OptimisticLockingQueryComposer(ModelContext ctx)
            : base(ctx)
        {
        }

        public override SqlStatement GetUpdateStatement(object obj, Condition iwc)
        {
            var sb = new UpdateStatementBuilder(Context.Info.From);
            Context.Handler.SetValuesForUpdate(sb, obj);
            var lv = (int)Context.Info.LockVersion.GetValue(obj);
            sb.Where.Conditions = iwc && (CK.K[Context.Info.LockVersion.Name] == lv);
            bool find = false;
            foreach (var kv in sb.Values)
            {
                if (kv.Key == Context.Info.LockVersion.Name)
                {
                    kv.Value = lv + 1;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                sb.Values.Add(new KeyOpValue(Context.Info.LockVersion.Name, lv + 1, KvOpertation.None));
            }
            return sb.ToSqlStatement(Context);
        }

        public override void ProcessAfterSave(object obj)
        {
            var lv = (int)Context.Info.LockVersion.GetValue(obj);
            lv++;
            Context.Info.LockVersion.SetValue(obj, lv);
        }
    }
}
