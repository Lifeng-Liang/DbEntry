using System;
using System.Collections;
using System.Data.Common;
using System.Text;

namespace Leafing.MockSql {
    public class RecorderParameterCollection : DbParameterCollection {
        private readonly ArrayList _parameters = new ArrayList();

        public override int Add(object value) {
            return _parameters.Add(value);
        }

        public override void AddRange(Array values) {
            _parameters.AddRange(values);
        }

        public override void Clear() {
            _parameters.Clear();
        }

        public override bool Contains(string value) {
            return _parameters.Contains(value);
        }

        public override bool Contains(object value) {
            return _parameters.Contains(value);
        }

        public override void CopyTo(Array array, int index) {
            _parameters.CopyTo(array, index);
        }

        public override int Count {
            get { return _parameters.Count; }
        }

        public override IEnumerator GetEnumerator() {
            return _parameters.GetEnumerator();
        }

        protected override DbParameter GetParameter(string parameterName) {
            throw RecorderFactory.NotImplemented;
        }

        protected override DbParameter GetParameter(int index) {
            return (DbParameter)_parameters[index];
        }

        public override int IndexOf(string parameterName) {
            return _parameters.IndexOf(parameterName);
        }

        public override int IndexOf(object value) {
            return _parameters.IndexOf(value);
        }

        public override void Insert(int index, object value) {
            _parameters.Insert(index, value);
        }

        public override bool IsFixedSize {
            get { return _parameters.IsFixedSize; }
        }

        public override bool IsReadOnly {
            get { return _parameters.IsReadOnly; }
        }

        public override bool IsSynchronized {
            get { return _parameters.IsSynchronized; }
        }

        public override void Remove(object value) {
            _parameters.Remove(value);
        }

        public override void RemoveAt(string parameterName) {
            throw RecorderFactory.NotImplemented;
        }

        public override void RemoveAt(int index) {
            _parameters.RemoveAt(index);
        }

        protected override void SetParameter(string parameterName, DbParameter value) {
            throw RecorderFactory.NotImplemented;
        }

        protected override void SetParameter(int index, DbParameter value) {
            _parameters[index] = value;
        }

        public override object SyncRoot {
            get { return _parameters.SyncRoot; }
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (RecorderParameter p in this) {
                sb.Append(p.ToString()).Append(",");
            }
            if (sb.Length > 0) {
                sb.Length--;
            }
            return sb.ToString();
        }
    }
}