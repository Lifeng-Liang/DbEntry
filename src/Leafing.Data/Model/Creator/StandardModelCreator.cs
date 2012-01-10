using System;
using System.Data;
using Leafing.Data.Definition;

namespace Leafing.Data.Model.Creator
{
    class StandardModelCreator : PureObjectModelCreator
    {
        public StandardModelCreator(Type dbObjectType, bool useIndex, bool noLazy) 
            : base(dbObjectType, useIndex, noLazy)
        {
        }

        protected override void InitObject(object obj, IDataReader dr)
        {
            var sudi = (DbObjectSmartUpdate)obj;
            sudi.m_InternalInit = true;
            LoadValues(obj, Ctx, dr, UseIndex, NoLazy);
            sudi.m_InternalInit = false;
        }
    }
}
