using System;
using System.Collections.Generic;
using System.Data;
using Lephone.Data.SqlEntry;

namespace Lephone.Data.Common
{
    public class GroupbyObjectHandler<T> : EmitObjectHandlerBase
    {
        public override object CreateInstance()
        {
            return new GroupByObject<T>();
        }

        protected override void LoadSimpleValuesByIndex(object o, IDataReader dr)
        {
            var obj = (GroupByObject<T>)o;
            obj.Column = (T)Convert.ChangeType(dr.GetValue(0), typeof(T));
            obj.Count = dr.GetInt64(1);
        }

        protected override void LoadSimpleValuesByName(object o, IDataReader dr)
        {
        }

        protected override void LoadRelationValuesByIndex(object o, IDataReader dr)
        {
        }

        protected override void LoadRelationValuesByName(object o, IDataReader dr)
        {
        }

        protected override void GetKeyValuesDirect(Dictionary<string, object> dic, object o)
        {
        }

        protected override void SetValuesForSelectDirect(List<KeyValuePair<string, string>> keys)
        {
        }

        protected override void SetValuesForInsertDirect(KeyValueCollection values, object o)
        {
        }

        protected override void SetValuesForUpdateDirect(KeyValueCollection values, object o)
        {
        }
    }
}
