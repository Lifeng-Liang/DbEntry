using Lephone.Data.Definition;
using Lephone.Data.Model.Composer;
using Lephone.Data.Model.Handler;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Model.Saver
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
            o.m_InitUpdateColumns();
            return key;
        }

        public override void Update(IDbObject obj)
        {
            var o = (DbObjectSmartUpdate)obj;
            if (o.m_UpdateColumns != null)
            {
                if (o.m_UpdateColumns.Count > 0)
                {
                    o.RaiseUpdating();
                    base.Update(o);
                }
            }
            o.m_InitUpdateColumns();
        }
    }
}
