using System;
using System.Data;
using Lephone.Data.Model.Handler;

namespace Lephone.Data.Model.Creator
{
    class AnonymousModelCreator : ModelCreator
    {
        private readonly DynamicLinqObjectHandler _handler;

        public AnonymousModelCreator(Type dbObjectType, bool useIndex, bool noLazy) 
            : base(dbObjectType, useIndex, noLazy)
        {
            _handler = DynamicLinqObjectHandler.Factory.GetInstance(DbObjectType);
        }

        public override object CreateObject(IDataReader dr)
        {
            return _handler.CreateObject(dr, UseIndex);
        }
    }
}
