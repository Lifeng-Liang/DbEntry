using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Lephone.Data.SqlEntry.Dynamic
{
    public class DynamicTable : List<DynamicTable.DynamicRow>
    {
        public class DynamicRow : DynamicObject
        {
            private readonly DynamicTable _owner;
            private readonly List<object> _jar;

            internal DynamicRow(DynamicTable table)
            {
                _owner = table;
                _jar = new List<object>();
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                int i = _owner.GetIndex(binder.Name);
                if (i >= 0)
                {
                    result = _jar[i];
                    return true;
                }
                throw new MissingMemberException("Member " + binder.Name + " not found!");
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                SetMember(binder.Name, value);
                return true;
            }

            internal void SetMember(string name, object value)
            {
                int i = _owner.GetIndex(name);
                if (i >= 0)
                {
                    _jar.Insert(i, value == DBNull.Value ? null : value);
                }
            }

            internal void AppendMember(object value)
            {
                _jar.Add(value);
            }

            internal void AppendMemberRange(object[] values)
            {
                _jar.AddRange(values);
            }
        }

        private readonly Dictionary<string, int> _jar = new Dictionary<string, int>();
        internal bool NeedInit = true;

        internal DynamicTable()
        {
        }

        public int GetIndex(string key)
        {
            if (_jar.ContainsKey(key))
            {
                return _jar[key];
            }
            return -1;
        }

        public void AddKey(string key, int index)
        {
            _jar.Add(key, index);
        }

        public DynamicRow NewRow()
        {
            var row = new DynamicRow(this);
            Add(row);
            return row;
        }
    }
}

