using System;
using Lephone.Data.Builder.Clause;

namespace Lephone.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class JoinOnAttribute : Attribute
    {
        public int Index;
        public JoinClause Joinner;

        public JoinOnAttribute(int index, Type modelType1, string key1, Type modelType2, string key2)
            : this(index, modelType1, key1, modelType2, key2, CompareOpration.Equal, JoinMode.Inner)
        {
        }

        public JoinOnAttribute(int index, Type modelType1, string key1, Type modelType2, string key2, CompareOpration comp)
            : this(index, modelType1, key1, modelType2, key2, comp, JoinMode.Inner)
        {
        }

        public JoinOnAttribute(int index, Type modelType1, string key1, Type modelType2, string key2, JoinMode mode)
            : this(index, modelType1, key1, modelType2, key2, CompareOpration.Equal, mode)
        {
        }

        public JoinOnAttribute(int index, Type modelType1, string key1, Type modelType2, string key2, CompareOpration comp, JoinMode mode)
        {
            this.Index = index;
            Joinner = new JoinClause(modelType1, key1, modelType2, key2, comp, mode);
        }

        public JoinOnAttribute(int index, string table1, string key1, string table2, string key2, CompareOpration comp, JoinMode mode)
        {
            this.Index = index;
            Joinner = new JoinClause(table1, key1, table2, key2, comp, mode);
        }
    }
}
