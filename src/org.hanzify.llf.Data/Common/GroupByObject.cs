
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using org.hanzify.llf.Data.Definition;

#endregion

namespace org.hanzify.llf.Data.Common
{
    public class GroupByObject<T>
    {
        // set to key make it as the first column
        [DbKey]
        public T Column;

        public long Count;

        public GroupByObject() { }

        public GroupByObject(T Column, long Count)
        {
            this.Column = Column;
            this.Count = Count;
        }
    }
}
