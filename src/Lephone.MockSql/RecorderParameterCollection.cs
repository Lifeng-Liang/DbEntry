using System;
using System.Collections;
using System.Data.Common;
using System.Text;

namespace Lephone.MockSql
{
    public class RecorderParameterCollection : DbParameterCollection
    {
        private readonly ArrayList Parameters = new ArrayList();

        public override int Add(object value)
        {
            return Parameters.Add(value);
        }

        public override void AddRange(Array values)
        {
            Parameters.AddRange(values);
        }

        public override void Clear()
        {
            Parameters.Clear();
        }

        public override bool Contains(string value)
        {
            return Parameters.Contains(value);
        }

        public override bool Contains(object value)
        {
            return Parameters.Contains(value);
        }

        public override void CopyTo(Array array, int index)
        {
            Parameters.CopyTo(array, index);
        }

        public override int Count
        {
            get { return Parameters.Count; }
        }

        public override IEnumerator GetEnumerator()
        {
            return Parameters.GetEnumerator();
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            throw RecorderFactory.NotImplemented;
        }

        protected override DbParameter GetParameter(int index)
        {
            return (DbParameter)Parameters[index];
        }

        public override int IndexOf(string parameterName)
        {
            return Parameters.IndexOf(parameterName);
        }

        public override int IndexOf(object value)
        {
            return Parameters.IndexOf(value);
        }

        public override void Insert(int index, object value)
        {
            Parameters.Insert(index, value);
        }

        public override bool IsFixedSize
        {
            get { return Parameters.IsFixedSize; }
        }

        public override bool IsReadOnly
        {
            get { return Parameters.IsReadOnly; }
        }

        public override bool IsSynchronized
        {
            get { return Parameters.IsSynchronized; }
        }

        public override void Remove(object value)
        {
            Parameters.Remove(value);
        }

        public override void RemoveAt(string parameterName)
        {
            throw RecorderFactory.NotImplemented;
        }

        public override void RemoveAt(int index)
        {
            Parameters.RemoveAt(index);
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            throw RecorderFactory.NotImplemented;
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            Parameters[index] = value;
        }

        public override object SyncRoot
        {
            get { return Parameters.SyncRoot; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(RecorderParameter p in this)
            {
                sb.Append(p.ToString()).Append(",");
            }
            if (sb.Length > 0)
            {
                sb.Length--;
            }
            return sb.ToString();
        }
    }
}
