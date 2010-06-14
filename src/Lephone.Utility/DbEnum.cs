using System;
using System.Collections;
using System.Collections.Specialized;
using Lephone.Core.Text;
using Lephone.Data;

namespace Lephone.Utility
{
    // order by [Id]
    public class DbEnum
    {
        private readonly HybridDictionary _dict;
        private readonly HybridDictionary _vdic;
        private readonly string[] _list;

        public DbEnum(int type)
        {
            IList dic = DbEntry
                .From<LephoneEnum>()
                .Where(CK.K["Type"] == type)
                .OrderBy("Id")
                .Select();
            _dict = new HybridDictionary();
            _vdic = new HybridDictionary();
            _list = new string[dic.Count];
            int n = 0;
            int m = 0;
            foreach (LephoneEnum e in dic)
            {
                _list[n] = e.Name;
                if (e.Value != null)
                {
                    m = (int)e.Value;
                }
                _dict[e.Name.ToLower()] = m;
                _vdic[m] = e.Name;
                n++;
                m++;
            }
        }

        public DbEnum(Type enumType)
        {
            _dict = new HybridDictionary();
            _vdic = new HybridDictionary();
            _list = Enum.GetNames(enumType);
            for (int i = 0; i < _list.Length; i++)
            {
                var o = Enum.Parse(enumType, _list[i]);
                var n = (int)o;
                var ns = StringHelper.EnumToString(o);
                _dict[_list[i].ToLower()] = n;
                if (_list[i] != ns)
                {
                    _list[i] = ns;
                    _dict[ns.ToLower()] = n;
                }
            }
            foreach (string s in _list)
            {
                _vdic[this[s]] = s;
            }
        }

        public string[] GetNames()
        {
            return _list;
        }

        public string this[int value]
        {
            get
            {
                return (string)_vdic[value];
            }
        }

        public int this[string key]
        {
            get
            {
                return ((int)_dict[key.ToLower()]);
            }
        }
    }
}
