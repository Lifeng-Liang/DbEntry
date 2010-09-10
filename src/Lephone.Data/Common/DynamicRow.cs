using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Lephone.Data.Common
{
    public class DynamicRow : DynamicObject
    {
        private readonly Dictionary<string, object> _jar = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_jar.ContainsKey(binder.Name))
            {
                result = _jar[binder.Name];
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
            _jar[name] = value == DBNull.Value ? null : value;
        }
    }
}
