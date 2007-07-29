
#region usings

using System;
using org.hanzify.llf.Data.Builder.Clause;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class JoinOnAttribute : Attribute
    {
        public int Index;
        public JoinClause joinner;

        public JoinOnAttribute(string Key1, string Key2)
            : this(Key1, Key2, CompareOpration.Equal, JoinMode.Inner)
        {
        }

        public JoinOnAttribute(string Key1, string Key2, CompareOpration comp)
            : this(Key1, Key2, comp, JoinMode.Inner)
        {
        }

        public JoinOnAttribute(string Key1, string Key2, JoinMode mode)
            : this(Key1, Key2, CompareOpration.Equal, mode)
        {
        }

        public JoinOnAttribute(string Key1, string Key2, CompareOpration comp, JoinMode mode)
        {
            Index = -1;
            joinner = new JoinClause(Key1, Key2, comp, mode);
        }

        public JoinOnAttribute(int Index, string Key1, string Key2)
            : this(Index, Key1, Key2, CompareOpration.Equal, JoinMode.Inner)
        {
        }

        public JoinOnAttribute(int Index, string Key1, string Key2, CompareOpration comp)
            : this(Index, Key1, Key2, comp, JoinMode.Inner)
        {
        }

        public JoinOnAttribute(int Index, string Key1, string Key2, JoinMode mode)
            : this(Index, Key1, Key2, CompareOpration.Equal, mode)
        {
        }

        public JoinOnAttribute(int Index, string Key1, string Key2, CompareOpration comp, JoinMode mode)
        {
            this.Index = Index;
            joinner = new JoinClause(Key1, Key2, comp, mode);
        }
    }
}
