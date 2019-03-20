using System;
using System.Collections.Generic;
using System.Data;
using Leafing.Data.Builder.Clause;

namespace Leafing.Data.Model.Handler {
    public class GroupbyObjectHandler<T> : EmitObjectHandlerBase {
        public override object CreateInstance() {
            return new GroupByObject<T>();
        }

        protected override void LoadSimpleValuesByIndex(object o, IDataReader dr) {
            var obj = (GroupByObject<T>)o;
            obj.Column = (T)Convert.ChangeType(dr.GetValue(0), typeof(T));
            obj.Count = dr.GetInt64(1);
        }

        protected override void GetKeyValuesDirect(Dictionary<string, object> dic, object o) {
        }

        protected override void SetKeyValueDirect(object obj, object key) {
        }

        protected override void LoadRelationValuesByIndex(object o, IDataReader dr) {
        }

        protected override void LoadRelationValuesByIndexNoLazy(object o, IDataReader dr) {
        }

        protected override void LoadRelationValuesByName(object o, IDataReader dr) {
        }

        protected override void LoadRelationValuesByNameNoLazy(object o, IDataReader dr) {
        }

        protected override void LoadSimpleValuesByName(object o, IDataReader dr) {
        }

        protected override void SetValuesForInsertDirect(List<KeyOpValue> values, object o) {
        }

        protected override void SetValuesForSelectDirect(List<KeyValuePair<string, string>> keys) {
        }

        protected override void SetValuesForSelectDirectNoLazy(List<KeyValuePair<string, string>> keys) {
        }

        protected override void SetValuesForUpdateDirect(List<KeyOpValue> values, object o) {
        }
    }
}