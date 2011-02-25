using Lephone.Data.Model.Composer;
using Lephone.Data.Model.Handler;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Model.Deleter
{
    class KeyModelDeleter : SimpleDeleter
    {
        public KeyModelDeleter(ObjectInfo info, QueryComposer composer, DataProvider provider, IDbObjectHandler handler)
            : base(info, composer, provider, handler)
        {
        }

        public override int Delete(Definition.IDbObject obj)
        {
            var n = base.Delete(obj);
            //TODO: use emit instead of reflection
            Info.KeyMembers[0].SetValue(obj, Info.KeyMembers[0].UnsavedValue);
            return n;
        }
    }
}
