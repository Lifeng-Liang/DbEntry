using Lephone.Data.Builder;
using Lephone.Data.Definition;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Model
{
    public class ModelDeleter
    {
        protected readonly object Obj;
        protected readonly ModelContext Ctx;

        public ModelDeleter(IDbObject obj)
        {
            this.Obj = obj;
            this.Ctx = ModelContext.GetInstance(obj.GetType());
        }

        public int Process()
        {
            Ctx.Operator.TryCreateTable();
            int ret = 0;

            if (Ctx.Info.HasAssociate)
            {
                DbEntry.UsingTransaction(delegate
                {
                    ret += ProcessRelation();
                    ret += InnerDelete();
                });
            }
            else
            {
                ret += InnerDelete();
            }

            if (Ctx.Info.KeyMembers[0].UnsavedValue != null)
            {
                Ctx.Info.KeyMembers[0].SetValue(Obj, Ctx.Info.KeyMembers[0].UnsavedValue);
            }
            return ret;
        }

        protected virtual int InnerDelete()
        {
            var sql = Ctx.Composer.GetDeleteStatement(Obj);
            return Ctx.Provider.ExecuteNonQuery(sql);
        }

        private int ProcessRelation()
        {
            int ret = 0;
            ret += ProcessHasManyAndHasOne();
            ret += ProcessHasAndBelongsToMany();
            return ret;
        }

        private int ProcessHasManyAndHasOne()
        {
            var result = 0;
            foreach(var member in Ctx.Info.RelationMembers)
            {
                if(member.Is.HasMany || member.Is.HasOne)
                {
                    var t = member.MemberInfo.MemberType.GetGenericArguments()[0];
                    var ctx0 = ModelContext.GetInstance(t);
                    var sb = new UpdateStatementBuilder(ctx0.Info.From.MainTableName);
                    var key = ctx0.Info.GetBelongsTo(Ctx.Info.HandleType).Name;
                    sb.Values.Add(new KeyValue(key, null));
                    sb.Where.Conditions = CK.K[key] == Ctx.Handler.GetKeyValue(Obj);
                    var sql = sb.ToSqlStatement(ctx0);
                    result += ctx0.Provider.ExecuteNonQuery(sql);
                }
            }
            return result;
        }

        private int ProcessHasAndBelongsToMany()
        {
            int ret = 0;
            foreach (CrossTable mt in Ctx.Info.CrossTables.Values)
            {
                var sb = new DeleteStatementBuilder(mt.Name);
                sb.Where.Conditions = CK.K[mt.ColumeName1] == Ctx.Handler.GetKeyValue(Obj);
                var sql = sb.ToSqlStatement(Ctx);
                ret += Ctx.Provider.ExecuteNonQuery(sql);
            }
            return ret;
        }
    }
}
