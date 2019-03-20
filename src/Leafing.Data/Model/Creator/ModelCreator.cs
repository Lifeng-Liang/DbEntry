using System;
using System.Data;
using Leafing.Data.Definition;

namespace Leafing.Data.Model.Creator {
    abstract class ModelCreator {
        public static ModelCreator GetCreator(Type dbObjectType, bool useIndex, bool noLazy) {
            if (dbObjectType.Name.StartsWith("<")) {
                return new AnonymousModelCreator(dbObjectType, useIndex, noLazy);
            }
            if (dbObjectType.Name.StartsWith("GroupByObject`1")) {
                return new GroupByModelCreator(dbObjectType, useIndex, noLazy);
            }
            if (dbObjectType.IsSubclassOf(typeof(DbObjectSmartUpdate))) {
                return new StandardModelCreator(dbObjectType, useIndex, noLazy);
            }
            return new PureObjectModelCreator(dbObjectType, useIndex, noLazy);
        }

        protected Type DbObjectType;
        protected bool UseIndex;
        protected bool NoLazy;

        protected ModelCreator(Type dbObjectType, bool useIndex, bool noLazy) {
            this.DbObjectType = dbObjectType;
            this.UseIndex = useIndex;
            this.NoLazy = noLazy;
        }

        public abstract object CreateObject(IDataReader dr);
    }
}
