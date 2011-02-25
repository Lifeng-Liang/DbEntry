using Lephone.Data.Definition;
using Lephone.Data.Model.Composer;
using Lephone.Data.Model.Handler;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Model.Deleter
{
    class SimpleDeleter
    {
        protected readonly ObjectInfo Info;
        protected readonly QueryComposer Composer;
        protected readonly DataProvider Provider;
        protected readonly IDbObjectHandler Handler;

        public SimpleDeleter(ObjectInfo info, QueryComposer composer, DataProvider provider, IDbObjectHandler handler)
        {
            this.Info = info;
            this.Composer = composer;
            this.Provider = provider;
            this.Handler = handler;
        }

        public virtual int Delete(IDbObject obj)
        {
            var sql = Composer.GetDeleteStatement(obj);
            return Provider.ExecuteNonQuery(sql);
        }
    }
}
