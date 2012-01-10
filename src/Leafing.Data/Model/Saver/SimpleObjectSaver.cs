using Leafing.Data.Definition;
using Leafing.Data.Model.Composer;
using Leafing.Data.Model.Handler;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Model.Saver
{
    class SimpleObjectSaver
    {
        protected readonly ObjectInfo Info;
        protected readonly QueryComposer Composer;
        protected readonly DataProvider Provider;
        protected readonly IDbObjectHandler Handler;

        public SimpleObjectSaver(ObjectInfo info, QueryComposer composer, DataProvider provider, IDbObjectHandler handler)
        {
            this.Info = info;
            this.Composer = composer;
            this.Provider = provider;
            this.Handler = handler;
        }

        public virtual void Save(IDbObject obj)
        {
            throw new DataException("To call this function, the table must have one primary key.");
        }

        public virtual object Insert(IDbObject obj)
        {
            var sb = Composer.GetInsertStatementBuilder(obj);
            var sql = sb.ToSqlStatement(Provider.Dialect, null);
            Provider.ExecuteNonQuery(sql);
            return null;
        }

        public virtual void Update(IDbObject obj)
        {
            var iwc = ModelContext.GetKeyWhereClause(obj);
            SqlStatement updateStatement = Composer.GetUpdateStatement(obj, iwc);
            if (Provider.ExecuteNonQuery(updateStatement) == 0)
            {
                throw new DataException("Record doesn't exist OR LockVersion doesn't match!");
            }
            Composer.ProcessAfterSave(obj);
        }
    }
}
