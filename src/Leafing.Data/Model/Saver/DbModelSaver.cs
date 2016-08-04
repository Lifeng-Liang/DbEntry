using Leafing.Data.Definition;
using Leafing.Data.Model.Composer;
using Leafing.Data.Model.Handler;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Model.Saver
{
    class DbModelSaver : DbObjectSaver
    {
        public DbModelSaver(ObjectInfo info, QueryComposer composer, DataProvider provider, IDbObjectHandler handler)
            : base(info, composer, provider, handler)
        {
        }

        public override object Insert(IDbObject obj)
        {
            var o = (DbObjectSmartUpdate)obj;
            o.RaiseInserting();
            var key = base.Insert(o);
			o.InitLoadedColumns ();
            return key;
        }

        public override void Update(IDbObject obj)
        {
            var o = (DbObjectSmartUpdate)obj;
            o.RaiseUpdating();
			base.InnerUpdate(o);
        }
    }
}
