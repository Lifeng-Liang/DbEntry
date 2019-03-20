using System.Collections.Generic;
using System.Data;
using Leafing.Data.Builder;
using Leafing.Data.Definition;

namespace Leafing.Data.Model.Handler {
    public interface IDbObjectHandler {
        void CtorInit(DbObjectSmartUpdate o);
        object CreateInstance();
        void LoadSimpleValues(object o, bool useIndex, IDataReader dr);
        void LoadRelationValues(object o, bool useIndex, bool noLazy, IDataReader dr);
        Dictionary<string, object> GetKeyValues(object o);
        object GetKeyValue(object o);
        void SetKeyValue(object obj, object key);
        void SetValuesForSelect(ISqlKeys isv, bool noLazy);
        void SetValuesForInsert(ISqlValues isv, object obj);
        void SetValuesForUpdate(ISqlValues isv, object obj);
    }
}