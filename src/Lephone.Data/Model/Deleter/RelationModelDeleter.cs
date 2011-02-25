using Lephone.Data.Builder;
using Lephone.Data.Definition;
using Lephone.Data.Model.Composer;
using Lephone.Data.Model.Handler;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Model.Deleter
{
    class RelationModelDeleter : KeyModelDeleter
    {
        public RelationModelDeleter(ObjectInfo info, QueryComposer composer, DataProvider provider, IDbObjectHandler handler)
            : base(info, composer, provider, handler)
        {
        }

        public override int Delete(IDbObject obj)
        {
            int ret = 0;
            DbEntry.UsingTransaction(delegate
            {
                ret += ProcessRelation(obj);
                ret += base.Delete(obj);
            });
            return ret;
        }

        private int ProcessRelation(IDbObject obj)
        {
            int ret = 0;
            ret += ProcessHasManyAndHasOne(obj);
            ret += ProcessHasAndBelongsToMany(obj);
            return ret;
        }

        private int ProcessHasManyAndHasOne(IDbObject obj)
        {
            var result = 0;
            foreach (var member in Info.RelationMembers)
            {
                if (member.Is.HasMany || member.Is.HasOne)
                {
                    var t = member.MemberInfo.MemberType.GetGenericArguments()[0];
                    var ctx0 = ModelContext.GetInstance(t);
                    var sb = new UpdateStatementBuilder(ctx0.Info.From.MainTableName);
                    var key = ctx0.Info.GetBelongsTo(Info.HandleType).Name;
                    sb.Values.Add(new KeyValue(key, null));
                    sb.Where.Conditions = CK.K[key] == Handler.GetKeyValue(obj);
                    var sql = sb.ToSqlStatement(ctx0);
                    result += ctx0.Provider.ExecuteNonQuery(sql);
                }
            }
            return result;
        }

        private int ProcessHasAndBelongsToMany(IDbObject obj)
        {
            int ret = 0;
            foreach (CrossTable mt in Info.CrossTables.Values)
            {
                var sb = new DeleteStatementBuilder(mt.Name);
                sb.Where.Conditions = CK.K[mt[0].Column] == Handler.GetKeyValue(obj);
                var sql = sb.ToSqlStatement(Provider.Dialect, Info.AllowSqlLog);
                ret += Provider.ExecuteNonQuery(sql);
            }
            return ret;
        }
    }
}
