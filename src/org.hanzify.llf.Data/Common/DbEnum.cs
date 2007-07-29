
#region usings

using System;
using System.Collections;
using System.Collections.Specialized;

using org.hanzify.llf.util.Text;

#endregion

namespace org.hanzify.llf.Data.Common
{
    // order by [Id]
    public class DbEnum
    {
        private HybridDictionary dict;
        private HybridDictionary vdic;
        private string[] list;

        public DbEnum(int Type)
        {
            IList dic = DbEntry
                .From<EnumTable>()
                .Where(CK.K["Type"] == Type)
                .OrderBy("Id")
                .Select();
            dict = new HybridDictionary();
            vdic = new HybridDictionary();
            list = new string[dic.Count];
            int n = 0;
            int m = 0;
            foreach (EnumTable e in dic)
            {
                list[n] = e.Name;
                if (e.Value != null)
                {
                    m = (int)e.Value;
                }
                dict[e.Name.ToLower()] = m;
                vdic[m] = e.Name;
                n++;
                m++;
            }
        }

        public DbEnum(Type EnumType)
        {
            dict = new HybridDictionary();
            vdic = new HybridDictionary();
            list = Enum.GetNames(EnumType);
            for (int i = 0; i < list.Length; i++)
            {
                object o = Enum.Parse(EnumType, list[i]);
                int n = (int)o;
                string ns = StringHelper.EnumToString(o);
                dict[list[i].ToLower()] = n;
                if (list[i] != ns)
                {
                    list[i] = ns;
                    dict[ns.ToLower()] = n;
                }
            }
            foreach (string s in list)
            {
                vdic[this[s]] = s;
            }
        }

        public string[] GetNames()
        {
            return list;
        }

        public string this[int Value]
        {
            get
            {
                return (string)vdic[Value];
            }
        }

        public int this[string Key]
        {
            get
            {
                return ((int)dict[Key.ToLower()]);
            }
        }
    }
}
