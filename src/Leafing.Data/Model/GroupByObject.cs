using System;
using System.Data;
using Leafing.Data.Definition;

namespace Leafing.Data.Model {
    internal interface IGroupByObject {
        void Init(IDataReader dr);
    }

    public class GroupByObject<T> : IGroupByObject, IDbObject {
        // set it to key to make it looks like the first column
        [DbKey(IsDbGenerate = false)]
        public T Column;

        public long Count;

        public GroupByObject() { }

        public GroupByObject(T column, long count) {
            Column = column;
            Count = count;
        }

        void IGroupByObject.Init(IDataReader dr) {
            this.Column = (T)dr[0];
            this.Count = (long)Convert.ChangeType(dr[1], typeof(long));
        }
    }
}