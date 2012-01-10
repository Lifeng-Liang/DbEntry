using System;
using System.Data;

namespace Leafing.Data.Model.Creator
{
    class GroupByModelCreator : PureObjectModelCreator
    {
        public GroupByModelCreator(Type dbObjectType, bool useIndex, bool noLazy) 
            : base(dbObjectType, useIndex, noLazy)
        {
        }

        protected override void InitObject(object obj, IDataReader dr)
        {
            ((IGroupByObject)obj).Init(dr);
        }
    }
}
