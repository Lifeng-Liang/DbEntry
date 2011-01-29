using Lephone.Data.Builder;
using Lephone.Data.Definition;

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
            ret += ProcessHasAndBelongsToMany();
            return ret;
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
